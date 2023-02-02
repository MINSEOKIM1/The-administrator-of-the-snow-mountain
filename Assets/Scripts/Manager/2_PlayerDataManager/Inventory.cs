using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static int InventoryCapacity = 27;
    
    public List<ItemSlot> items;

    public ItemInfo test;
    

    private void Start()
    {
        items = new List<ItemSlot>(InventoryCapacity);
        
        for (int i = 0; i < items.Capacity; i++)
        {
            items.Add(null);
        }
        for (int i = 0; i < items.Capacity; i++)
        {
            Debug.Log("TEST");
            AddItem(test, 19);
        }
    }
    
    public void TestAddItem(int n) { AddItem(test, n);}

    public void TestDeleteItem(int n)
    {
        DeleteItem(test, n);}

    public void AddItem(ItemInfo item, int count)
    {
        for (int j = 0; j < count; j++)
        {
            var has = HasItem(item, 1);
            if (has == null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] == null || items[i].count <= 0)
                    {
                        items[i] = new ItemSlot(item, 1);
                        break;
                    }
                }
            }
            else
            {
                has.count += 1;
            }
        }
    }

    public bool DeleteItem(ItemInfo item, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var itemSlot = HasItem(item, 0);

            if (itemSlot == null || itemSlot.count == 0) return false;
            itemSlot.count--;
        }

        return true;
    }

    public ItemSlot HasItem(ItemInfo item, int count)
    {
        foreach (var i in items)
        {
            if (i == null) continue;
            if (i.item.itemNum == item.itemNum && i.count + count <= i.item.maxCount && i.count > 0) return i;
        }

        return null;
    }
}
