using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameMode
    {
        Base,
        Dungeon
    }
    public GameMode CurrentMode;

    public BaseManager BaseManager;
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public GroupController GroupController;
    public Inventory inventoryPlayer;
    public UIManager UIManager;

    public int m_FoodAmount = 100;
    public int m_Gold = 0;
    private int m_turn = 0;
    public int dungeonFloor = 0;

    public List<Ability> abilities;

    public TurnManager TurnManager { get; private set; }

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

    void Start()
    {
        InitBase();
    }

    public void EnterBase()
    {
        CurrentMode = GameMode.Base;
        SceneManager.LoadScene("BaseMap");
    }

    public void InitBase()
    {
        CurrentMode = GameMode.Base;
        SceneManager.LoadScene("BaseMap");
    }

    public void EnterDungeon()
    {
        CurrentMode = GameMode.Dungeon;
        SceneManager.LoadScene("DungeonMap");
    }

    public void InitDungeon()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;
        dungeonFloor = 0;

        NewLevel();
    }

    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Init(BoardManager.GroupController);
        dungeonFloor++;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        if (CurrentMode == GameMode.Base)
        {
            StartCoroutine(InitBaseDelayed());
            GroupController = FindFirstObjectByType<GroupController>();
        }
        else if (CurrentMode == GameMode.Dungeon)
        {
            BoardManager = FindFirstObjectByType<BoardManager>();
            GroupController = FindFirstObjectByType<GroupController>();
            PlayerController = FindFirstObjectByType<PlayerController>();

            if (BoardManager != null && PlayerController != null)
            {
                InitDungeon();
            }
            else
            {
                Debug.LogError("Dungeon managers missing");
            }
        }
    }

    IEnumerator InitBaseDelayed()
    {
        yield return null; // espera 1 frame

        if (BaseManager != null)
        {
            BaseManager.Init();
        }
        else
        {
            Debug.LogError("BaseManager STILL NULL");
        }
    }

    public void RegisterBaseManager(BaseManager bm)
    {
        BaseManager = bm;
        BaseManager.Init();
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
        /*
        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!\n\nYou traveled through " + m_CurrentLevel + " levels";
        }
        */
    }

    public void PotionUse(int amount)
    {
        GroupController.ActiveCharacter.HealPotion(amount);
    }

    public void OpenChest(ChestObject chest)
    {
        int roll = Random.Range(0, 100);
        if (roll < chest.rateMoney)
        {
            int gold = Random.Range(chest.minGold, chest.maxGold);
            m_Gold += gold;
            UIManager.Instance.gold.UpdateGold(m_Gold);
        }
        else if (roll < chest.rateMoney + chest.rateArmor)
        {
            Item item = new Item();
            item.itemName = "Armor";
            item.description = "Gives Resistences";
            inventoryPlayer.addItem(item);
        }
        else
        {
            Item item = new Item();
            item.itemName = "Weapon";
            item.description = "Does Damage";
            inventoryPlayer.addItem(item);
        }
    }
}
