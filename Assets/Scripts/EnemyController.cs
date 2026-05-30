using System.Drawing;
using UnityEngine;

public class EnemyController : Character
{
    public string enemyName;
    public int givenExp = 2;
    public CellObject itemEquipped;

    public override bool PlayerWantsToEnter()
    {
        TakeDamage(GameManager.Instance.GroupController.ActiveCharacter.attack, GameManager.Instance.GroupController.ActiveCharacter);
        GameManager.Instance.TurnManager.EndAllyTurn();
        if (GroupController.Instance.groupMode)
        {
            GroupController.Instance.NextCharacter();
        }
        return false;
    }

    public void TakeDamage(int amount, AllyController attacker)
    {
        int finalDamage = 0;
        int n = Random.Range(0, 100);

        if (n < attacker.rateCrit)
        {
            int baseDamage = Mathf.Max(1, amount - defense);
            finalDamage = Mathf.RoundToInt(baseDamage * 1.5f);
        }
        else
        {
            finalDamage = Mathf.Max(1, amount - defense);
        }

        currentHP -= finalDamage;

        if (currentHP <= 0)
        {
            var cell = GameManager.Instance.BoardManager.GetCellData(m_Cell);
            if (cell != null)
            {
                cell.Occupant = null;
            }
            Die(attacker);
                       
        }
    }

     protected virtual void Die(AllyController killer)
    {
        var board = GameManager.Instance.BoardManager;
        var currentCell = board.GetCellData(m_Cell);

        if (currentCell != null)
        {
            currentCell.Occupant = null;
            if (itemEquipped != null)
            {
                Vector2Int dropCell = m_Cell;
                if (currentCell.ContainedObject != null)
                {
                    dropCell = FindEmptyCell(m_Cell);
                    if (dropCell != m_Cell)
                    {
                        currentCell = board.GetCellData(dropCell);
                    }
                    else
                    {
                        Destroy(itemEquipped.gameObject);
                        itemEquipped = null;
                    }
                }
                if (itemEquipped != null)
                {
                    currentCell.ContainedObject = itemEquipped;
                    itemEquipped.gameObject.SetActive(true);
                    itemEquipped.transform.position = board.CellToWorld(dropCell);
                    itemEquipped.Init(dropCell);
                }
            }
        }
        GameManager.Instance.BoardManager.enemyList.Remove(this);
        Destroy(gameObject);
        killer.EnemyKill(this);
    }

    Vector2Int FindEmptyCell(Vector2Int currentCell)
    {
        var board = GameManager.Instance.BoardManager;
        BoardManager.CellData targetCell;

        targetCell = board.GetCellData(currentCell + Vector2Int.up);
        if (targetCell != null && targetCell.Passable && targetCell.ContainedObject == null)
        {
            return currentCell + Vector2Int.up;
        }
        targetCell = board.GetCellData(currentCell + Vector2Int.right);
        if (targetCell != null && targetCell.Passable && targetCell.ContainedObject == null)
        {
            return currentCell + Vector2Int.right;
        }
        targetCell = board.GetCellData(currentCell + Vector2Int.down);
        if (targetCell != null && targetCell.Passable && targetCell.ContainedObject == null)
        {
            return currentCell + Vector2Int.down;
        }
        if (targetCell != null && targetCell.Passable && targetCell.ContainedObject == null)
        {
            return currentCell + Vector2Int.left;
        }
        targetCell = board.GetCellData(currentCell + new Vector2Int(1, 1));
        if (targetCell != null && targetCell.Passable && targetCell.ContainedObject == null)
        {
            return currentCell + new Vector2Int(1, 1);
        }
        targetCell = board.GetCellData(currentCell + new Vector2Int(1, -1));
        if (targetCell != null && targetCell.Passable && targetCell.ContainedObject == null)
        {
            return currentCell + new Vector2Int(1, -1);
        }
        targetCell = board.GetCellData(currentCell + new Vector2Int(-1, -1));
        if (targetCell != null && targetCell.Passable && targetCell.ContainedObject == null)
        {
            return currentCell + new Vector2Int(-1, -1);
        }
        targetCell = board.GetCellData(currentCell + new Vector2Int(-1, 1));
        if (targetCell != null && targetCell.Passable && targetCell.ContainedObject == null)
        {
            return currentCell + new Vector2Int(-1, 1);
        }
        return currentCell;
    }

