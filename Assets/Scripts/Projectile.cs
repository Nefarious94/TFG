using System;
using UnityEngine;
using System.Collections;


public class Projectile : MonoBehaviour
{
    public enum StatType
    {
        None,
        Strength,
        Dexterity,
        Intelligence,
        Vitality
    }

    private Vector2Int currentCell;
    private Vector2Int direction;

    private int range;
    private int damage;
    public StatType stat;

    private AllyController owner;

    public float moveSpeed = 0.02f;

    private Action onFinished;

    public void Init(
        Vector2Int startCell,
        Vector2Int dir,
        int projectileRange,
        int dmg,
        AllyController shooter,
        Action finishedCallback)
    {
        currentCell = startCell;
        direction = dir;

        range = projectileRange;
        damage = dmg;

        owner = shooter;

        onFinished = finishedCallback;

        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        BoardManager board = GameManager.Instance.BoardManager;

        for (int i = 0; i < range; i++)
        {
            currentCell += direction;

            var cell = board.GetCellData(currentCell);

            // pared
            if (cell == null || !cell.Passable)
            {
                Destroy(gameObject);
                FinishProjectile();
                yield break;
            }

            // mover visualmente
            Vector3 targetPos = board.CellToWorld(currentCell);

            while (Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed
                );

                yield return null;
            }

            // impacto
            if (cell.Occupant is EnemyController enemy)
            {
                int finaldamage = CalculateFinalDamage(damage, stat);
                enemy.TakeDamage(finaldamage, owner);
                Destroy(gameObject);
                FinishProjectile();
                yield break;
            }
        }
        FinishProjectile();
    }

    private int CalculateFinalDamage(int baseDamage, StatType stat)
    {
        int statValue = 0;

        if (owner != null)
        {
            switch (stat)
            {
                case StatType.Strength:
                    statValue = owner.strength;
                    break;
                case StatType.Dexterity:
                    statValue = owner.dexterity;
                    break;
                case StatType.Intelligence:
                    statValue = owner.intelligence;
                    break;
                case StatType.Vitality:
                    statValue = owner.vitality;
                    break;
                case StatType.None:
                default:
                    statValue = 0;
                    break;
            }
        }

        return baseDamage + statValue;
    }

    void FinishProjectile()
    {
        onFinished?.Invoke();

        Destroy(gameObject);
    }
}