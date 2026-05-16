using Unity.VisualScripting;
using UnityEngine;

public class PotionObject : CellObject
{
    public string potionName;
    public string description;
    public int HPGranted;
    public override void PlayerEntered()
    {
        Destroy(gameObject);
        Item item = new Item();
        item.itemName = potionName;
        item.itemType = "Consumable";
        item.itemSubType = "potion";
        item.description = description;
        item.stat_1_value = HPGranted;
        GameManager.Instance.inventoryPlayer.addItem(item);
    }
}
