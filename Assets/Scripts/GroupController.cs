using UnityEngine;
using System.Collections.Generic;

public class GroupController : MonoBehaviour
{
    public List<Character> Party = new List<Character>();
    private int m_CurrentIndex = 0;
    public bool HasParty => Party.Count > 0;
    public bool groupMode = false;
    public int m_ActionsTaken = 0;

    public Character ActiveCharacter => Party[m_CurrentIndex];

    public void AddCharacter(Character character)
    {
        Party.Add(character);
    }

    public void NextCharacter()
    {
        if (Party.Count == 0) return;

        m_CurrentIndex = (m_CurrentIndex + 1) % Party.Count;
    }

    public void groupMove(Vector2Int dir)
    {
        if (!groupMode)
        {
            for (int i = 0; i < Party.Count; ++i)
            {
                
                if (Party[i] == ActiveCharacter)
                {
                    bool moved = ActiveCharacter.TryMove(ActiveCharacter.Cell + dir);
                    if (moved)
                    {
                        GameManager.Instance.TurnManager.Tick();
                    }                    
                }
                else
                {
                    Party[i].AutoMove();
                }
            }
        }
        else
        {
            bool moved = ActiveCharacter.TryMove(ActiveCharacter.Cell + dir);
            

            if (moved)
            {
                m_ActionsTaken++;

                if (m_ActionsTaken >= Party.Count)
                {
                    GameManager.Instance.TurnManager.Tick();
                    m_ActionsTaken = 0;
                }

                NextCharacter();
            }
        }
    }

    public void Death(Character character)
    {
        Party.Remove(character);
        if (Party.Count == 0)
        {
            //GameOver();
            Debug.Log("Game Over");
            Application.Quit();
        }
    }
}