using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new();

    public void AddItem(Item item, int amount)
    {
        var existingItem = items.Find(i => i.ItemName == item.ItemName);
        if (existingItem != null)
        {
            existingItem.ItemCount += amount;
        }
        else
        {
            var newItem = ScriptableObject.CreateInstance<Item>();
            newItem.ItemName = item.ItemName;
            newItem.ItemDescription = item.ItemDescription;
            newItem.ItemCount = amount;
            newItem.ItemIcon = item.ItemIcon;
            newItem.ApplyEffect = item.ApplyEffect;
            items.Add(newItem);
        }
    }

    public void RemoveItem(Item item)
    {
        var existingItem = items.Find(i => i.ItemName == item.ItemName);
        if (existingItem != null)
        {
            existingItem.ItemCount -= item.ItemCount;
            if (existingItem.ItemCount <= 0)
                items.Remove(existingItem);
        }
    }

    public bool HasItem(Item item)
    {
        return items.Exists(i => i.ItemName == item.ItemName);
    }

    public int GetItemCount(Item item)
    {
        var existingItem = items.Find(i => i.ItemName == item.ItemName);
        return existingItem != null ? existingItem.ItemCount : 0;
    }

    public Item GetItem(string itemName)
    {
        return items.Find(i => i.ItemName == itemName);
    }
}
