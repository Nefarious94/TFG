using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Item;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameMode
    {
        Main,
        Base,
        Dungeon
    }
    public GameMode CurrentMode;

    public BaseManager BaseManager;
    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public GroupController GroupController;
    public AllyController[] PartyPrefabs;
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
        CurrentMode = GameMode.Main;
        DialogManager.Instance.InitGameDialog();
    }

    public void EnterMain()
    {
        m_Gold = 0;
        inventoryPlayer.ClearInventory();
        foreach (AllyController ally in GroupController.Party)
        {
            if (ally != null)
            {
                Destroy(ally.gameObject);
            }
        }
        GroupController.Party.Clear();
        CurrentMode = GameMode.Main;
        SceneManager.LoadScene("Main");
    }

    public void EnterBase()
    {
        CurrentMode = GameMode.Base;
        SceneManager.LoadScene("BaseMap");
        if (GroupController.Instance != null)
        {
            foreach (AllyController ally in GroupController.Instance.Party)
            {
                ally.gameObject.SetActive(false);
            }
        }
        GroupController.m_CurrentIndex = 0;
    }

    public void InitBase()
    {
        CurrentMode = GameMode.Base;
        SceneManager.LoadScene("BaseMap");
        if (GroupController.Instance != null)
        {
            foreach (AllyController ally in GroupController.Instance.Party)
            {
                ally.gameObject.SetActive(false);
            }
        }
        GroupController.Instance.m_CurrentIndex = 0;
    }

    public void EnterDungeon()
    {
        CurrentMode = GameMode.Dungeon;
        SceneManager.LoadScene("DungeonMap");
        if (GroupController.Instance != null)
        {
            foreach (AllyController ally in GroupController.Instance.Party)
            {
                ally.gameObject.SetActive(false);
                ally.currentMana = ally.maxMana;
            }
        }
        GroupController.Instance.m_CurrentIndex = 0;
    }

    public void InitDungeon()
    {
        TurnManager = new TurnManager();
        dungeonFloor = 0;
        m_FoodAmount = 100;
        foreach (AllyController ally in GroupController.Party)
        {
            ally.isDead = false;
            ally.currentHP = ally.maxHP;
            ally.currentMana = ally.maxMana;
        }
        NewLevel();
    }

    public void NewLevel()
    {
        dungeonFloor++;
        StartCoroutine(GenerateLevelRoutine());
    }

    // Usamos una corrutina para asegurar que la limpieza y la creación ocurran en frames separados
    private IEnumerator GenerateLevelRoutine()
    {
        // 1. Forzar a que GroupController apunte a la instancia persistente real del Singletón
        if (GroupController.Instance != null)
        {
            GroupController = GroupController.Instance;
        }

        // 2. Limpiamos por completo el Tilemap, enemigos y objetos del piso anterior
        BoardManager.Clean();

        // 3. Esperamos un frame para que Unity procese las destrucciones de los enemigos/objetos viejos
        yield return new WaitForEndOfFrame();

        // 4. Inicializamos el nuevo tablero (Crea salas, pasillos, muros y llama a RepositionParty)
        yield return StartCoroutine(BoardManager.Init());

        // 5. Vinculamos al jugador con los aliados actuales y arrancamos los turnos
        PlayerController.Init(GroupController);
        TurnManager.StartLevel();
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
            GroupController = GroupController.Instance;
        }
        else if (CurrentMode == GameMode.Dungeon)
        {
            BoardManager = FindFirstObjectByType<BoardManager>();
            GroupController = GroupController.Instance;
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
        else if (CurrentMode == GameMode.Main)
        {
            StartCoroutine(InitMainDelayed());
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
        while (GroupController.Instance == null)
        {
            yield return null;
        }
    }

    IEnumerator InitMainDelayed()
    {
        yield return null;

        DialogManager.Instance.InitGameDialog();
    }

    public void RegisterBaseManager(BaseManager bm)
    {
        BaseManager = bm;
        BaseManager.Init();
    }

    public void OnTurnHappen()
    {
        m_turn++;
        if(m_turn == 10)
        {
            m_FoodAmount--;
            Debug.Log("Food Amount : " + m_FoodAmount);
            m_turn = 0;
            if (m_FoodAmount <= 0)
            {
                EnterBase();
            }
        }
        foreach (AllyController ally in GroupController.Instance.Party)
        {
            if (ally.isBuffed)
            {
                ally.buffTurns--;
                if (ally.buffTurns <= 0)
                {
                    ally.isBuffed = false;
                }
            }
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

    public void PotionUse(Item item)
    {
        GroupController.ActiveCharacter.UsePotion(item);
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
            string[] light = { "dexterity", "intelligence" };
            string[] medium = { "strength", "dexterity" };
            string[] heavy = { "strength", "vitality" };
            Item item = new Item();
            item.itemType = ItemType.Armor;
            item.itemSubType = (Item.SubType)UnityEngine.Random.Range(2, 7);
            item.armorType = (Item.ArmorType)UnityEngine.Random.Range(0, 3);
            switch (item.armorType)
            {
                case ArmorType.Heavy:
                    item.stat_1 = heavy[Random.Range(0, heavy.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    break;

                case ArmorType.Medium:
                    item.stat_1 = heavy[Random.Range(0, heavy.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    break;

                case ArmorType.Light:
                    item.stat_1 = light[Random.Range(0, light.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    break;
            }
            item.itemName = item.armorType + " " + item.itemSubType;
            item.description = item.stat_1 + ": " + item.stat_1_value;
            inventoryPlayer.addItem(item);
        }
        else
        {
            Item item = new Item();
            item.itemType = ItemType.Weapon;
            item.weaponType = (Item.WeaponType)UnityEngine.Random.Range(0, 7);
            switch (item.weaponType)
            {
                case WeaponType.Bow:
                    item.stat_1 = "dexterity";
                    item.stat_1_value = Random.Range(1, 6);
                    break;

                case WeaponType.Glove:
                    item.stat_1 = "strength";
                    item.stat_1_value = Random.Range(1, 6);
                    break;

                case WeaponType.Wand:
                    item.stat_1 = "intelligence";
                    item.stat_1_value = Random.Range(1, 6);
                    break;

                case WeaponType.Shield:
                    item.stat_1 = "vitality";
                    item.stat_1_value = Random.Range(1, 6);
                    break;

                case WeaponType.Dagger:
                    item.stat_1 = "dexterity";
                    item.stat_1_value = Random.Range(1, 6);
                    break;

                case WeaponType.Sword:
                    item.stat_1 = "strength";
                    item.stat_1_value = Random.Range(1, 6);
                    break;

                case WeaponType.Cane:
                    item.stat_1 = "intelligence";
                    item.stat_1_value = Random.Range(1, 6);
                    break;
            }
            item.itemName = item.weaponType.ToString();
            item.description = item.stat_1 + ": " + item.stat_1_value;
            inventoryPlayer.addItem(item);
        }
    }
}