    /*
    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null
            || !targetCell.Passable
            || targetCell.ContainedObject != null)
        {
            return false;
        }

        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }
    */
    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null
            || !targetCell.Passable
            || targetCell.Occupant != null)
        {
            return false;
        }
        else if (targetCell.ContainedObject != null && this.itemEquipped == null) 
        {
            if (!(targetCell.ContainedObject is ExitCellObject))
            {
                itemEquipped = targetCell.ContainedObject;
                itemEquipped.gameObject.SetActive(false);
                targetCell.ContainedObject = null;
            }   
        }

        var currentCell = board.GetCellData(m_Cell);

        currentCell.Occupant = null;

        targetCell.Occupant = this;

        m_Cell = coord;

        transform.position = board.CellToWorld(coord);

        return true;
    }

    bool TryMoveAwayInX(int xDist)
    {
        // Si el jugador está a la derecha (xDist > 0), nos movemos a la izquierda, y viceversa
        if (xDist > 0) return MoveTo(m_Cell + Vector2Int.left);
        if (xDist < 0) return MoveTo(m_Cell + Vector2Int.right);
        return false;
    }

    bool TryMoveAwayInY(int yDist)
    {
        // Si el jugador está arriba (yDist > 0), nos movemos abajo, y viceversa
        if (yDist > 0) return MoveTo(m_Cell + Vector2Int.down);
        if (yDist < 0) return MoveTo(m_Cell + Vector2Int.up);
        return false;
    }

    bool TryFlee(int xDist, int yDist)
    {
        // Evaluamos si el jugador está más lejos horizontal o verticalmente para decidir la prioridad de escape
        int absX = Mathf.Abs(xDist);
        int absY = Mathf.Abs(yDist);

        // Intentamos movernos primero en la diagonal completamente opuesta al jugador
        Vector2Int diagonalFleeDir = new Vector2Int(xDist > 0 ? -1 : 1, yDist > 0 ? -1 : 1);
        if (MoveTo(m_Cell + diagonalFleeDir)) return true;

        // Si la diagonal falla, huimos priorizando el eje donde el jugador esté más encima
        if (absX > absY)
        {
            if (TryMoveAwayInX(xDist)) return true;
            if (TryMoveAwayInY(yDist)) return true;
        }
        else
        {
            if (TryMoveAwayInY(yDist)) return true;
            if (TryMoveAwayInX(xDist)) return true;
        }

        // Si todas las rutas ideales de huida están bloqueadas por obstáculos o paredes,
        // intentamos movernos a CUALQUIER otra dirección aleatoria libre que no sea acercarse al jugador.
        Vector2Int[] escapeDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        for (int i = 0; i < escapeDirections.Length; i++)
        {
            Vector2Int randomDir = escapeDirections[Random.Range(0, escapeDirections.Length)];

            // Calculamos si dar este paso nos aleja o mantiene igual la distancia con el jugador
            Vector2Int futureCell = m_Cell + randomDir;
            var playerCell = GameManager.Instance.GroupController.ActiveCharacter.Cell; // O el jugador objetivo
            int newDist = Mathf.Abs(playerCell.x - futureCell.x) + Mathf.Abs(playerCell.y - futureCell.y);
            int currentDist = absX + absY;

            if (newDist >= currentDist)
            {
                if (MoveTo(futureCell)) return true;
            }
        }

        return false; // No hay escapatoria posible
    }

    public void TurnHappened()
    {
        int xDist = int.MaxValue;
        int yDist = int.MaxValue;

        int absXDist = int.MaxValue;
        int absYDist = int.MaxValue;

        int auxXDist;
        int auxYDist;

        var group = GameManager.Instance.BoardManager.GroupController.Party;
        int minDistance = int.MaxValue;
        AllyController player = null;

        for (int i = 0; i < group.Count; i++)
        {
            if (group[i].isDead) { continue; }

            var playerCell = group[i].Cell;
            auxXDist = playerCell.x - m_Cell.x;
            auxYDist = playerCell.y - m_Cell.y;
            int dist = Mathf.Abs(auxXDist) + Mathf.Abs(auxYDist);
            if (minDistance > dist)
            {
                minDistance = dist;
                xDist = auxXDist;
                yDist = auxYDist;
                absXDist = Mathf.Abs(auxXDist);
                absYDist = Mathf.Abs(auxYDist);
                player = group[i];
            }
        }

        if (player == null)
        {
            return;
        }

        float hpPercentage = (float)currentHP / maxHP;

        if (enemyName == "Char_Sunflower" && hpPercentage <= 0.3f)
        {
            if (TryFlee(xDist, yDist))
            {
                GameManager.Instance.TurnManager.EndEnemyTurn();
                return;
            }
        }

        if (minDistance > 3)
        {
            /*
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
            */
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
            //if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
            if (absYDist <= 1 && absXDist <= 1)
            {
                if (this.name.Contains("Char_Snake"))
                {
                    AllyController mejorObjetivo = player; // Por defecto el que calculamos antes
                    int menosVida = player.currentHP;

                    // Buscamos si hay OTRO aliado que también esté adyacente y tenga menos vida
                    for (int i = 0; i < group.Count; i++)
                    {
                        if (group[i].isDead || group[i] == player) { continue; }

                        // Calculamos la distancia de este otro aliado al enemigo
                        int targetXDist = Mathf.Abs(group[i].Cell.x - m_Cell.x);
                        int targetYDist = Mathf.Abs(group[i].Cell.y - m_Cell.y);

                        // Si este aliado TAMBIÉN está adyacente (a distancia de ataque 1x1)
                        if (targetXDist <= 1 && targetYDist <= 1)
                        {
                            // Si tiene menos vida que el que íbamos a atacar, cambiamos de objetivo
                            if (group[i].currentHP < menosVida)
                            {
                                menosVida = group[i].currentHP;
                                mejorObjetivo = group[i];
                            }
                        }
                    }
                    // Asignamos el objetivo óptimo con menos vida que esté a nuestro lado
                    player = mejorObjetivo;
                }

                if (currentHP > 0)
                {
                    player.TakeDamage(attack, this);
                }
                GameManager.Instance.TurnManager.EndEnemyTurn();
                return;
            }
            else
            {
                /*
                if (absXDist > absYDist)
                {
                    if (!TryMoveInX(xDist))
                    {
                        //if our move was not successful (so no move and not attack)
                        //we try to move along Y
                        TryMoveInY(yDist);
                    }
                }
                else
                {
                    if (!TryMoveInY(yDist))
                    {
                        TryMoveInX(xDist);
                    }
                }*/
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
        }
        GameManager.Instance.TurnManager.EndEnemyTurn();
    }
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
}
