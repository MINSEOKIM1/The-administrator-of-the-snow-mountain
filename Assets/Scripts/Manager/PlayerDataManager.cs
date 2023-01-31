using System;
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
        set => maxHp = value;
    }

    public float maxMp
    {
        get => playerInfo.maxMp + level * 10;
        set => maxMp = value;
    }

    public float hpIncRate
    {
        get => playerInfo.hpIncRate + level * 0.1f;
        private set => hpIncRate = value;
    }
    public float mpIncRate
    {
        get => playerInfo.mpIncRate + level * 0.1f;
        private set => mpIncRate = value;
    }
    
    public int[] attackSkillLevel;
    public int[] utilSkillLevel;

    public float maxSpeed;
    public float accel;
    public float jumpPower;

    public float atk;
    public float def;

    public float stance;

    public int weapon;

    public int money;

    private void Start()
    {
        hp = maxHp;
        mp = maxMp;
    }

    private void Update()
    {
        hp = Mathf.Clamp(hp, 0, maxHp);
        mp = Mathf.Clamp(mp, 0, maxMp);

        hp += hpIncRate * Time.deltaTime;
        mp += mpIncRate * Time.deltaTime;
    }
}
