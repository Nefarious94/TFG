using System.Collections.Generic;
using UnityEngine;
using static Ability;
using static BoardManager;


public class AllyController : Character
{
    private CellObject lastCell;

    public int currentExp;
    public int needExp = 5;

    public bool isDead = false;
    public bool action = true;

    public Item Head;
    public Item Chest;
    public Item Arms;
    public Item Legs;
    public Item Boots;
    public Item RightHand;
    public Item LeftHand;

    public int bonusStrength;
    public int bonusDexterity;
    public int bonusVitality;
    public int bonusIntelligence;

    public bool TryMove(Vector2Int target)
    {
        if (GameManager.Instance.TurnManager.ProjectileActive)
        {
            return false;
        }
        var cellData = m_Board.GetCellData(target);

        if (cellData == null || !cellData.Passable)
            return false;

        if (cellData.Occupant != null)
        {
            if (!cellData.Occupant.PlayerWantsToEnter())
                return false;
        }

        return MoveTo(target);
    }

    public bool MoveTo(Vector2Int target)
    {
        var targetCell = m_Board.GetCellData(target);

        if (!targetCell.Passable)
            return false;

        var currentCell = m_Board.GetCellData(m_Cell);

        if (currentCell.ContainedObject is ExitCellObject exit)
        {
            exit.ResetExit();
        }

        currentCell.Occupant = null;

        m_Cell = target;

        targetCell.Occupant = this;

        transform.position = m_Board.CellToWorld(target);

        if (this == GameManager.Instance.GroupController.ActiveCharacter)
        {
            if (targetCell.ContainedObject != null)
            {
                targetCell.ContainedObject.PlayerEntered();
            }
        }

        return true;
    }

    /*
    public void AutoMove()
    {
        EnemyController enemy = GetNearbyEnemy(2);

        int xDist;
        int yDist;

        int absXDist;
        int absYDist;

        if (enemy != null)
        {
            var enemyCell = enemy.Cell;

            xDist = enemyCell.x - m_Cell.x;
            yDist = enemyCell.y - m_Cell.y;

            absXDist = Mathf.Abs(xDist);
            absYDist = Mathf.Abs(yDist);

            //if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
            if (absYDist <= 1 && absXDist <= 1)
            {
                enemy.TakeDamage(attack, this);
                GameManager.Instance.TurnManager.EndAllyTurn();
                return;
            }
            else
            {
                /*
                if (absXDist > absYDist)
                {
                    if (!TryMoveInX(xDist))
                    {
                        TryMoveInY(yDist);
                    }
                }
                else
                {
                    if (!TryMoveInY(yDist))
                    {
                        TryMoveInX(xDist);
                    }
                }
                
                if (!TryMoveDiagonal(xDist, yDist))
                {
                    if (absXDist > absYDist)
                    {
                        if (!TryMoveInX(xDist))
                        {
                            TryMoveInY(yDist);
                        }
                    }
                    else
                    {
                        if (!TryMoveInY(yDist))
                        {
                            TryMoveInX(xDist);
                        }
                    }
                }
            }
            GameManager.Instance.TurnManager.EndAllyTurn();
            return;
        }
        var player = GameManager.Instance.GroupController.ActiveCharacter;
        var playerCell = player.Cell;

        xDist = playerCell.x - m_Cell.x;
        yDist = playerCell.y - m_Cell.y;

        absXDist = Mathf.Abs(xDist);
        absYDist = Mathf.Abs(yDist);

        if (absXDist > 3 || absYDist > 3)
        {
            bool moved = false;
            int attempts = 0;
            while (!moved && attempts < 100)
            {
                attempts++;
                string[] move = { "up", "down", "right", "left" };
                string chosenMove = move[Random.Range(0, move.Length)];
                switch (chosenMove)
                {
                    case "up":
                        moved = MoveTo(m_Cell + Vector2Int.up);
                        break;

                    case "down":
                        moved = MoveTo(m_Cell + Vector2Int.down);
                        break;

                    case "right":
                        moved = MoveTo(m_Cell + Vector2Int.right);
                        break;

                    case "left":
                        moved = MoveTo(m_Cell + Vector2Int.left);
                        break;
                }
            }
        }
        else
        {
            if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
            {
            }
            else
            {
                if (absXDist > absYDist)
                {
                    if (!TryMoveInX(xDist))
                    {
                        TryMoveInY(yDist);
                    }
                }
                else
                {
                    if (!TryMoveInY(yDist))
                    {
                        TryMoveInX(xDist);
                    }
                }
            }
        }
        GameManager.Instance.TurnManager.EndAllyTurn();

        var player = GameManager.Instance.GroupController.ActiveCharacter;
        var playerCell = player.Cell;

        xDist = playerCell.x - m_Cell.x;
        yDist = playerCell.y - m_Cell.y;

        absXDist = Mathf.Abs(xDist);
        absYDist = Mathf.Abs(yDist);

        if (absXDist > 3 || absYDist > 3)
        {
            bool moved = false;
            int attempts = 0;

            while (!moved && attempts < 100)
            {
                attempts++;

                Vector2Int[] directions =
                {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right,

                // diagonales
                new Vector2Int(1, 1),
                new Vector2Int(-1, 1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1)
            };

                Vector2Int dir =
                    directions[Random.Range(0, directions.Length)];

                moved = MoveTo(m_Cell + dir);
            }
        }
        else
        {
            // si está adyacente no hace nada
            if (!(absXDist <= 1 &&
                  absYDist <= 1))
            {
                // intentar diagonal primero
                if (!TryMoveDiagonal(xDist, yDist))
                {
                    // fallback cardinal
                    if (absXDist > absYDist)
                    {
                        if (!TryMoveInX(xDist))
                        {
                            TryMoveInY(yDist);
                        }
                    }
                    else
                    {
                        if (!TryMoveInY(yDist))
                        {
                            TryMoveInX(xDist);
                        }
                    }
                }
            }
        }

        GameManager.Instance.TurnManager.EndAllyTurn();
    }
    */

