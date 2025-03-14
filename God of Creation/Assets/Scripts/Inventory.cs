using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new();

    public void AddItem(Item item, int amount)
    {
        var existingItem = items.Find(i => i.ItemName == item.ItemName);
        if (existingItem != null)
            existingItem.ItemCount += amount;
        else
            items.Add(CreateItem(item, amount));
    }

    public void RemoveItem(Item item)
    {
        var existingItem = items.Find(i => i.ItemName == item.ItemName);
        if (existingItem != null)
        {
            if (existingItem.ItemCount > 1)
                existingItem.ItemCount -= 1;
            else
                items.Remove(existingItem);
        }
    }

    public int GetItemCount(Item item) => items.Find(i => i.ItemName == item.ItemName)?.ItemCount ?? 0;

    public Item GetItem(string itemName) => items.Find(i => i.ItemName == itemName);

    private Item CreateItem(Item item, int amount)
    {
        var newItem = ScriptableObject.CreateInstance<Item>();
        newItem.ItemName = item.ItemName;
        newItem.ItemDescription = item.ItemDescription;
        newItem.ItemCount = amount;
        newItem.ItemIcon = item.ItemIcon;
        newItem.PictureID = item.PictureID;
        newItem.ApplyEffect = item.ApplyEffect;
        return newItem;
    }
}
