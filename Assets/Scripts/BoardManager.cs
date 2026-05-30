using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public Character Occupant;
        public CellObject ContainedObject;
    }

    public int MapWidth = 25;
    public int MapHeight = 25;
    public int minRoomWidth = 5;
    public int minRoomHeight = 5;
    public int maxRoomWidth = 8;
    public int maxRoomHeight = 8;
    public int NumberOfRooms = 3;

    private CellData[,] m_BoardData;
    private Tilemap m_Tilemap;
    //private Grid m_Grid;

    public EnemyController[] EnemyPrefab;
    public GroupController GroupController;
    public AllyController[] PartyPrefabs;
    public FoodObject[] FoodPrefab;
    public PotionObject[] PotionPrefab;
    public ChestObject[] ChestPrefab;
    public Tile[] GroundTiles;
    public ExitCellObject ExitCellPrefab;

    public Tile WallTop;
    public Tile WallBottom;
    public Tile WallLeft;
    public Tile WallRight;

    public Tile CornerTopLeft;
    public Tile CornerTopRight;
    public Tile CornerBottomLeft;
    public Tile CornerBottomRight;

    private List<Vector2Int> m_EmptyCellsList;
    private List<RectInt> rooms = new List<RectInt>();
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // arriba
        new Vector2Int(0, -1),  // abajo
        new Vector2Int(-1, 0),  // izquierda
        new Vector2Int(1, 0)    // derecha
    };

    public List<EnemyController> enemyList = new List<EnemyController>();

    public IEnumerator Init()
    {
        GroupController = FindAnyObjectByType<GroupController>();
        m_Tilemap = GetComponentInChildren<Tilemap>();

        // 1. PRIMERO: Inicializamos las listas para que no sean null
        m_EmptyCellsList = new List<Vector2Int>();
        rooms = new List<RectInt>();

        // 2. SEGUNDO: Inicializamos la matriz bidimensional con el tamaño del mapa
        m_BoardData = new CellData[MapWidth, MapHeight];

        // 3. TERCERO: Instanciamos un objeto 'CellData' dentro de cada casilla de la matriz
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                m_BoardData[x, y] = new CellData();
                m_BoardData[x, y].Passable = false; // Por defecto todo es muro/imposible de pisar
            }
        }

        // Ahora Clean() ya no se saldrá por culpa del null, limpiará lo que toque
        Clean();

        // Espera 1: Esperamos a que Unity limpie los objetos viejos del frame
        yield return new WaitForEndOfFrame();

        // 4. CUARTO: Generamos el mapa
        GenerateDungeon();
        CorridorCreator();
        BuildWalls();

        // Espera 2: Esperamos a que el Tilemap se asiente físicamente en el motor
        yield return new WaitForEndOfFrame();

        RepositionParty();
        GenerateEnemy();
        GenerateFood();
        GeneratePotion();
        GenerateChest();
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < NumberOfRooms; i++)
        {
            bool roomPlaced = false;
            int attempts = 0;

            while (!roomPlaced && attempts < 100) // Intentar 100 veces máximo para evitar bucles infinitos
            {
                // Definir tamaño aleatorio para la sala (ejemplo entre 4 y 7)
                int roomW = Random.Range(minRoomWidth, maxRoomWidth);
                int roomH = Random.Range(minRoomHeight, maxRoomHeight);

                // Posición aleatoria
                int posX = Random.Range(0, MapWidth - roomW);
                int posY = Random.Range(0, MapHeight - roomH);

                RectInt newRoom = new RectInt(posX, posY, roomW, roomH);

                // Comprobar si solapa con alguna sala existente
                if (!OverlapsExistingRoom(newRoom))
                {
                    DrawRoom(newRoom);
                    rooms.Add(newRoom);
                    roomPlaced = true;
                }
                attempts++;
            }
        }
        Vector2Int endCoord = GetRandomPointInRoom(rooms[Random.Range(0, rooms.Count)]);
        AddObject(Instantiate(ExitCellPrefab), endCoord);
        //m_EmptyCellsList.Remove(endCoord);
    }

    bool OverlapsExistingRoom(RectInt newRoom)
    {
        foreach (var room in rooms)
        {
            // Añadimos un margen de 1 para que no estén pegadas pared con pared
            if (newRoom.Overlaps(new RectInt(room.x - 3, room.y - 3, room.width + 4, room.height + 4)))
            {
                return true;
            }
        }
        return false;
    }

    void DrawRoom(RectInt room)
    {
        for (int y = 0; y < room.height; y++)
        {
            for (int x = 0; x < room.width; x++)
            {
                Tile tile = null;
                Vector3Int pos = new Vector3Int(room.x + x, room.y + y, 0);

                // Lógica de muros y esquinas relativa a la sala
                // 1. Esquinas

                if (x == 0 && y == room.height - 1)
                {
                    tile = CornerTopLeft;
                }
                else if (x == room.width - 1 && y == room.height - 1)
                {
                    tile = CornerTopRight;
                }
                else if (x == 0 && y == 0)
                {
                    tile = CornerBottomLeft;
                }
                else if (x == room.width - 1 && y == 0)
                {
                    tile = CornerBottomRight;
                }
                // 2. Laterales
                else if (y == room.height - 1)
                {
                    tile = WallTop;
                }
                else if (y == 0)
                {
                    tile = WallBottom;
                }
                else if (x == 0)
                {
                    tile = WallLeft;
                }
                else if (x == room.width - 1)
                {
                    tile = WallRight;
                }
                // 3. Suelo
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    m_BoardData[pos.x, pos.y].Passable = true;
                    m_EmptyCellsList.Add(new Vector2Int(pos.x, pos.y));
                }

                m_Tilemap.SetTile(pos, tile);
            }
        }
    }

    void AddObject(CellObject obj, Vector2Int coord)
    {
        CellData data = m_BoardData[coord.x, coord.y];

        if (data.ContainedObject != null || data.Occupant != null)
        {
            Debug.LogWarning("Cell already occupied!");
            Destroy(obj.gameObject);
            return;
        }

        obj.transform.position = CellToWorld(coord);
        if (obj is Character)
        {
            data.Occupant = obj as Character;
        }else
        {
            data.ContainedObject = obj;
        }    
        obj.Init(coord);
    }

    void CorridorCreator()
    {
        if (rooms.Count == 0) return;

        List<RectInt> connected = new List<RectInt>();
        List<RectInt> remaining = new List<RectInt>(rooms);

        // Empezamos con una sala cualquiera
        int number = Random.Range(0, remaining.Count);
        connected.Add(remaining[number]);
        remaining.RemoveAt(number);

        while (remaining.Count > 0)
        {
            float minDistance = float.MaxValue;
            RectInt bestA = new RectInt();
            RectInt bestB = new RectInt();

            foreach (var roomA in connected)
            {
                foreach (var roomB in remaining)
                {
                    float dist = Vector2Int.Distance(
                        GetRandomPointInRoom(roomA),
                        GetRandomPointInRoom(roomB)
                    );

                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        bestA = roomA;
                        bestB = roomB;
                    }
                }
            }

            // Conectar las dos salas
            ConnectCorridor(
            GetRandomPointInRoom(bestA),
            GetRandomPointInRoom(bestB)
        );

            // Mover roomB a conectadas
            connected.Add(bestB);
            remaining.Remove(bestB);
        }
    }

    void ConnectCorridor(Vector2Int roomA, Vector2Int roomB)
    {
        Vector2Int current = roomA;

        while (current != roomB)
        {

            int cell = Random.Range(0, 2);

            // Añadir casilla horizontalmente (si ya esta alineado crea vertical)
            if (cell == 0)
            {
                if (current.x != roomB.x)
                    current.x += (roomB.x > current.x) ? 1 : -1;
                else
                    current.y += (roomB.y > current.y) ? 1 : -1;
            }
            // Añadir casilla verticalmente (si ya esta alineado crea horizontal)
            else
            {
                if (current.y != roomB.y)
                    current.y += (roomB.y > current.y) ? 1 : -1;
                else
                    current.x += (roomB.x > current.x) ? 1 : -1;
            }

            DrawFloor(current);
        }
    }

    Vector2Int GetRandomPointInRoom(RectInt room)
    {
        int x = Random.Range(room.x + 1, room.x + room.width - 1);
        int y = Random.Range(room.y + 1, room.y + room.height - 1);

        return new Vector2Int(x, y);
    }

    void DrawFloor(Vector2Int pos)
    {
        Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);

        TileBase currentTile = m_Tilemap.GetTile(tilePos);

        bool placedFloor = false;

        if (currentTile != null && IsWall(currentTile))
        {
            Tile tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
            m_Tilemap.SetTile(tilePos, tile);
            placedFloor = true;

            m_BoardData[pos.x, pos.y].Passable = true;

            if (!m_EmptyCellsList.Contains(pos))
                m_EmptyCellsList.Add(pos);
        }
        else if (currentTile == null)
        {
            Tile tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
            m_Tilemap.SetTile(tilePos, tile);
            placedFloor = true;
            if (!m_EmptyCellsList.Contains(pos))
            {
                m_EmptyCellsList.Add(pos);
            }
            m_BoardData[pos.x, pos.y].Passable = true;
        }

        //  SOLO si hemos colocado suelo → dibujar paredes alrededor
        if (placedFloor)
        {
            DrawWallsAround(pos);
            CheckCorner(pos, Vector2Int.up, Vector2Int.right, new Vector2Int(1, 1), CornerTopRight);
            CheckCorner(pos, Vector2Int.up, Vector2Int.left, new Vector2Int(-1, 1), CornerTopLeft);
            CheckCorner(pos, Vector2Int.down, Vector2Int.right, new Vector2Int(1, -1), CornerBottomRight);
            CheckCorner(pos, Vector2Int.down, Vector2Int.left, new Vector2Int(-1, -1), CornerBottomLeft);
        }
    }

    bool IsWall(TileBase tile)
    {
        if (tile == WallTop ||
            tile == WallBottom ||
            tile == WallLeft ||
            tile == WallRight ||
            tile == CornerTopLeft ||
            tile == CornerTopRight ||
            tile == CornerBottomLeft ||
            tile == CornerBottomRight)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void DrawWallsAround(Vector2Int pos)
    {
        foreach (var dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            Vector3Int tilePos = new Vector3Int(neighbor.x, neighbor.y, 0);

            TileBase existingTile = m_Tilemap.GetTile(tilePos);

            // Solo pintar si está vacío
            if (existingTile == null)
            {
                Tile wallTile = null;
                if (dir == Vector2Int.up) wallTile = WallTop;
                else if (dir == Vector2Int.down) wallTile = WallBottom;
                else if (dir == Vector2Int.left) wallTile = WallLeft;
                else if (dir == Vector2Int.right) wallTile = WallRight;
                m_Tilemap.SetTile(tilePos, wallTile);
                m_BoardData[neighbor.x, neighbor.y].Passable = false;
            }
        }
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Tilemap.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= MapWidth
            || cellIndex.y < 0 || cellIndex.y >= MapHeight)
        {
            return null;
        }

        TileBase tile = m_Tilemap.GetTile((Vector3Int)cellIndex);

        if (tile == null)
            return null;

        return m_BoardData[cellIndex.x, cellIndex.y];
    }

    public Vector2Int GetRandomRoomCell()
    {
        return m_EmptyCellsList[Random.Range(0, m_EmptyCellsList.Count)];
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile, bool passable)
    {
        Vector3Int pos = new Vector3Int(cellIndex.x, cellIndex.y, 0);

        m_Tilemap.SetTile(pos, tile);

        m_BoardData[cellIndex.x, cellIndex.y].Passable = passable;

        if (passable)
        {
            if (!m_EmptyCellsList.Contains(cellIndex))
                m_EmptyCellsList.Add(cellIndex);
        }
        else
        {
            m_EmptyCellsList.Remove(cellIndex);
        }
    }

    /*
    public void Clean()
    {
        if (m_BoardData == null)
            return;

        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                var cellData = m_BoardData[x, y];
                if (cellData == null) continue;

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                    cellData.ContainedObject = null;
                }

                if (cellData.Occupant != null)
                {
                    if (cellData.Occupant is AllyController)
                    {
                        cellData.Occupant.gameObject.SetActive(false);                                                
                    }
                    else if (cellData.Occupant is EnemyController enemy)
                    {
                        if (enemy.itemEquipped != null)
                        {
                            Destroy(enemy.itemEquipped.gameObject);
                        }
                        Destroy(enemy.gameObject);
                    }
                    cellData.Occupant = null;
                }
            }
        }

        m_Tilemap.ClearAllTiles();
        m_EmptyCellsList.Clear();
        rooms.Clear();
    }
    */

    public void Clean()
    {
        // 1. DESACTIVAR ALIADOS DE FORMA SEGURA (Sin destruirlos)
        // Usamos GroupController.Instance para asegurarnos de ir al singletón real actual
        var currentGroup = GroupController.Instance != null ? GroupController.Instance : GroupController;
        if (currentGroup != null && currentGroup.Party != null)
        {
            foreach (AllyController ally in currentGroup.Party)
            {
                if (ally != null && ally.gameObject != null)
                {
                    ally.gameObject.SetActive(false);
                    // Si el aliado está en la escena, nos aseguramos de quitarle el padre temporalmente
                    // para que no se destruya si estuviera emparentado con el Tilemap o la Grid
                    ally.transform.SetParent(null);
                }
            }
        }

        // 2. DESTRUIR ENEMIGOS DESDE SU LISTA OFICIAL
        if (enemyList != null)
        {
            foreach (EnemyController enemy in enemyList)
            {
                if (enemy != null)
                {
                    if (enemy.itemEquipped != null)
                    {
                        Destroy(enemy.itemEquipped.gameObject);
                    }
                    Destroy(enemy.gameObject);
                }
            }
            enemyList.Clear();
        }

        // 3. LIMPIAR EL MAPA CASILLA POR CASILLA
        if (m_BoardData != null)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                for (int x = 0; x < MapWidth; x++)
                {
                    var cellData = m_BoardData[x, y];
                    if (cellData == null) continue;

                    if (cellData.ContainedObject != null)
                    {
                        // VALIDACIÓN CRÍTICA: Asegurarnos de que el objeto contenedor NO sea un aliado camuflado
                        if (cellData.ContainedObject.GetComponent<AllyController>() == null)
                        {
                            Destroy(cellData.ContainedObject.gameObject);
                        }
                        cellData.ContainedObject = null;
                    }

                    if (cellData.Occupant != null)
                    {
                        // VALIDACIÓN CRÍTICA: Asegurarnos de que si es un Enemigo suelto por ahí se destruya,
                        // pero si es un Aliado, SOLO quitamos la referencia de la matriz, NUNCA lo destruimos.
                        if (cellData.Occupant is EnemyController enemySuelto)
                        {
                            if (enemySuelto != null) Destroy(enemySuelto.gameObject);
                        }

                        cellData.Occupant = null; // Liberamos la casilla
                    }
                }
            }
        }

        // 4. LIMPIAR ESTRUCTURAS
        if (m_Tilemap != null)
        {
            m_Tilemap.ClearAllTiles();
        }

        if (m_EmptyCellsList != null) m_EmptyCellsList.Clear();
        if (rooms != null) rooms.Clear();
    }

    void GenerateEnemy()
    {
        int enemyCount = Random.Range(1, 3);
        for (int i = 0; i < enemyCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            EnemyController newEnemy = Instantiate(EnemyPrefab[Random.Range(0, EnemyPrefab.Length)]);
            enemyList.Add(newEnemy);
            AddObject(newEnemy, coord);
        }
    }

    private void RepositionParty()
    {
        if (rooms == null || rooms.Count == 0)
        {
            Debug.LogError("¡No hay salas (rooms) generadas para posicionar a la party!");
            return;
        }

        // 1. Elegir una sala completamente al azar
        RectInt startingRoom = rooms[Random.Range(0, rooms.Count)];

        // 2. En lugar de un punto totalmente aleatorio que pueda estar pegado a una puerta,
        // calculamos el CENTRO exacto de la sala elegida.
        int centerX = startingRoom.x + (startingRoom.width / 2);
        int centerY = startingRoom.y + (startingRoom.height / 2);
        Vector2Int leadearcoord = new Vector2Int(centerX, centerY);

        List<Vector2Int> list = new List<Vector2Int>()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left,
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
    };

        for (int i = 0; i < GroupController.Party.Count; ++i)
        {
            AllyController character = GroupController.Party[i];
            if (character == null || character.isDead) continue;

            Vector2Int coord = leadearcoord;

            if (i == 0)
            {
                coord = leadearcoord;
            }
            else
            {
                bool validCoordFound = false;
                List<Vector2Int> alternativeDirections = new List<Vector2Int>(list);

                while (alternativeDirections.Count > 0)
                {
                    int randomDirIndex = Random.Range(0, alternativeDirections.Count);
                    Vector2Int potentialCoord = leadearcoord + alternativeDirections[randomDirIndex];
                    alternativeDirections.RemoveAt(randomDirIndex);

                    if (potentialCoord.x >= 0 && potentialCoord.x < MapWidth &&
                        potentialCoord.y >= 0 && potentialCoord.y < MapHeight)
                    {
                        CellData cell = m_BoardData[potentialCoord.x, potentialCoord.y];

                        // COMPROBACIÓN GEOMÉTRICA ESTRICTA:
                        // Forzamos matemáticamente que la coordenada esté DENTRO del rectángulo de la sala,
                        // ignorando si las casillas de fuera (como los pasillos) son Passable o no.
                        // Usamos un margen de +1 y -1 para evitar también los muros perimetrales de la sala.
                        bool insideRoomX = potentialCoord.x >= (startingRoom.x + 1) && potentialCoord.x <= (startingRoom.x + startingRoom.width - 2);
                        bool insideRoomY = potentialCoord.y >= (startingRoom.y + 1) && potentialCoord.y <= (startingRoom.y + startingRoom.height - 2);

                        if (insideRoomX && insideRoomY)
                        {
                            if (cell != null && cell.Passable && cell.Occupant == null && cell.ContainedObject == null)
                            {
                                coord = potentialCoord;
                                validCoordFound = true;
                                break;
                            }
                        }
                    }
                }

                // Plan de contingencia: Si por algún motivo el centro está obstruido por un cofre o trampa,
                // buscamos cualquier punto aleatorio que cumpla obligatoriamente con estar dentro del rectángulo de la sala.
                if (!validCoordFound)
                {
                    int safetyAttempts = 0;
                    while (!validCoordFound && safetyAttempts < 50)
                    {
                        // GetRandomPointInRoom ya garantiza por su diseño interno que el punto es interior a la sala
                        Vector2Int randomRoomCoord = GetRandomPointInRoom(startingRoom);
                        CellData cell = m_BoardData[randomRoomCoord.x, randomRoomCoord.y];

                        if (cell != null && cell.Passable && cell.Occupant == null && cell.ContainedObject == null)
                        {
                            coord = randomRoomCoord;
                            validCoordFound = true;
                        }
                        safetyAttempts++;
                    }
                }
            }

            // Aplicamos la posición segura dentro de la sala
            m_EmptyCellsList.Remove(coord);

            character.gameObject.SetActive(true);
            AddObject(character, coord);

            SPUM_Prefabs spumComponent = character.GetComponent<SPUM_Prefabs>();
            if (spumComponent != null)
            {
                spumComponent.gameObject.SetActive(false);
                spumComponent.gameObject.SetActive(true);
            }
        }
    }

    private void GenerateFood()
    {
        int foodCount = Random.Range(1, 3);
        for (int i = 0; i < foodCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            FoodObject newFood = Instantiate(FoodPrefab[Random.Range(0, FoodPrefab.Length)]);
            AddObject(newFood, coord);
        }
    }

    private void GenerateChest()
    {
        int chestCount = Random.Range(1, 3);
        for (int i = 0; i < chestCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            ChestObject newChest = Instantiate(ChestPrefab[Random.Range(0, ChestPrefab.Length)]);
            AddObject(newChest, coord);
        }
    }

    private void GeneratePotion()
    {
        int potionCount = Random.Range(0, 2);
        for (int i = 0; i < potionCount; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            PotionObject newPotion = Instantiate(PotionPrefab[Random.Range(0, PotionPrefab.Length)]);
            AddObject(newPotion, coord);
        }
    }

    bool IsFloor(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= MapWidth ||
            pos.y < 0 || pos.y >= MapHeight)
            return false;

        return m_BoardData[pos.x, pos.y].Passable;
    }

    void BuildWalls()
    {
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (IsFloor(pos))
                    continue;

                bool up = IsFloor(pos + Vector2Int.up);
                bool down = IsFloor(pos + Vector2Int.down);
                bool left = IsFloor(pos + Vector2Int.left);
                bool right = IsFloor(pos + Vector2Int.right);

                Tile tile = null;

                if (down && right)
                {
                    tile = CornerBottomRight;
                }
                else if (down && left)
                {
                    tile = CornerBottomLeft;
                }
                else if (up && right)
                {
                    tile = CornerTopRight;
                }
                else if (up && left)
                {
                    tile = CornerTopLeft;
                }
                else if (down)
                {
                    tile = WallTop;
                }
                else if (up)
                {
                    tile = WallBottom;
                }
                else if (right)
                {
                    tile = WallLeft;
                }
                else if (left)
                {
                    tile = WallRight;
                }
                if (tile != null)
                {
                    m_Tilemap.SetTile((Vector3Int)pos, tile);
                }
            }
        }
    }

    void CheckCorner(Vector2Int center, Vector2Int dirA, Vector2Int dirB, Vector2Int diagonal, Tile cornerTile)
    {
        Vector2Int posA = center + dirA;
        Vector2Int posB = center + dirB;
        Vector2Int diag = center + diagonal;

        if (!(diag.x >= 0 && diag.x < MapWidth && diag.y >= 0 && diag.y < MapHeight))
            return;

        TileBase tileA = m_Tilemap.GetTile((Vector3Int)posA);
        TileBase tileB = m_Tilemap.GetTile((Vector3Int)posB);
        TileBase tileDiag = m_Tilemap.GetTile((Vector3Int)diag);

        bool aIsWall = tileA != null && IsWall(tileA);
        bool bIsWall = tileB != null && IsWall(tileB);

        if (aIsWall && bIsWall && tileDiag == null)
        {
            m_Tilemap.SetTile((Vector3Int)diag, cornerTile);
            m_BoardData[diag.x, diag.y].Passable = false;
        }
    }
}

