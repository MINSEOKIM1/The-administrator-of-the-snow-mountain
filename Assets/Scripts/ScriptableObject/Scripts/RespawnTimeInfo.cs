using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SceneInfo", menuName = "ScriptableObjects/RespawnTimeInfo", order = 1)]
public class RespawnTimeInfo : ScriptableObject
{
    public float[] respawnTime;
}
