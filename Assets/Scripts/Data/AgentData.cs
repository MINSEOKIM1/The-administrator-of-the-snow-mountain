using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AgentData
{
    public string agentName;
    public float timeElapsed, timeTakenToHunt;
    public Sprite agentImage;
    public float timeRate; // 맵에 따라 달라짐.
    public string currentMapName;
    public bool atVillage;
}
