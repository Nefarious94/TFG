using System.Collections;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

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

    string GetSavePath(int slot)
    {
        return Application.persistentDataPath
            + "/save_" + slot + ".json";
    }

    public void SaveGame(int slot)
    {
        SaveData data = CreateSaveData();

        string json =
            JsonUtility.ToJson(data, true);

        File.WriteAllText(
            GetSavePath(slot),
            json
        );

        Debug.Log("Game Saved");
    }

    public void LoadGame(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.Log("No save found");
            return;
        }

        string json =
            File.ReadAllText(path);

        SaveData data =
            JsonUtility.FromJson<SaveData>(json);

        GameManager.Instance.InitBase();

        StartCoroutine(
            DelayedLoad(data)
        );

        Debug.Log("Game Loaded");
    }
    IEnumerator DelayedLoad(SaveData data)
    {
        yield return null;
        yield return null;

        ApplySaveData(data);
    }

    SaveData CreateSaveData()
    {
        SaveData data = new SaveData();

        GameManager gm = GameManager.Instance;

        data.gold = gm.m_Gold;
        data.food = gm.m_FoodAmount;
        data.dungeonFloor = gm.dungeonFloor;

        // INVENTORY

        foreach (Item item in gm.inventoryPlayer.ItemsList)
        {
            data.inventory.Add(
                ConvertItem(item)
            );
        }

        // PARTY

        foreach (AllyController ally in gm.GroupController.Party)
        {
            SaveDataCharacter charData =
                new SaveDataCharacter();

            charData.characterClass =
                ally.m_class;

            charData.level = ally.level;
            charData.currentHP = ally.currentHP;
            charData.currentExp = ally.currentExp;

            charData.head =
                ConvertItem(ally.Head);

            charData.chest =
                ConvertItem(ally.Chest);

            charData.arms =
                ConvertItem(ally.Arms);

            charData.legs =
                ConvertItem(ally.Legs);

            charData.boots =
                ConvertItem(ally.Boots);

            charData.rightHand =
                ConvertItem(ally.RightHand);

            data.party.Add(charData);
        }

        return data;
    }

    SaveDataItem ConvertItem(Item item)
    {
        if (item == null)
            return null;

        return new SaveDataItem
        {
            itemName = item.itemName,

            itemType = (int)item.itemType,
            itemSubType = (int)item.itemSubType,

            armorType = (int)item.armorType,
            weaponType = (int)item.weaponType,

            stat1 = item.stat_1_value,
            stat2 = item.stat_2_value,

            description = item.description
        };
    }

    void ApplySaveData(SaveData data)
    {
        GameManager gm = GameManager.Instance;

        gm.m_Gold = data.gold;
        gm.m_FoodAmount = data.food;
        gm.dungeonFloor = data.dungeonFloor;

        // INVENTORY

        gm.inventoryPlayer.ItemsList.Clear();

        foreach (SaveDataItem itemData in data.inventory)
        {
            gm.inventoryPlayer.addItem(
                CreateItem(itemData)
            );
        }

        // PARTY

        foreach (AllyController ally in gm.GroupController.Party)
        {
            Destroy(ally.gameObject);
        }

        gm.GroupController.Party.Clear();

        foreach (SaveDataCharacter charData in data.party)
        {
            AllyController prefab =
                GetPrefabFromClass(charData.characterClass);

            AllyController ally =
                Instantiate(prefab);

            ally.gameObject.SetActive(false);

            ally.level = charData.level;
            ally.currentHP = charData.currentHP;
            ally.currentExp = charData.currentExp;

            ally.Head = CreateItem(charData.head);
            ally.Chest = CreateItem(charData.chest);
            ally.Arms = CreateItem(charData.arms);
            ally.Legs = CreateItem(charData.legs);
            ally.Boots = CreateItem(charData.boots);
            ally.RightHand = CreateItem(charData.rightHand);

            ally.CalculateStats();

            gm.GroupController.AddCharacter(ally);
        }

        if (UIManager.Instance != null)
        {
            if (UIManager.Instance.ability != null) UIManager.Instance.ability.InitAbilityUI();
            if (UIManager.Instance.equipment != null) UIManager.Instance.equipment.InitEquipmentUI();
        }
    }

    Item CreateItem(SaveDataItem data)
    {
        if (data == null)
            return null;

        Item item = new Item();

        item.itemName = data.itemName;

        item.itemType =
            (Item.ItemType)data.itemType;

        item.itemSubType =
            (Item.SubType)data.itemSubType;

        item.armorType =
            (Item.ArmorType)data.armorType;

        item.weaponType =
            (Item.WeaponType)data.weaponType;

        item.stat_1_value = data.stat1;
        item.stat_2_value = data.stat2;

        item.description = data.description;

        return item;
    }

    AllyController GetPrefabFromClass(Character.charClass charClass)
    {
        AllyController character = null;
        switch (charClass)
        {
            case Character.charClass.Archer:
                character = GameManager.Instance.PartyPrefabs[0];
                break;

            case Character.charClass.Berserker:
                character = GameManager.Instance.PartyPrefabs[1];
                break;

            case Character.charClass.BlackMage:
                character = GameManager.Instance.PartyPrefabs[2];
                break;

            case Character.charClass.Knight:
                character = GameManager.Instance.PartyPrefabs[3];
                break;

            case Character.charClass.Rogue:
                character = GameManager.Instance.PartyPrefabs[4];
                break;

            case Character.charClass.Warrior:
                character = GameManager.Instance.PartyPrefabs[5];
                break;

            case Character.charClass.WhiteMage:
                character = GameManager.Instance.PartyPrefabs[6];
                break;
        }
        return character;
    }
}