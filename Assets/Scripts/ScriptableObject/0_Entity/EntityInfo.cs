using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityInfo", menuName = "ScriptableObjects/EntityInfo", order = 1)]
public class EntityInfo : ScriptableObject
{
    public float maxSpeed;
    public float accel;
    public float jumpPower;
    public Vector2 backStepPower;
    public float dashPower;
    public float rollPower;
    
    public float maxHp;
    public float maxMp;

    public float hpIncRate;
    public float mpIncRate;
    
    public float atk;
    public float def;

    public float stance; 
}
