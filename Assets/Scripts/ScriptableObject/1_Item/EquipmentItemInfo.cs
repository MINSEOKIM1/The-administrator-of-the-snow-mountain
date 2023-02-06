using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EquipmentItemInfo", menuName = "ScriptableObjects/EquipmentItemInfo", order = 1)]
public class EquipmentItemInfo : ItemInfo
{
    public static int StatCount = 7;
    public int equipmentPart;

    public float[] stats = new float[7];
    
    public float atk;
    public float def;
    public float hp;
    public float mp;
    public float hpIncRate;
    public float mpIncRate;
    public float stance;

    public float atkSpeed;

    public override int GetUseOption()
    {
        return Equipment;
    }
}
