using UnityEngine;

public class Item
{
    public enum ItemType { Consumible, Armor, Weapon }
    public enum SubType { Food, Potion, Head, Arm, Chest, Legs, Boots }
    public enum ArmorType { Heavy, Medium, Light }
    public enum WeaponType { Bow, Glove, Wand, Shield, Dagger, Sword, Cane }


    public static int code = 0;
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public SubType itemSubType;
    public ArmorType armorType;
    public WeaponType weaponType;
    public string description;

    public string stat_1;
    public int stat_1_value;

    public string stat_2;
    public int stat_2_value;

    public string stat_3;
    public int stat_3_value;

    public Item()
    {       
        while (GameManager.Instance.inventoryPlayer.getItem(code) != null)
        {
            code++;
            if (code == 100)
            {
                code = 0;
            }
        }
        itemID = code;
    }

    public void useItem()
    {
        if (itemSubType == SubType.Food)
        {
            GameManager.Instance.ChangeFood(stat_1_value);
        }
        if (itemSubType == SubType.Potion)
        {
            GameManager.Instance.PotionUse(this);
        }
    }
}