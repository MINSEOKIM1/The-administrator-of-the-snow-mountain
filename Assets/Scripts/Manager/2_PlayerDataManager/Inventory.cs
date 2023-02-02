using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static int InventoryCapacity = 27;
    
    public List<ItemSlot> items;

    public ItemInfo test;

    public int usedSlotCount;

    public delegate void InventoryUIUpdate();

    public event InventoryUIUpdate InventoryUIUpdateEvent;


    private void Start()
    {
        items = new List<ItemSlot>(InventoryCapacity);
        
        for (int i = 0; i < InventoryCapacity; i++)
        {
            items.Add(null);
        }
        for (int i = 0; i < InventoryCapacity; i++)
        {
            Debug.Log("TEST");
            AddItem(test, 19);
        }
    }

    public void UpdateInventoryState()
    {
        CountUsedSlot();
    }

    private void CountUsedSlot()
    {
        usedSlotCount = 0;
        for (int i = 0; i < InventoryCapacity; i++)
        {
            if (items[i].count > 0) usedSlotCount++;
            else items[i] = null;
        }
    }

    public void TestAddItem(int num)
    {
        if (AddItem(test, num))
        {
            Debug.Log("Success!");
        }
        else
        {
            Debug.Log("Fail... because there is no space...");
        }
    }

    public void TestDeleteItem(int num)
    {
        if (DeleteItem(test, num))
        {
            Debug.Log("Success!");
        }
        else
        {
            Debug.Log("Fail... there is no items as you said");
        }
    }
    
    /// <summary>
    /// Add Item...
    /// </summary>
    /// <param name="item"> what item to add </param>
    /// <param name="count"> how many items </param>
    /// <returns> true if succeed, false if there is no space </returns>
    public bool AddItem(ItemInfo item, int count)
    {
        int leftSpace = 0;
        for (int i = 0; i < InventoryCapacity; i++)
        {
            if (items[i] == null) leftSpace += item.maxCount;
            else if (items[i].count == 0) leftSpace += item.maxCount;
            else if (items[i].item.itemNum == item.itemNum) leftSpace += item.maxCount - items[i].count;
        }

        if (leftSpace < count) return false;

        for (int j = 0; j < count; j++)
        {
            for (int i = 0; i < InventoryCapacity; i++)
            {
                if (items[i] == null || items[i].count == 0)
                {
                    items[i] = new ItemSlot(item, 1);
                    break;
                } else if (items[i].item.itemNum == item.itemNum && items[i].count < items[i].item.maxCount)
                {
                    items[i].count++;
                    break;
                }
            }
        }

        return true;
    }

    public bool DeleteItem(ItemInfo item, int count)
    {
        int itemCount = CountItem(item);

        if (itemCount < count) return false;
        else
        {
            for (int j = 0; j < count; j++)
            {
                for (int i = InventoryCapacity - 1; i >= 0; i--)
                {
                    if (items[i] != null && items[i].count > 0 && items[i].item.itemNum == item.itemNum)
                    {
                        items[i].count--;
                        break;
                    }
                }
            }

            return true;
        }
    }

    public int CountItem(ItemInfo item)
    {
        int count = 0;
        
        for (int i = 0; i < InventoryCapacity; i++)
        {
            if (items[i] == null) continue;
            if (items[i].count > 0 && items[i].item.itemNum == item.itemNum) count += items[i].count;
        }

        return count;
    }

    public void Swap(int index0, int index1)
    {
        (items[index0], items[index1]) = (items[index1], items[index0]);
    }
}
