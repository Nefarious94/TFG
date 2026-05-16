using UnityEngine;

public class Item
{
    public static int code = 0;
    public int itemID;
    public string itemName;
    public string itemType;
    public string itemSubType;
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
        if (itemSubType == "food")
        {
            GameManager.Instance.ChangeFood(stat_1_value);
        }
        if (itemSubType == "potion")
        {
            GameManager.Instance.PotionUse(stat_1_value);
        }
    }
}