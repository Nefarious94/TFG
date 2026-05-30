using Unity.VisualScripting;
using UnityEngine;

public class FoodObject : CellObject
{
    public string foodName;
    public string description;
    public int AmountGranted;
    public override void PlayerEntered()
    {
        var cell =
        GameManager.Instance.BoardManager.GetCellData(m_Cell);
        cell.ContainedObject = null;
        Destroy(gameObject);
        Item item = new Item();
        item.itemName = foodName;
        item.itemType = Item.ItemType.Consumible;
        item.itemSubType = Item.SubType.Food;
        item.description = description;
        item.stat_1_value = AmountGranted;
        GameManager.Instance.inventoryPlayer.addItem(item);
    }
}
