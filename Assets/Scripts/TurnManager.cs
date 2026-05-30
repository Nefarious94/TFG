using Unity.VisualScripting;
using UnityEngine;

public class TurnManager
{
    private int m_TurnCount;
    public AllyController currentCharacter;
    public EnemyController currentEnemy;
    public int m_TurnIndex;
    public bool ProjectileActive { get; private set; }

    public TurnManager()
    {
        m_TurnCount = 0;
    }

    public void StartLevel()
    {
        m_TurnCount++;
        m_TurnIndex = 0;
        currentCharacter = null;
        currentEnemy = null;
        StartAllyTurn();
    }

    void StartAllyTurn()
    {
        GroupController group = GroupController.Instance;

        if (group.Party[m_TurnIndex].isDead)
        {
            EndAllyTurn();
            return;
        }

        currentCharacter = group.Party[m_TurnIndex];
        if (currentCharacter == group.ActiveCharacter)
        {
            group.ActiveCharacter.action = true;
        }
        if (!group.groupMode && group.Party[m_TurnIndex] != group.ActiveCharacter)
        {
            group.Party[m_TurnIndex].AutoMove();
        }
    }

    public void EndAllyTurn()
    {
        if (m_TurnIndex < GroupController.Instance.Party.Count - 1)
        {
            m_TurnIndex++;
            StartAllyTurn();
        }
        else
        {
            m_TurnIndex = 0;
            StartEnemyTurn();
        }
    }

    void StartEnemyTurn()
    {
        var enemies = GameManager.Instance.BoardManager.enemyList;

        if (enemies == null || enemies.Count == 0)
        {
            m_TurnIndex = 0;
            m_TurnCount++;
            StartAllyTurn();
            return;
        }

        currentEnemy = enemies[m_TurnIndex];
        currentEnemy.TurnHappened();
    }

    public void EndEnemyTurn()
    {
        if (m_TurnIndex < GameManager.Instance.BoardManager.enemyList.Count - 1)
        {
            m_TurnIndex++;
            StartEnemyTurn();
        }
        else
        {
            m_TurnIndex = 0;
            m_TurnCount++;
            GameManager.Instance.OnTurnHappen();
            StartAllyTurn();
        }
    }

    public void StartProjectile()
    {
        ProjectileActive = true;
    }

    public void EndProjectile()
    {
        ProjectileActive = false;
    }
}
