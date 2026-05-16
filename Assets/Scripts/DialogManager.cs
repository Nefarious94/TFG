using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    public UIDialog uiDialog;

    private Action<int> m_CurrentCallback;

    Item auxItem;

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
        string text = "What do you want? (5 gold)";

        List<string> options = new List<string>()
        {
            "Food",
            "Potion",
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
                if (GameManager.Instance.m_Gold >= 5)
                {
                    Item itemF = new Item();
                    itemF.itemName = "Blue Shroom";
                    itemF.itemType = "Consumable";
                    itemF.itemSubType = "food";
                    itemF.description = "Restores a tiny amount of energy";
                    itemF.stat_1_value = 5;
                    GameManager.Instance.inventoryPlayer.addItem(itemF);
                    GameManager.Instance.m_Gold -= 5;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 1:
                if (GameManager.Instance.m_Gold >= 5)
                {
                    Item itemP = new Item();
                    itemP.itemName = "HP potion";
                    itemP.itemType = "Consumable";
                    itemP.itemSubType = "potion";
                    itemP.description = "Restores a small amount of HitPoints";
                    itemP.stat_1_value = 5;
                    GameManager.Instance.inventoryPlayer.addItem(itemP);
                    GameManager.Instance.m_Gold -= 5;
                    UIManager.Instance.gold.UpdateGold(GameManager.Instance.m_Gold);
                }
                break;

            case 2:
                break;
        }
        uiDialog.CloseDialog();
    }

    public void InventoryDialog(int id)
    {
        auxItem = GameManager.Instance.inventoryPlayer.getItem(id);
        string text = "What do you want to do wit this item?";
        List<string> options;
        if (auxItem.itemType == "Consumable")
        {
            options = new List<string>()
            {
                "Use",
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
        if (auxItem.itemType == "Consumable")
        {
            switch (index)
            {
                case 0:
                    auxItem.useItem();
                    GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                    UIManager.Instance.inventory.RefreshInventory();
                    break;

                case 1:
                    GameManager.Instance.inventoryPlayer.removeItem(auxItem);
                    UIManager.Instance.inventory.RefreshInventory();
                    break;

                case 2:
                    break;
            }
        }
        uiDialog.CloseDialog();
    }
}