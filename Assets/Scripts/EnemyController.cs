using System.Drawing;
using UnityEngine;

public class EnemyController : CellObject
{
    public int Health = 3;
    public int attack = 1;

    private int m_CurrentHealth;
    public Vector2Int Cell => m_Cell;

    private void Awake()
    {
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
    }

    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= TurnHappened;
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = Health;
    }

    public override bool PlayerWantsToEnter()
    {
        m_CurrentHealth -= 1;

        if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }

        if (!GameManager.Instance.BoardManager.GroupController.groupMode)
        {
            GameManager.Instance.TurnManager.Tick();
        }
        else
        {
            GameManager.Instance.BoardManager.GroupController.m_ActionsTaken++;

            if (GameManager.Instance.BoardManager.GroupController.m_ActionsTaken >= GameManager.Instance.BoardManager.GroupController.Party.Count)
            {
                GameManager.Instance.TurnManager.Tick();
                GameManager.Instance.BoardManager.GroupController.m_ActionsTaken = 0;
            }

            GameManager.Instance.BoardManager.GroupController.NextCharacter();
        }
        return false;
    }

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

        //remove enemy from current cell
        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        //add it to the next cell
        targetCell.ContainedObject = this;
        m_Cell = coord;
        transform.position = board.CellToWorld(coord);

        return true;
    }

    void TurnHappened()
    {
        int xDist = int.MaxValue;
        int yDist = int.MaxValue;

        int absXDist = int.MaxValue;
        int absYDist = int.MaxValue;

        int auxXDist;
        int auxYDist;

        var group = GameManager.Instance.BoardManager.GroupController.Party;
        int minDistance = int.MaxValue;
        Character player = null;

        for (int i = 0; i < group.Count; i++)
        {
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

        if (minDistance > 3)
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
                player.TakeDamage(attack);
                return;
            }
            else
            {
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

    public void TakeDamage()
    {
        m_CurrentHealth -= 1;

        if (m_CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
