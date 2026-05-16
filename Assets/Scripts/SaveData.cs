using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string currentScene;

    public float playerPosX;
    public float playerPosY;
    public int gold;
    public List<string> inventoryItems;
    public List<AllyControllerSaveData> group;
    //public List<string> equippedItems;

    public SaveData()
    {
        inventoryItems = new List<string>();
        group = new List<AllyControllerSaveData>();
        //equippedItems = new List<string>();
    }
}