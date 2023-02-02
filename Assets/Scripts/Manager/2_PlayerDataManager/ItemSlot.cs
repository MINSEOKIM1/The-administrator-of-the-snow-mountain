using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSlot 
{
    public ItemInfo item;
    public int count;

    public ItemSlot(ItemInfo item, int count)
    {
        this.item = item;
        this.count = count;
    }
}
