using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerController PlayerController;
    private int m_FoodAmount = 100;
    private int m_Gold = 0;
    private int m_turn = 0;

    public TurnManager TurnManager { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        NewLevel();
    }

    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Init(BoardManager.GroupController);
    }

    void OnTurnHappen()
    {
        m_turn++;
        if(m_turn == 10)
        {
            m_FoodAmount--;
            Debug.Log("Food Amount : " + m_FoodAmount);
            m_turn = 0;
        }
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        if (m_FoodAmount > 100)
        {
            m_FoodAmount = 100;
            m_turn = 0;
        }
        //m_FoodLabel.text = "Food : " + m_FoodAmount;
        /*
        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!\n\nYou traveled through " + m_CurrentLevel + " levels";
        }
        */
    }

    public void OpenChest(ChestObject chest)
    {
        int roll = Random.Range(0, 100);
        if (roll < chest.rateMoney)
        {
            int gold = Random.Range(chest.minGold, chest.maxGold);
            m_Gold += gold;
            Debug.Log("Gold obtained: " + gold);
        }
        else if (roll < chest.rateMoney + chest.rateArmor)
        {
            Debug.Log("Armor obtained: ");
        }
        else
        {
            Debug.Log("Weapon obtained: ");
        }
    }
}
