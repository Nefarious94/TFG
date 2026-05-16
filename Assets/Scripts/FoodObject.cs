using Unity.VisualScripting;
using UnityEngine;

public class FoodObject : CellObject
{
    public string foodName;
    public string description;
    public int AmountGranted;
    public override void PlayerEntered()
    {
        Destroy(gameObject);
        Item item = new Item();
        item.itemName = foodName;
        item.itemType = "Consumable";
        item.itemSubType = "food";
        item.description = description;
        item.stat_1_value = AmountGranted;
        GameManager.Instance.inventoryPlayer.addItem(item);
    }
}
