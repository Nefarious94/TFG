using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
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
    public Character[] PartyPrefabs;
    public FoodObject[] FoodPrefab;
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

    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        //m_Grid = GetComponentInChildren<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();
        m_BoardData = new CellData[MapWidth, MapHeight];
        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
                m_BoardData[x, y] = new CellData();
                m_BoardData[x, y].Passable = false;
            }
        }
        GenerateDungeon();
        CorridorCreator();
        GenerateEnemy();
        GenerateFood();
        GenerateChest();
        if (!GroupController.HasParty)
        {
            SpawnParty();
        }
        else
        {
            RepositionParty();
        }
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

        if (data.ContainedObject != null)
        {
            Debug.LogWarning("Cell already occupied!");
            Destroy(obj.gameObject);
            return;
        }

        obj.transform.position = CellToWorld(coord);
        data.ContainedObject = obj;
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

            int cell = Random.Range(0, 1);

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

    public void Clean()
    {
        if (m_BoardData == null)
            return;

        for (int y = 0; y < MapHeight; y++)
        {
            for (int x = 0; x < MapWidth; x++)
            {
                var cellData = m_BoardData[x, y];

                if (cellData != null && cellData.ContainedObject != null)
                {
                    if (!(cellData.ContainedObject is Character))
                    {
                        Destroy(cellData.ContainedObject.gameObject);                        
                    }
                    cellData.ContainedObject = null;
                }
            }
        }

        m_Tilemap.ClearAllTiles();
        m_EmptyCellsList.Clear();
        rooms.Clear();
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
            AddObject(newEnemy, coord);
        }
    }

    void SpawnParty()
    {
        int partySize = Random.Range(1, 3);
        for (int i = 0; i < partySize; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            Character newChar = Instantiate(PartyPrefabs[Random.Range(0, PartyPrefabs.Length)]);
            AddObject(newChar, coord);

            GroupController.AddCharacter(newChar);
        }
    }

    private void RepositionParty()
    {
        for (int i = 0;i < GroupController.Party.Count; ++i)
        {
            int randomIndex = Random.Range(0, m_EmptyCellsList.Count);
            Vector2Int coord = m_EmptyCellsList[randomIndex];

            m_EmptyCellsList.RemoveAt(randomIndex);
            Character character = GroupController.Party[i];
            AddObject(character, coord);
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
}