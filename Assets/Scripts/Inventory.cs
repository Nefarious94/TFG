using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> ItemsList = new List<Item> ();

    public void addItem(Item item)
    {
        ItemsList.Add (item);
    }

    public void removeItem(Item item)
    {
        ItemsList.Remove(item);
    }

    public Item getItem(int id)
    {
        foreach (Item item in ItemsList)
        {
            if (id == item.itemID)
            {
                return item;
            }
        }
        return null;
    }
}
