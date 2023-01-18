using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SceneInfo", menuName = "ScriptableObjects/DungeonInfo", order = 1)]
public class DungeonInfo : ScriptableObject
{
    // # Scriptable objects
    public int maxMob;
    public int curMob;
}
