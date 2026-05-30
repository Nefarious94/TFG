using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    public UIDialog uiDialog;

    private Action<int> m_CurrentCallback;

    Item auxItem;
    Ability auxAbility;
    private List<AllyController> m_ActualOpciones;
    private Action<AllyController> m_OrdenCallback;

    private List<Item> m_item;
    private Action <Item> m_Action;

    public int groupChoice;

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
    private void Start()
    {
        FindUI();
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
        FindUI();
    }

    void FindUI()
    {
        uiDialog = FindAnyObjectByType<UIDialog>();
    }

    public void InitGameDialog()
    {
        string text = " ";

        List<string> options = new List<string>()
        {
            "New Game",
            "LoadGame",
            "Exit"
        };

        m_CurrentCallback = OnInitGameChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void OnInitGameChoice(int index)
    {
        switch (index)
        {
            case 0:
                ChooseCharacterDialog();
                break;

            case 1:
                LoadDialog();
                break;

            case 2:
                Application.Quit();
                break;
        }
    }

    public void ChooseCharacterDialog()
    {
        string text = "What charactar do wou want as your first member";

        List<string> options = new List<string>()
        {
            "Archer",
            "Berserker",
            "Black Mage",
            "Knight",
            "Rogue",
            "Warrior",
            "White Mage"
        };

        m_CurrentCallback = OnChooseCharacterChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void OnChooseCharacterChoice(int index)
    {
        Item item = new Item();
        AllyController newChar;
        switch (index)
        {
            case 0:
                newChar = Instantiate(GameManager.Instance.PartyPrefabs[0]);
                GroupController.Instance.AddCharacter(newChar);
                GameManager.Instance.InitBase();
                break;

            case 1:
                newChar = Instantiate(GameManager.Instance.PartyPrefabs[1]);
                GroupController.Instance.AddCharacter(newChar);
                GameManager.Instance.InitBase();
                break;

            case 2:
                newChar = Instantiate(GameManager.Instance.PartyPrefabs[2]);
                GroupController.Instance.AddCharacter(newChar);
                GameManager.Instance.InitBase();
                break;

            case 3:
                newChar = Instantiate(GameManager.Instance.PartyPrefabs[3]);
                GroupController.Instance.AddCharacter(newChar);
                GameManager.Instance.InitBase();
                break;

            case 4:
                newChar = Instantiate(GameManager.Instance.PartyPrefabs[4]);
                GroupController.Instance.AddCharacter(newChar);
                GameManager.Instance.InitBase();
                break;

            case 5:
                newChar = Instantiate(GameManager.Instance.PartyPrefabs[5]);
                GroupController.Instance.AddCharacter(newChar);
                GameManager.Instance.InitBase();
                break;

            case 6:
                newChar = Instantiate(GameManager.Instance.PartyPrefabs[6]);
                GroupController.Instance.AddCharacter(newChar);
                GameManager.Instance.InitBase();
                break;
        }
    }

    public void DungeonEntranceDialog()
    {
        string text = "Do you want to enter the dungeon?";

        List<string> options = new List<string>()
        {
            "Yes",
            "No"
        };

        m_CurrentCallback = OnDungeonEntranceChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void OnDungeonEntranceChoice(int index)
    {
        switch (index)
        {
            case 0:

                GameManager.Instance.EnterDungeon();

                break;

            case 1:

                uiDialog.CloseDialog();

                break;
        }
    }

    public void ExitCellDialog()
    {
        string text = "What do you want to do?";

        List<string> options = new List<string>()
        {
            "Climb",
            "Stay"
        };

        if (GameManager.Instance.dungeonFloor % 5 == 0)
        {
            options.Add("Go to base");
        }

        m_CurrentCallback = ExitCellChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void ExitCellChoice(int index)
    {
        switch (index)
        {
            case 0:

                GameManager.Instance.NewLevel();

                break;

            case 1:

                uiDialog.CloseDialog();

                break;

            case 2:
                GameManager.Instance.EnterBase();
                break;

        }
    }

    public void ShopVendorDialog()
    {
        string text = "What do you want to do?";

        List<string> options = new List<string>()
        {
            "Buy",
            "Sell",
            "Cheat",
            "Nothing"
        };

        m_CurrentCallback = ShopVendorChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void ShopVendorChoice(int index)
    {
        switch (index)
        {
            case 0:
                ShopVendorBuyDialog();
                break;

            case 1:
                ShopVendorSellDialog();
                break;

            case 2:
                ShopVendorCheatDialog();
                break;

            case 3:
                uiDialog.CloseDialog();
                break;
        }
    }

    public void ShopVendorCheatDialog()
    {
        string text = "Are you sure about that? (1000 gold)";

        List<string> options = new List<string>()
        {
            "Yes, give me the money!",
            "No"
        };

        m_CurrentCallback = ShopVendorCheatChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void ShopVendorCheatChoice(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.Instance.m_Gold += 1000;
                UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                break;

            case 1:
                uiDialog.CloseDialog();
                break;
        }
    }

    public void ShopVendorSellDialog()
    {
        string text = "Which item do you want to sell?";
        List<string> options = new List<string>();
        m_item = new List<Item>();
        List<Item> inventory = GameManager.Instance.inventoryPlayer.ItemsList;

        if (inventory == null || inventory.Count == 0)
        {
            options.Add("Back (Inventory Empty)");
        }
        else
        {
            foreach (Item item in inventory)
            {
                if (item.itemType == Item.ItemType.Consumible)
                {
                    options.Add(item.itemName + " (3 Gold)");
                }
                else if (item.itemType == Item.ItemType.Armor || item.itemType == Item.ItemType.Weapon)
                {
                    options.Add(item.itemName + " (15 Gold)");
                }
                    m_item.Add(item);
            }
            options.Add("Cancel"); // Añadimos una opción para echarse atrás
        }

        m_CurrentCallback = ShopVendorSellChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void ShopVendorSellChoice(int index)
    {
        uiDialog.CloseDialog();

        if (index >= 0 && index < m_item.Count)
        {
            if (m_item[index].itemType == Item.ItemType.Consumible)
            {
                GameManager.Instance.m_Gold += 3;
            }
            else if (m_item[index].itemType == Item.ItemType.Armor || m_item[index].itemType == Item.ItemType.Weapon)
            {
                GameManager.Instance.m_Gold += 15;
            }
            UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
            GameManager.Instance.inventoryPlayer.removeItem(m_item[index]);
            ShopVendorSellDialog();
        }
        else 
        {
            uiDialog.CloseDialog();
        }
    }

    public void ShopVendorBuyDialog()
    {
        string text = "Which item do you want to buy?";

        List<string> options = new List<string>()
        {
            "Food (5 gold)",
            "HP Potion (5 gold)",
            "Mana Potion (5 gold)",
            "Armor (25 gold)" ,
            "Weapon (25 gold)",
            "Companion (50 gold)",
            "Nothing"
        };

        m_CurrentCallback = ShopVendorBuyChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void ShopVendorBuyChoice(int index)
    {
        switch (index)
        {
            case 0:
                if (GameManager.Instance.m_Gold >= 5)
                {
                    Item itemF = new Item();
                    itemF.itemName = "Blue Shroom";
                    itemF.itemType = Item.ItemType.Consumible;
                    itemF.itemSubType = Item.SubType.Food;
                    itemF.description = "Restores a tiny amount of energy";
                    itemF.stat_1_value = 5;
                    GameManager.Instance.inventoryPlayer.addItem(itemF);
                    GameManager.Instance.m_Gold -= 5;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                    ShopVendorBuyDialog();
                }
                break;

            case 1:
                if (GameManager.Instance.m_Gold >= 5)
                {
                    Item itemP = new Item();
                    itemP.itemName = "HP potion";
                    itemP.itemType = Item.ItemType.Consumible;
                    itemP.itemSubType = Item.SubType.Potion;
                    itemP.description = "Restores a small amount of HitPoints";
                    itemP.stat_1_value = 5;
                    GameManager.Instance.inventoryPlayer.addItem(itemP);
                    GameManager.Instance.m_Gold -= 5;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                    ShopVendorBuyDialog();
                }
                break;

            case 2:
                if (GameManager.Instance.m_Gold >= 5)
                {
                    Item itemP = new Item();
                    itemP.itemName = "Mana potion";
                    itemP.itemType = Item.ItemType.Consumible;
                    itemP.itemSubType = Item.SubType.Potion;
                    itemP.description = "Restores a small amount of Mana";
                    itemP.stat_1_value = 5;
                    GameManager.Instance.inventoryPlayer.addItem(itemP);
                    GameManager.Instance.m_Gold -= 5;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                    ShopVendorBuyDialog();
                }
                break;

            case 3:
                ShopVendorArmorDialog();
                break;

            case 4:
                ShopVendorWeaponDialog();
                break;

            case 5:
                ShopVendorCompanionDialog();
                break;

            case 6:
                uiDialog.CloseDialog();
                break;
        }
    }

    public void ShopVendorArmorDialog()
    {
        string text = "What do you want?";

        List<string> options = new List<string>()
        {
            "Light Head",
            "Medium Head",
            "Heavy Head",
            "Light Arm",
            "Medium Arm",
            "Heavy Arm",
            "Light Chest",
            "Medium Chest",
            "Heavy Chest",
            "Light Legs",
            "Medium Legs",
            "Heavy Legs",
            "Light Boots",
            "Medium Boots",
            "Heavy Boots",
            "Nothing"
        };

        m_CurrentCallback = ShopVendorArmorChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void ShopVendorArmorChoice(int index)
    {
        string[] light = { "dexterity", "intelligence" };
        string[] medium = { "strength", "dexterity" };
        string[] heavy = { "strength", "vitality" };
        switch (index)
        {
            case 0:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Head;
                    item.armorType = Item.ArmorType.Light;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = light[Random.Range(0, light.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 1:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Head;
                    item.armorType = Item.ArmorType.Medium;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = medium[Random.Range(0, medium.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 2:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Head;
                    item.armorType = Item.ArmorType.Heavy;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = heavy[Random.Range(0, heavy.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 3:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Arm;
                    item.armorType = Item.ArmorType.Light;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = light[Random.Range(0, light.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 4:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Arm;
                    item.armorType = Item.ArmorType.Medium;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = medium[Random.Range(0, medium.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 5:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Arm;
                    item.armorType = Item.ArmorType.Heavy;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = heavy[Random.Range(0, heavy.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 6:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Chest;
                    item.armorType = Item.ArmorType.Light;
                    item.stat_1 = light[Random.Range(0, light.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 7:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Chest;
                    item.armorType = Item.ArmorType.Medium;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = medium[Random.Range(0, medium.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 8:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Chest;
                    item.armorType = Item.ArmorType.Heavy;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = heavy[Random.Range(0, heavy.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 9:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Legs;
                    item.armorType = Item.ArmorType.Light;
                    item.stat_1 = light[Random.Range(0, light.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 10:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Legs;
                    item.armorType = Item.ArmorType.Medium;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = medium[Random.Range(0, medium.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 11:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Legs;
                    item.armorType = Item.ArmorType.Heavy;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = heavy[Random.Range(0, heavy.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;
            case 12:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Boots;
                    item.armorType = Item.ArmorType.Light;
                    item.stat_1 = light[Random.Range(0, light.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 13:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Boots;
                    item.armorType = Item.ArmorType.Medium;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = medium[Random.Range(0, medium.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 14:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Armor;
                    item.itemSubType = Item.SubType.Boots;
                    item.armorType = Item.ArmorType.Heavy;
                    item.itemName = item.armorType + " " + item.itemSubType;
                    item.stat_1 = heavy[Random.Range(0, heavy.Length)];
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 15:
                break;
        }
        ShopVendorBuyDialog();
    }

    public void ShopVendorWeaponDialog()
    {
        string text = "What do you want?";

        List<string> options = new List<string>()
        {
            "Bow",
            "Glove",
            "Wand" ,
            "Shield",
            "Dagger",
            "Sword",
            "Cane",
            "Nothing"
        };

        m_CurrentCallback = ShopVendorWeaponChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void ShopVendorWeaponChoice(int index)
    {
        switch (index)
        {
            case 0:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Weapon;
                    item.weaponType = Item.WeaponType.Bow;
                    item.itemName = item.weaponType.ToString();
                    item.stat_1 = "dexterity";
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 1:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Weapon;
                    item.weaponType = Item.WeaponType.Glove;
                    item.itemName = item.weaponType.ToString();
                    item.stat_1 = "strength";
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 2:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Weapon;
                    item.weaponType = Item.WeaponType.Wand;
                    item.itemName = item.weaponType.ToString();
                    item.stat_1 = "intelligence";
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 3:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Weapon;
                    item.weaponType = Item.WeaponType.Shield;                   
                    item.itemName = item.weaponType.ToString();
                    item.stat_1 = "vitality";
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 4:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Weapon;
                    item.weaponType = Item.WeaponType.Dagger;                    
                    item.itemName = item.weaponType.ToString();
                    item.stat_1 = "dexterity";
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 5:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Weapon;
                    item.weaponType = Item.WeaponType.Sword;
                    item.itemName = item.weaponType.ToString();
                    item.stat_1 = "strength";
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 6:
                if (GameManager.Instance.m_Gold >= 25)
                {
                    Item item = new Item();
                    item.itemType = Item.ItemType.Weapon;
                    item.weaponType = Item.WeaponType.Cane;
                    item.itemName = item.weaponType.ToString();
                    item.stat_1 = "intelligence";
                    item.stat_1_value = Random.Range(1, 6);
                    item.description = item.stat_1 + ": " + item.stat_1_value;
                    GameManager.Instance.inventoryPlayer.addItem(item);
                    GameManager.Instance.m_Gold -= 25;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 7:
                break;
        }
        ShopVendorBuyDialog();
    }

    public void ShopVendorCompanionDialog()
    {
        string text = "What do you want?";

        List<string> options = new List<string>()
        {
            "Archer",
            "Berserker",
            "BlackMage" ,
            "Knight",
            "Rogue",
            "Warrior",
            "WitheMage",
            "Nothing"
        };

        m_CurrentCallback = ShopVendorCompanionChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void ShopVendorCompanionChoice(int index)
    {
        AllyController newChar;
        switch (index)
        {
            case 0:
                if (GameManager.Instance.m_Gold >= 50)
                {
                    newChar = Instantiate(GameManager.Instance.PartyPrefabs[0]);
                    GroupController.Instance.AddCharacter(newChar);
                    newChar.gameObject.SetActive(false);
                }
                break;

            case 1:
                if (GameManager.Instance.m_Gold >= 50)
                {
                    newChar = Instantiate(GameManager.Instance.PartyPrefabs[1]);
                    GroupController.Instance.AddCharacter(newChar);
                    newChar.gameObject.SetActive(false);
                }
                break;

            case 2:
                if (GameManager.Instance.m_Gold >= 50)
                {
                    newChar = Instantiate(GameManager.Instance.PartyPrefabs[2]);
                    GroupController.Instance.AddCharacter(newChar);
                    newChar.gameObject.SetActive(false);
                }
                break;

            case 3:
                if (GameManager.Instance.m_Gold >= 50)
                {
                    newChar = Instantiate(GameManager.Instance.PartyPrefabs[3]);
                    GroupController.Instance.AddCharacter(newChar);
                    newChar.gameObject.SetActive(false);
                }                        
                break;

            case 4:
                if (GameManager.Instance.m_Gold >= 50)
                {
                    newChar = Instantiate(GameManager.Instance.PartyPrefabs[4]);
                    GroupController.Instance.AddCharacter(newChar);
                    newChar.gameObject.SetActive(false);
                }
                break;

            case 5:
                if (GameManager.Instance.m_Gold >= 50)
                {
                    newChar = Instantiate(GameManager.Instance.PartyPrefabs[5]);
                    GroupController.Instance.AddCharacter(newChar);
                    newChar.gameObject.SetActive(false);
                }
                break;

            case 6:
                if (GameManager.Instance.m_Gold >= 50)
                {
                    newChar = Instantiate(GameManager.Instance.PartyPrefabs[6]);
                    GroupController.Instance.AddCharacter(newChar);
                    newChar.gameObject.SetActive(false);
                }
                break;

            case 7:
                break;
        }
        if (GroupController.Instance.Party.Count >= 5)
        {
            DeleteAllyDialog();
        }
        else 
        {
            uiDialog.CloseDialog();
        }
        
    }

    public void DeleteAllyDialog()
    {
        m_ActualOpciones = GroupController.Instance.Party;

        string text = "The party is full. You have to release one member?";
        List<string> options = new List<string>();

        foreach (AllyController ally in m_ActualOpciones)
        {
            options.Add(ally.m_class.ToString());
        }

        m_CurrentCallback = DeleteAllyChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void DeleteAllyChoice(int index)
    {
        uiDialog.CloseDialog();

        if (index >= 0 && index < m_ActualOpciones.Count)
        {
            AllyController character = m_ActualOpciones[index];
            GroupController.Instance.Party.Remove(character);
            Destroy(character.gameObject);
        }
        if (GroupController.Instance.Party.Count > 4)
        {
            DeleteAllyDialog();
        }
    }

    public void InventoryDialog(int id)
    {
        auxItem = GameManager.Instance.inventoryPlayer.getItem(id);
        string text = "What do you want to do wit this item?";
        List<string> options;
        if (auxItem.itemType == Item.ItemType.Consumible && GroupController.Instance.ActiveCharacter.action)
        {
            options = new List<string>()
            {
                "Use",
                "Discard",
                "Nothing"
            };
        }
        else if (auxItem.itemType == Item.ItemType.Armor)
        {
            options = new List<string>()
            {
                "Equip",
                "Discard",
                "Nothing"
            };
        }
        else if (auxItem.itemType == Item.ItemType.Weapon)
        {
            options = new List<string>()
            {
                "Equip",
                "Discard",
                "Nothing"
            };
        }
        else
        {
            options = new List<string>()
            {
                "Discard",
                "Nothing"
            };
        }

        m_CurrentCallback = InventoryChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void InventoryChoice(int index)
    {
        bool equipped;
        if (auxItem.itemType == Item.ItemType.Consumible && GroupController.Instance.ActiveCharacter.action)
        {
            switch (index)
            {
                case 0:
                    auxItem.useItem();
                    GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                    UIManager.Instance.inventory.RefreshInventory();
                    GroupController.Instance.ActiveCharacter.action = false;
                    break;

                case 1:
                    GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                    UIManager.Instance.inventory.RefreshInventory();
                    break;

                case 2:
                    break;
            }
        }

        else if (auxItem.itemType == Item.ItemType.Armor)
        {            
            switch (index)
            {
                case 0:
                    equipped = GroupController.Instance.ActiveCharacter.EquipArmor(auxItem);
                    if (equipped)
                    {
                        GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                        UIManager.Instance.inventory.RefreshInventory();
                    }
                    break;

                case 1:
                    GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                    UIManager.Instance.inventory.RefreshInventory();
                    break;

                case 2:
                    break;
            }
        }

        else if (auxItem.itemType == Item.ItemType.Weapon)
        {
            switch (index)
            {
                case 0:
                    equipped = GroupController.Instance.ActiveCharacter.EquipWeapon(auxItem);
                    if (equipped)
                    {
                        GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                        UIManager.Instance.inventory.RefreshInventory();
                    }
                    break;

                case 1:
                    GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                    UIManager.Instance.inventory.RefreshInventory();
                    break;

                case 2:
                    break;
            }
        }

        else
        {
            switch (index)
            {
                case 0:
                    GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                    UIManager.Instance.inventory.RefreshInventory();
                    break;

                case 1:
                    break;
            }
        }
            uiDialog.CloseDialog();
    }

    public void AbilityDialog(Ability ability)
    {
        AllyController active = GroupController.Instance.ActiveCharacter;

        if (active.currentMana <= 0 || GameManager.Instance.CurrentMode != GameManager.GameMode.Dungeon)
        {
            uiDialog.CloseDialog();
            return;
        }

        auxAbility = ability;
        string text = "";
        List<string> options = new List<string>();

        if (auxAbility.type == Ability.AbilityType.Melee || auxAbility.type == Ability.AbilityType.Ranged)
        {
            text = "Which direction do you want to shoot in?";

            options = new List<string>()
            {
                "Up",
                "UpRight",
                "Right",
                "DownRight",
                "Down",
                "DownLeft",
                "Left",
                "UpLeft"
            };
        }
        else if (auxAbility.type == Ability.AbilityType.Healing || auxAbility.type == Ability.AbilityType.Buff)
        {
            text = "Select a target";
            
            // Pasamos el rango de la habilidad para filtrar los aliados cercanos
            m_ActualOpciones = active.GetNearbyAllies(ability.range);

            foreach (AllyController ally in m_ActualOpciones)
            {
                // Mostramos la clase del aliado y su vida actual para que el jugador sepa a quién curar
                options.Add(ally.m_class.ToString());
            }

            // Si no hay nadie en rango (raro, porque al menos estará el propio lanzador)
            if (options.Count == 0)
            {
                uiDialog.CloseDialog();
                return;
            }
        }

        
        m_CurrentCallback = AbilityChoice;


        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void AbilityChoice(int index)
    {
        AllyController ally = GroupController.Instance.ActiveCharacter;
        if (auxAbility.type == Ability.AbilityType.Melee || auxAbility.type == Ability.AbilityType.Ranged){
            switch (index)
            {
                case 0:
                    ally.UseAbility(auxAbility, Vector2Int.up);
                    break;

                case 1:
                    ally.UseAbility(auxAbility, new Vector2Int(1, 1));
                    break;

                case 2:
                    ally.UseAbility(auxAbility, Vector2Int.right);
                    break;

                case 3:
                    ally.UseAbility(auxAbility, new Vector2Int(1, -1));
                    break;

                case 4:
                    ally.UseAbility(auxAbility, Vector2Int.down);
                    break;

                case 5:
                    ally.UseAbility(auxAbility, new Vector2Int(-1, -1));
                    break;

                case 6:
                    ally.UseAbility(auxAbility, Vector2Int.left);
                    break;

                case 7:
                    ally.UseAbility(auxAbility, new Vector2Int(-1, 1));
                    break;
            }
        }
        else if (auxAbility.type == Ability.AbilityType.Healing || auxAbility.type == Ability.AbilityType.Buff)
        {
            // Verificamos que el índice sea válido y que existan aliados en la lista
            if (m_ActualOpciones != null && index >= 0 && index < m_ActualOpciones.Count)
            {
                AllyController targetAlly = m_ActualOpciones[index];

                // Llamamos a una nueva sobrecarga de UseAbility que acepta un objetivo directo
                ally.UseAbility(auxAbility, targetAlly);
            }
        }
        uiDialog.CloseDialog();
        UIManager.Instance.ability.CloseAbility();
        UIManager.Instance.pause.ClosePause();
    }

    public void EquipmentDialog()
    {
        string text = "Which direction do you want to shoot in?";
        List<string> options = new List<string>()
        {
            "Up",
            "Right",
            "Left",
            "Down"
        };

        m_CurrentCallback = EquipmentChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void EquipmentChoice(int index)
    {
        AllyController ally = GroupController.Instance.ActiveCharacter;
        switch (index)
        {
            case 0:
                ally.UseAbility(auxAbility, Vector2Int.up);
                break;

            case 1:
                ally.UseAbility(auxAbility, Vector2Int.right);
                break;

            case 2:
                ally.UseAbility(auxAbility, Vector2Int.left);
                break;

            case 3:
                ally.UseAbility(auxAbility, Vector2Int.down);
                break;
        }
        uiDialog.CloseDialog();
        UIManager.Instance.equipment.CloseEquipment();
        UIManager.Instance.pause.ClosePause();
    }

    public void NewOrderDialog(List<AllyController> auxneworder, Action<AllyController> election )
    {
        m_ActualOpciones = auxneworder;
        m_OrdenCallback = election;

        string text = "Which Character do you want next?";
        List<string> options = new List<string>();

        foreach (AllyController ally in auxneworder)
        {
            options.Add(ally.m_class.ToString());
        }

        m_CurrentCallback = NewOrderChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void NewOrderChoice(int index)
    {
        uiDialog.CloseDialog();

        if (index >= 0 && index < m_ActualOpciones.Count)
        {
            AllyController personajeElegido = m_ActualOpciones[index];

            m_OrdenCallback?.Invoke(personajeElegido);
        }
    }

    public void SaveDialog()
    {
        string text = "Choose save slot";
        List<string> options = new List<string>()
        {
            "Save Slot 1",
            "Save Slot 2",
            "Save Slot 3",
            "Cancel"
        };

        m_CurrentCallback = SaveChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void SaveChoice(int index)
    {
        uiDialog.CloseDialog();

        switch (index)
        {
            case 0:
                SaveManager.Instance.SaveGame(0);
                break;

            case 1:
                SaveManager.Instance.SaveGame(1);
                break;

            case 2:
                SaveManager.Instance.SaveGame(2);
                break;

            case 3:
                break;
        }
    }

    public void LoadDialog()
    {
        string text = "Choose save slot";

        List<string> options = new List<string>()
        {
            "Load Slot 1",
            "Load Slot 2",
            "Load Slot 3",
            "Cancel"
        };

        m_CurrentCallback = LoadChoice;

        uiDialog.OpenDialog(
            text,
            options,
            m_CurrentCallback
        );
    }

    void LoadChoice(int index)
    {
        switch (index)
        {
            case 0:
                SaveManager.Instance.LoadGame(0);
                break;

            case 1:
                SaveManager.Instance.LoadGame(1);
                break;

            case 2:
                SaveManager.Instance.LoadGame(2);
                break;

            case 3:
                if (GameManager.Instance.CurrentMode == GameManager.GameMode.Main)
                {
                    InitGameDialog();
                }
                else
                {
                    uiDialog.CloseDialog();
                }
                break;
        }
    }
}