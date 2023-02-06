using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ItemInfo", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    public const int Equipment = 0, Consumable = 1, ETC = 2;
    public int useOption;
    public int itemNum;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public int maxCount;

    public virtual int GetUseOption()
    {
        return ETC;
    }
}
