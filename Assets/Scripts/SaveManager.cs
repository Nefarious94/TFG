using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

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
        SaveData data = new SaveData();

        data.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        Transform player = GameManager.Instance.PlayerController.transform;

        data.playerPosX = player.position.x;
        data.playerPosY = player.position.y;

        data.gold = GameManager.Instance.m_Gold;

        foreach (Item item in GameManager.Instance.inventoryPlayer.ItemsList)
        {
            data.inventoryItems.Add(item.itemName);
        }

        foreach (Character character in GameManager.Instance.GroupController.Party)
        {
            AllyControllerSaveData allyData = new AllyControllerSaveData();

            //allyData.level = character.level;
            allyData.maxHp = character.maxHp;
            //allyData.attack = character.attack;
            //allyData.defense = character.defense;

            data.group.Add(allyData);
        }

        // Equipment
        // TODO cuando hagas equipment

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(GetSavePath(slot), json);

        Debug.Log("GAME SAVED SLOT: " + slot);
    }

    public void LoadGame(int slot)
    {
        string path = GetSavePath(slot);

        if (!File.Exists(path))
        {
            Debug.Log("NO SAVE FILE");
            return;
        }

        string json = File.ReadAllText(path);

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        StartCoroutine(LoadGameCoroutine(data));

        Debug.Log("GAME LOADED SLOT: " + slot);
    }

    IEnumerator LoadGameCoroutine(SaveData data)
    {
        SceneManager.LoadScene(data.currentScene);

        yield return null;

        Transform player = GameManager.Instance.PlayerController.transform;

        player.position = new Vector3(
            data.playerPosX,
            data.playerPosY
        );

        GameManager.Instance.m_Gold = data.gold;

        GameManager.Instance.inventoryPlayer.ItemsList.Clear();

        foreach (string itemName in data.inventoryItems)
        {
            Item item = new Item();

            item.itemName = itemName;

            GameManager.Instance.inventoryPlayer.addItem(item);
        }

        foreach (AllyControllerSaveData allyData in data.group)
        {
            Character character = new Character();

            //character.level = allyData.level;
            character.maxHp = allyData.maxHp;
            //character.attack = allyData.attack;
            //character.defense = allyData.defense;

            GameManager.Instance.GroupController.Party.Add(character);
        }
    }

    public bool SaveExists(int slot)
    {
        return File.Exists(GetSavePath(slot));
    }

    public void DeleteSave(int slot)
    {
        string path = GetSavePath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);

            Debug.Log("SAVE DELETED SLOT: " + slot);
        }
    }
}