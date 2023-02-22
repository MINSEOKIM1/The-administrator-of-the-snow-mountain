using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public EquipmentItemInfo[] items; 
    /*
     * 0 : hat
     * 1 : top
     * 2 : bottom
     * 3 : weapon
     */
    public int[] GameToDataPack()
    {
        int[] tmp = new int[items.Length];
        for (int i = 0; i < tmp.Length; i++)
        {
            if (items[i] != null) tmp[i] = items[i].itemNum;
            else tmp[i] = -1;
        }

        return tmp;
    }

    public void SetFromDataPack(int[] tmp)
    {
        for (int i = 0; i < tmp.Length; i++)
        {
            items[i] = (EquipmentItemInfo) GameManager.Instance.ScriptableObjectManager.GetWithIndex(tmp[i]);
        }
    }
}
