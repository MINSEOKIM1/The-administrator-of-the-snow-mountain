using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectManager : MonoBehaviour
{
    public ItemInfo[] objects;

    public ItemInfo GetWithIndex(int index)
    {
        if (index == -1) return null;
        foreach (var i in objects)
        {
            if (i.itemNum == index)
            {
                return i;
            }
        }

        return null;
    }
}
