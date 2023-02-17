using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonData
{
    public int id;
    public string name;
    public float time;
    public float respawnTime;
    public int maxMob;
    public int curMob;
    public int bgmIndex;
}
