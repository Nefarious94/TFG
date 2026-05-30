using UnityEngine;
using System.Collections.Generic;

public class GroupController : MonoBehaviour
{
    public static GroupController Instance { get; private set; }

    public List<AllyController> Party = new List<AllyController>();
    public int m_CurrentIndex = 0;
    public bool HasParty => Party.Count > 0;
    public bool groupMode = false;
    public int m_ActionsTaken = 0;

    private List<AllyController> NewOrder;
    private List<AllyController> NewOrderRemaining;

    public AllyController ActiveCharacter => Party[m_CurrentIndex];

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void AddCharacter(AllyController character)
    {
        Party.Add(character);
        DontDestroyOnLoad(character.gameObject);
    }

    public void NextCharacter()
    {
        if (Party.Count == 0) return;

        int attempts = 0;

        do
        {
            m_CurrentIndex = (m_CurrentIndex + 1) % Party.Count;
            attempts++;
        }
        while (attempts < Party.Count && Party[m_CurrentIndex].isDead);
    }

    public void groupMove(Vector2Int dir)
    {
        TurnManager turn = GameManager.Instance.TurnManager;

        if (!groupMode)
        {
            if (Party[turn.m_TurnIndex] == ActiveCharacter)
            {
                bool moved = ActiveCharacter.TryMove(ActiveCharacter.Cell + dir);
                if (moved)
                {
                    turn.EndAllyTurn();
                }
            }
        }
        else
        {
            if (ActiveCharacter.isDead)
            {
                GameManager.Instance.TurnManager.EndAllyTurn();
                return;
            }
            bool moved = ActiveCharacter.TryMove(ActiveCharacter.Cell + dir);
            if (moved)
            {
                turn.EndAllyTurn();
                NextCharacter();
            }
        }
    }

    public void Death(AllyController character)
    {
        if (Party.Count == 1)
        {
            GameManager.Instance.EnterBase();
        }
        else
        {
            int count = 0;
            foreach (AllyController ally in Party)
            {
                if (!ally.isDead)
                {
                    count++;
                }
            }
            if (count == 0)
            {
                GameManager.Instance.EnterBase();
            }
        }
    }

    public void StartChangeOrder()
    {
        NewOrder = new List<AllyController>();
        NewOrderRemaining = new List<AllyController>(Party);
        AskForNextCharacter();
    }

    private void AskForNextCharacter()
    {
        if (NewOrderRemaining.Count == 1)
        {
            NewOrder.Add(NewOrderRemaining[0]);
            Party = new List<AllyController>(NewOrder);
            return;
        }
        DialogManager.Instance.NewOrderDialog(NewOrderRemaining, Election);
    }

    private void Election(AllyController elegido)
    {
        NewOrder.Add(elegido);
        NewOrderRemaining.Remove(elegido);
        AskForNextCharacter();
    }
}