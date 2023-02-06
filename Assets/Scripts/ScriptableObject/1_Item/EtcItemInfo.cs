using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "EtcItemInfo", menuName = "ScriptableObjects/EtcItemInfo", order = 1)]
public class EtcItemInfo : ItemInfo
{
    public override int GetUseOption()
    {
        return ETC;
    }
}
