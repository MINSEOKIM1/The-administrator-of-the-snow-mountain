using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SceneInfo", menuName = "ScriptableObjects/SceneInfo", order = 1)]
public class SceneInfo : ScriptableObject
{
    // # scriptable objects
    public string beforeSceneName;
    public string currentSceneName;
    public DungeonInfo[] dungeons;
    public VillageInfo village;
}
