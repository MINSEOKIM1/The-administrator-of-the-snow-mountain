using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SceneInfo", menuName = "ScriptableObjects/VillageInfo", order = 1)]
public class VillageInfo : ScriptableObject
{
    // # Scriptable objects
    public int maxMob;
    public int curMob;
}
