using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public PlayerInfo playerInfo;
    
    public int level;
    
    public float hp;
    public float mp;

    public float maxHp
    {
        get => playerInfo.maxHp + level * 10;
        private set => maxHp = value;
    }

    public float maxMp
    {
        get => playerInfo.maxMp + level * 10;
        private set => maxMp = value;
    }

    public float hpIncRate;
    public float mpIncRate;
    
    public int[] skillLevel;
    
    public int skillPoint;
    
    public float maxSpeed;
    public float accel;
    public float jumpPower;

    public float atk;
    public float def;

    public float stance;

    public int weapon;
}
