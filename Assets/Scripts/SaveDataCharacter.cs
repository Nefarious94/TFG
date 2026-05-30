[System.Serializable]
public class SaveDataCharacter
{
    public Character.charClass characterClass;

    public int level;
    public int currentHP;
    public int currentExp;

    public SaveDataItem head;
    public SaveDataItem chest;
    public SaveDataItem arms;
    public SaveDataItem legs;
    public SaveDataItem boots;
    public SaveDataItem rightHand;
}