    public void AutoMove()
    {
        if (isDead) {
            GameManager.Instance.TurnManager.EndAllyTurn();
            return; } 

        int xDist = int.MaxValue;
        int yDist = int.MaxValue;
        int absXDist = int.MaxValue;
        int absYDist = int.MaxValue;

        int auxXDist;
        int auxYDist;

        var enemies = GameManager.Instance.BoardManager.enemyList;
        int minDistance = int.MaxValue;
        EnemyController enemy = null;

        // 1. Buscar al enemigo más cercano
        for (int i = 0; i < enemies.Count; i++)
        {
            var enemyCell = enemies[i].Cell;
            auxXDist = enemyCell.x - m_Cell.x;
            auxYDist = enemyCell.y - m_Cell.y;
            int dist = Mathf.Abs(auxXDist) + Mathf.Abs(auxYDist);
            if (minDistance > dist)
            {
                minDistance = dist;
                xDist = auxXDist;
                yDist = auxYDist;
                absXDist = Mathf.Abs(auxXDist);
                absYDist = Mathf.Abs(auxYDist);
                enemy = enemies[i];
            }
        }

        // 2. Comportamiento si hay un enemigo cerca (Distancia <= 3)
        if (enemy != null && minDistance <= 3)
        {
            if (absXDist <= 1 && absYDist <= 1)
            {
                enemy.TakeDamage(attack, this);
                GameManager.Instance.TurnManager.EndAllyTurn();
                return; // Atacó, terminamos turno aquí de forma segura
            }

            // Intentar moverse hacia el enemigo si no está adyacente
            if (!TryMoveDiagonal(xDist, yDist))
            {
                if (absXDist > absYDist)
                {
                    if (!TryMoveInX(xDist)) TryMoveInY(yDist);
                }
                else
                {
                    if (!TryMoveInY(yDist)) TryMoveInX(xDist);
                }
            }

            GameManager.Instance.TurnManager.EndAllyTurn();
            return; // Se movió hacia el enemigo, terminamos
        }

        // 3. Comportamiento si NO hay enemigos cerca: Seguir al jugador activo
        var player = GameManager.Instance.GroupController.ActiveCharacter;

        // Si el jugador activo soy yo mismo y no hay enemigos, no hago nada
        if (player != this)
        {
            Vector2Int playerCell = player.Cell;
            xDist = playerCell.x - m_Cell.x;
            yDist = playerCell.y - m_Cell.y;
            absXDist = Mathf.Abs(xDist);
            absYDist = Mathf.Abs(yDist);

            // Si ya estoy al lado del jugador, no nos movemos más
            if (absXDist > 1 || absYDist > 1)
            {
                if (absXDist <= 3 && absYDist <= 3)
                {
                    // Moverse inteligentemente hacia el jugador
                    if (!TryMoveDiagonal(xDist, yDist))
                    {
                        if (absXDist > absYDist)
                        {
                            if (!TryMoveInX(xDist)) TryMoveInY(yDist);
                        }
                        else
                        {
                            if (!TryMoveInY(yDist)) TryMoveInX(xDist);
                        }
                    }
                }
                else
                {
                    // El jugador está muy lejos, moverse al azar para buscar camino
                    bool moved = false;
                    int attempts = 0;
                    while (!moved && attempts < 100)
                    {
                        attempts++;
                        Vector2Int[] directions = {
                        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                        new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1)
                    };
                        Vector2Int dir = directions[Random.Range(0, directions.Length)];
                        moved = MoveTo(m_Cell + dir);
                    }
                }
            }
        }

