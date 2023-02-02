using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ItemInfo", menuName = "ScriptableObjects/ItemInfo", order = 1)]
public class ItemInfo : ScriptableObject
{
    public int itemNum;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public int maxCount;
}
