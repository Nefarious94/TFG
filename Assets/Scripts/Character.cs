using System.Text;
using UnityEngine;

public class Character : CellObject
{
    protected BoardManager m_Board;

    private bool m_IsGameOver;
    public Vector2Int Cell => m_Cell;
    public int m_CurrentHP;
    public int maxHp = 10;
    private bool initiated = false;

    private CellObject lastCell;

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_Board = GameManager.Instance.BoardManager;
        if (!initiated)
        {
            m_CurrentHP = maxHp;
            initiated = true;
        }       
    }

    public bool TryMove(Vector2Int target)
    {
        var cellData = m_Board.GetCellData(target);

        if (cellData == null || !cellData.Passable)
            return false;

        if (cellData.ContainedObject != null)
        {
            if (!cellData.ContainedObject.PlayerWantsToEnter())
                return false;
        }

        return MoveTo(target);
    }

    public bool MoveTo(Vector2Int target)
    {
        var targetCell = m_Board.GetCellData(target);

        if (targetCell.Passable)
        {
            var currentCell = m_Board.GetCellData(m_Cell);
            if (lastCell is ExitCellObject lastExit)
            {
                currentCell.ContainedObject = lastExit.ResetExit();
            }
            else
            {
                currentCell.ContainedObject = null;
            }
            m_Cell = target;
            GroupController group = GameManager.Instance.BoardManager.GroupController;

            if (targetCell.ContainedObject is ExitCellObject exit)
            {
                transform.position = m_Board.CellToWorld(target);
                exit.PlayerEntered();
                lastCell = exit;
                return true;
            }
            else if (targetCell.ContainedObject != null)
            {
                if (targetCell.ContainedObject is Character)
                {
                    return false;
                }
                targetCell.ContainedObject.PlayerEntered();
                lastCell = null;
            }
            else
            {
                lastCell = null;
            }

            targetCell.ContainedObject = this;

            transform.position = m_Board.CellToWorld(target);

            return true;
        }
        else
        {
            return false;
        }
    }

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

            if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
            {
                enemy.TakeDamage();
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
            return;
        }

        var player = GameManager.Instance.BoardManager.GroupController.ActiveCharacter;
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
                //wait
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

    private void SwapPositions(Character other)
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

    public void TakeDamage(int amount)
    {
        m_CurrentHP -= amount;
        Debug.Log("Damage received: " + amount + ", HP left: " + m_CurrentHP);
        GroupController group = GameManager.Instance.BoardManager.GroupController;
        if (m_CurrentHP <= 0)
        {
            var cell = m_Board.GetCellData(m_Cell);
            cell.ContainedObject = null;
            Destroy(gameObject);
            Debug.Log(name + " died");
            group.Death(this);
            if (this == group.ActiveCharacter)
            {
                group.NextCharacter();
            }
        }
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

    public void HealPotion(int amount)
    {
        m_CurrentHP += amount;
        if (m_CurrentHP > maxHp)
        {
            m_CurrentHP = maxHp;
        }
    }
}