        // 4. Cierre seguro: Cualquier camino que llegue hasta aquí finalizará el turno
        GameManager.Instance.TurnManager.EndAllyTurn();
    }

    /*
    bool TryMoveInX(int xDist)
    {
        //try to get closer in x

        //player to our right
        if (xDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.right);
        }

        //player to our left
        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        //try to get closer in y

        //player on top
        if (yDist > 0)
        {
            return MoveTo(m_Cell + Vector2Int.up);
        }

        //player below
        return MoveTo(m_Cell + Vector2Int.down);
    }

    bool TryMoveDiagonal(int xDist, int yDist)
    {
        Vector2Int dir = Vector2Int.zero;

        if (xDist > 0)
            dir.x = 1;
        else if (xDist < 0)
            dir.x = -1;

        if (yDist > 0)
            dir.y = 1;
        else if (yDist < 0)
            dir.y = -1;

        // si no es diagonal real
        if (dir.x == 0 || dir.y == 0)
            return false;

        return MoveTo(m_Cell + dir);
    }
    */

    private bool TryMoveInX(int xDist)
    {
        Vector2Int primaryDir = xDist > 0 ? Vector2Int.right : Vector2Int.left;

        if (IsCellFree(m_Cell + primaryDir))
            return MoveTo(m_Cell + primaryDir);

        // Si la ruta óptima en X está tapada por un aliado, intenta flanquear por Y
        if (IsCellFree(m_Cell + Vector2Int.up)) return MoveTo(m_Cell + Vector2Int.up);
        if (IsCellFree(m_Cell + Vector2Int.down)) return MoveTo(m_Cell + Vector2Int.down);

        return false;
    }

    private bool TryMoveInY(int yDist)
    {
        Vector2Int primaryDir = yDist > 0 ? Vector2Int.up : Vector2Int.down;

        if (IsCellFree(m_Cell + primaryDir))
            return MoveTo(m_Cell + primaryDir);

        // Si la ruta óptima en Y está tapada por un aliado, intenta flanquear por X
        if (IsCellFree(m_Cell + Vector2Int.right)) return MoveTo(m_Cell + Vector2Int.right);
        if (IsCellFree(m_Cell + Vector2Int.left)) return MoveTo(m_Cell + Vector2Int.left);

        return false;
    }

    private bool TryMoveDiagonal(int xDist, int yDist)
    {
        Vector2Int dir = Vector2Int.zero;
        if (xDist > 0) dir.x = 1; else if (xDist < 0) dir.x = -1;
        if (yDist > 0) dir.y = 1; else if (yDist < 0) dir.y = -1;

        if (dir.x == 0 || dir.y == 0) return false;

        if (IsCellFree(m_Cell + dir))
            return MoveTo(m_Cell + dir);

        // Si la diagonal ideal está bloqueada por un aliado, intenta avanzar de forma recta
        if (IsCellFree(m_Cell + new Vector2Int(dir.x, 0))) return MoveTo(m_Cell + new Vector2Int(dir.x, 0));
        if (IsCellFree(m_Cell + new Vector2Int(0, dir.y))) return MoveTo(m_Cell + new Vector2Int(0, dir.y));

        return false;
    }

    // --- FILTRO DE ENTRADA UTILIZANDO TU CELLDATA ---
    private bool IsCellFree(Vector2Int targetCell)
    {
        CellData cellData = GameManager.Instance.BoardManager.GetCellData(targetCell);

        if (cellData == null || !cellData.Passable)
            return false;

        // Comprobamos tu variable 'Occupant' usando polimorfismo (herencia)
        if (cellData.Occupant != null && cellData.Occupant is AllyController)
        {
            return false; // Casilla considerada bloqueada porque hay un aliado tuyo
        }

        return true;
    }

    public override bool PlayerWantsToEnter()
    {
        var group = GameManager.Instance.BoardManager.GroupController;
        var character = group.ActiveCharacter;

        if (group.groupMode)
        {
            return false;
        }
        else
        {
            SwapPositions(character);
            return true;
        }
    }

    private void SwapPositions(AllyController other)
    {
        var board = m_Board;

        Vector2Int myCell = m_Cell;
        Vector2Int otherCell = other.m_Cell;

        var myCellData = board.GetCellData(myCell);
        var otherCellData = board.GetCellData(otherCell);

        // intercambiar en grid
        myCellData.ContainedObject = other;
        otherCellData.ContainedObject = this;

        // intercambiar coords
        m_Cell = otherCell;
        other.m_Cell = myCell;

        // actualizar posiciones visuales
        transform.position = board.CellToWorld(m_Cell);
        other.transform.position = board.CellToWorld(other.m_Cell);
    }

    public EnemyController GetNearbyEnemy(int range)
    {
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int checkCoord = m_Cell + new Vector2Int(x, y);
                var cellData = m_Board.GetCellData(checkCoord);

                if (cellData != null && cellData.ContainedObject is EnemyController enemy)
                {
                    return enemy;
                }
            }
        }
        return null;
    }

    public void UsePotion(Item item)
    {
        switch (item.itemName)
        {
            case "HP potion":
                currentHP += item.stat_1_value;
                if (currentHP > maxHP)
                {
                    currentHP = maxHP;
                }
                break;

            case "Mana potion":
                currentMana += item.stat_1_value;
                if (currentMana > maxMana)
                {
                    currentMana = maxMana;
                }
                break;
        }    
    }

    public void TakeDamage(int amount, EnemyController enemy)
    {
        int n = Random.Range(0, 100);
        int finalDamage;
        if (n < enemy.rateCrit)
        {
            int baseDamage = Mathf.Max(1, amount - defense);
            finalDamage = Mathf.RoundToInt(baseDamage * 1.5f);
        }
        else
        {
            finalDamage = Mathf.Max(1, amount - defense);
        }    
        currentHP -= finalDamage;
        Debug.Log("Damage received: " + amount + ", HP left: " + currentHP);
        GroupController group = GameManager.Instance.GroupController;
        if (currentHP <= 0)
        {
            var cell = m_Board.GetCellData(m_Cell);
            cell.Occupant = null;
            this.gameObject.SetActive(false);
            isDead = true;
            Debug.Log(name + " died");
            group.Death(this);
            if (this == group.ActiveCharacter)
            {
                group.NextCharacter();
            }
        }
    }

    public void EnemyKill(EnemyController enemy)
    {
        currentExp += enemy.givenExp;
        if (currentExp == needExp)
        {
            level++;
            currentExp = 0;
            CalculateStats();
            CheckAbilities();
        }
        else if (currentExp > needExp)
        {
            level++;
            currentExp -= needExp;
            CalculateStats();
            CheckAbilities();
        }
    }

    public List<AllyController> GetNearbyAllies(int range)
    {
        List<AllyController> nearbyAllies = new List<AllyController>();

        // Obtenemos todos los miembros de la party del GroupController
        var party = GameManager.Instance.BoardManager.GroupController.Party;

        for (int i = 0; i < party.Count; i++)
        {
            // Saltamos si el aliado está muerto
            if (party[i].isDead) continue;

            // Calculamos la distancia Manhattan entre el personaje actual y el aliado de la lista
            int distX = Mathf.Abs(party[i].Cell.x - this.Cell.x);
            int distY = Mathf.Abs(party[i].Cell.y - this.Cell.y);
            int totalDistance = distX + distY;

            // Si está dentro del rango de la habilidad (incluyéndose a sí mismo, totalDistance == 0)
            if (totalDistance <= range)
            {
                nearbyAllies.Add(party[i]);
            }
        }

        return nearbyAllies;
    }

    public void UseAbility(Ability ability, Vector2Int direction)
    {
        switch (ability.type)
        {
            case AbilityType.Ranged:
                LaunchProjectile(ability, direction);
                break;

            case AbilityType.Melee:
                MeleeAbility(ability, direction);
                break;
        }
        currentMana--; 
        if (GroupController.Instance.groupMode)
        {
            GroupController.Instance.NextCharacter();
        }
    }

    public void UseAbility(Ability ability, AllyController ally)
    {
        switch (ability.type)
        {
            case AbilityType.Healing:
                LaunchHealing(ability, ally);
                break;

            case AbilityType.Buff:
                LaunchBuff(ability, ally);
                break;
        }
        currentMana--;
        if (GroupController.Instance.groupMode)
        {
            GroupController.Instance.NextCharacter();
        }
    }

    void LaunchHealing(Ability ability, AllyController ally)
    {
        switch (ability.abilityName)
        {
            case "Heal":
                ally.currentHP += intelligence / 4;
                if (ally.currentHP >= ally.maxHP)
                {
                    currentHP = ally.maxHP;
                }
                break;
        }
    }

    void LaunchBuff(Ability ability, AllyController ally)
    {
        switch (ability.abilityName)
        {
            case "Protect":
                ally.isBuffed = true;
                ally.buffStat = "defense";
                ally.buffValue = ally.defense;
                ally.buffTurns = 3;
                CalculateStats();
                break;
        }
    }

    void LaunchProjectile(Ability ability, Vector2Int dir)
    {
        GameManager.Instance.TurnManager.StartProjectile();

        GameObject obj = Instantiate(ability.projectilePrefab, transform.position, Quaternion.identity);

        if (dir == Vector2Int.up)
            obj.transform.rotation = Quaternion.Euler(0, 0, 90);

        else if (dir == Vector2Int.down)
            obj.transform.rotation = Quaternion.Euler(0, 0, -90);

        else if (dir == Vector2Int.left)
            obj.transform.rotation = Quaternion.Euler(0, 0, 180);

        else if (dir == Vector2Int.right)
            obj.transform.rotation = Quaternion.Euler(0, 0, 0);

        else if (dir == new Vector2Int(1, 1))
            obj.transform.rotation = Quaternion.Euler(0, 0, 45);

        else if (dir == new Vector2Int(-1, 1))
            obj.transform.rotation = Quaternion.Euler(0, 0, 135);

        else if (dir == new Vector2Int(1, -1))
            obj.transform.rotation = Quaternion.Euler(0, 0, -45);

        else if (dir == new Vector2Int(-1, -1))
            obj.transform.rotation = Quaternion.Euler(0, 0, -135);

        Projectile projectile = obj.GetComponent<Projectile>();

        projectile.Init(m_Cell, dir, ability.range, ability.power, this, OnProjectileFinished);
    }

    void MeleeAbility(Ability ability, Vector2Int dir)
    {
        List<Vector2Int> pattern = GetAttackPattern(ability.range);

        foreach (var offset in pattern)
        {
            Vector2Int rotatedOffset = RotateOffset(offset, dir);

            Vector2Int targetCell = Cell + rotatedOffset;

            var cellData = GameManager.Instance.BoardManager.GetCellData(targetCell);

            if (cellData != null && cellData.Occupant is EnemyController enemy)
            {
                int damage;
                switch (ability.abilityName)
                {
                    case "Double Punch":
                        damage = attack * 2;
                        enemy.TakeDamage(damage, this);
                        break;

                    case "Slash":
                        damage = attack;
                        enemy.TakeDamage(damage, this);
                        break;

                    case "Bloodthirsty Dagger":
                        damage = attack + dexterity;
                        enemy.TakeDamage(damage, this);
                        currentHP += (damage - enemy.defense) / 2;
                        if (currentHP >= maxHP)
                        {
                            maxHP = currentHP;
                        }
                        break;
                }
            }
        }
        GameManager.Instance.TurnManager.EndAllyTurn();
    }

    List<Vector2Int> GetAttackPattern(int range)
    {
        switch (range)
        {
            // Ataque frontal (1 casilla adelante)
            case 0:
                return new List<Vector2Int>() { new Vector2Int(0, 1) };

            // Barrido de 3 casillas frontales
            case 1:
                return new List<Vector2Int>()
            {
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1)
            };

            // Barrido + laterales (5 casillas)
            case 2:
                return new List<Vector2Int>()
            {
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                new Vector2Int(-1, 0),
                new Vector2Int(1, 0)
            };
        }
        return new List<Vector2Int>();
    }

    Vector2Int RotateOffset(Vector2Int offset, Vector2Int dir)
    {
        // Si atacamos hacia ARRIBA, el patrón se queda tal cual
        if (dir == Vector2Int.up) return offset;

        // Si atacamos hacia ABAJO, invertimos los ejes de forma simétrica
        if (dir == Vector2Int.down) return new Vector2Int(-offset.x, -offset.y);

        // Si atacamos hacia la DERECHA, rotamos 90 grados hacia la derecha
        if (dir == Vector2Int.right) return new Vector2Int(offset.y, -offset.x);

        // Si atacamos hacia la IZQUIERDA, rotamos 90 grados hacia la izquierda
        if (dir == Vector2Int.left) return new Vector2Int(-offset.y, offset.x);


        // =========================================================================
        // CORRECCIÓN PARA LAS DIAGONALES (Evita que el patrón se estire o se pierda)
        // =========================================================================

        // Si el offset es la casilla central del ataque (0, 1), se convierte directamente en la diagonal
        if (offset == new Vector2Int(0, 1))
        {
            return dir;
        }

        // Para las casillas laterales del barrido, las recalculamos para que se conviertan 
        // en los dos lados adyacentes de la diagonal (Arriba e Izquierda en tu caso)
        if (dir == new Vector2Int(-1, 1)) // ARRIBA IZQUIERDA
        {
            if (offset == new Vector2Int(-1, 1)) return Vector2Int.left; // El lateral izquierdo golpea a la IZQUIERDA
            if (offset == new Vector2Int(1, 1)) return Vector2Int.up;   // El lateral derecho golpea ARRIBA
        }

        if (dir == new Vector2Int(1, 1)) // ARRIBA DERECHA
        {
            if (offset == new Vector2Int(-1, 1)) return Vector2Int.up;
            if (offset == new Vector2Int(1, 1)) return Vector2Int.right;
        }

        if (dir == new Vector2Int(-1, -1)) // ABAJO IZQUIERDA
        {
            if (offset == new Vector2Int(-1, 1)) return Vector2Int.down;
            if (offset == new Vector2Int(1, 1)) return Vector2Int.left;
        }

        if (dir == new Vector2Int(1, -1)) // ABAJO DERECHA
        {
            if (offset == new Vector2Int(-1, 1)) return Vector2Int.right;
            if (offset == new Vector2Int(1, 1)) return Vector2Int.down;
        }

        return offset;
    }

    void OnProjectileFinished()
    {
        GameManager.Instance.TurnManager.EndProjectile();
        GameManager.Instance.TurnManager.EndAllyTurn();
    }

    public bool EquipArmor(Item armor)
    {
        if (armor.armorType == armorType)
        {
            if (Head == null && armor.itemSubType == Item.SubType.Head)
            {
                Head = armor;
            }
            else if (Head != null && armor.itemSubType == Item.SubType.Head)
            {
                GameManager.Instance.inventoryPlayer.addItem(Head);
                Head = armor;
            }

            if (Chest == null && armor.itemSubType == Item.SubType.Chest)
            {
                Chest = armor;
            }
            else if (Chest != null && armor.itemSubType == Item.SubType.Chest)
            {
                GameManager.Instance.inventoryPlayer.addItem(Chest);
                Chest = armor;
            }

            if (Arms == null && armor.itemSubType == Item.SubType.Arm)
            {
                Arms = armor;
            }
            else if (Arms != null && armor.itemSubType == Item.SubType.Arm)
            {
                GameManager.Instance.inventoryPlayer.addItem(Arms);
                Arms = armor;
            }

            if (Legs == null && armor.itemSubType == Item.SubType.Legs)
            {
                Legs = armor;
            }
            else if (Legs != null && armor.itemSubType == Item.SubType.Legs)
            {
                GameManager.Instance.inventoryPlayer.addItem(Legs);
                Legs = armor;
            }

            if (Boots == null && armor.itemSubType == Item.SubType.Boots)
            {
                Boots = armor;
            }
            else if (Boots != null && armor.itemSubType == Item.SubType.Boots)
            {
                GameManager.Instance.inventoryPlayer.addItem(Boots);
                Boots = armor;
            }
            this.CalculateStats();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool EquipWeapon(Item weapon)
    {
        if (weapon.weaponType == weaponType)
        {
            if (RightHand == null)
            {
                RightHand = weapon;
                GameManager.Instance.inventoryPlayer.removeItem(weapon);
            }
            else if (RightHand != null)
            {
                GameManager.Instance.inventoryPlayer.addItem(RightHand);
                RightHand = weapon;
            }
            this.CalculateStats();
            return true;
        }
        return false;
    }

    public void CalculateEquipment()
    {
        bonusStrength = 0;
        bonusDexterity = 0;
        bonusVitality = 0;
        bonusIntelligence = 0;

        if (Head != null) { AddBonusStats(Head.stat_1, Head.stat_1_value); }
        if (Arms != null) { AddBonusStats(Arms.stat_1, Arms.stat_1_value); }
        if (Chest != null) { AddBonusStats(Chest.stat_1, Chest.stat_1_value); }
        if (Legs != null) { AddBonusStats(Legs.stat_1, Legs.stat_1_value); }
        if (Boots != null) { AddBonusStats(Boots.stat_1, Boots.stat_1_value); }
        if (RightHand != null) { AddBonusStats(RightHand.stat_1, RightHand.stat_1_value); }
        if (LeftHand != null) { AddBonusStats(LeftHand.stat_1, LeftHand.stat_1_value); }
    }

    public void AddBonusStats(string stat, int value)
    {
        switch (stat) 
        {
            case "strength":
                bonusStrength += value;
                break;

            case "dexterity":
                bonusDexterity += value;
                break;

            case "vitality":
                bonusVitality += value;
                break;

            case "intelligence":
                bonusIntelligence += value;
                break;
        }
    }
}
