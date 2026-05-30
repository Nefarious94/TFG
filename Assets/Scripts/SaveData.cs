using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int gold;
    public int food;
    public int dungeonFloor;

    public List<SaveDataCharacter> party =
        new List<SaveDataCharacter>();

    public List<SaveDataItem> inventory =
        new List<SaveDataItem>();
}