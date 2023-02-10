using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ConsumableItemInfo", menuName = "ScriptableObjects/ConsumableItemInfo", order = 1)]
public class ConsumableItemInfo : ItemInfo
{
    public float hpRecovery;
    public float mpRecovery;
    public float saturationRecovery;
    
    [Serializable]
    public class IngredientCountPair
    {
        public ItemInfo item;
        public int count;
    }

    public IngredientCountPair[] IngredientCountPairs;

    public override int GetUseOption()
    {
        return Consumable;
    }
}
