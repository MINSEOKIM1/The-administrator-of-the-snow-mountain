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
    public float saturation;

    public float attackSpeed
    {
        get
        {
            if (equipment.items[3] != null)
            {
                return equipment.items[3].atkSpeed;
            }
            else
            {
                return 0;
            }
        }

        set => attackSpeed = value;
    }

    public float maxHp
    {
        get
        {
            float total = 0;
            for (int i = 0; i < equipment.items.Length; i++)
            {
                if (equipment.items[i] != null) total += equipment.items[i].hp;
            }
            return playerInfo.maxHp + level * 10 + total;
        }
        set => maxHp = value;
    }

    public float maxMp
    {
        get
        {
            float total = 0;
            for (int i = 0; i < equipment.items.Length; i++)
            {
                if (equipment.items[i] != null) total += equipment.items[i].mp;
            }
            return playerInfo.maxMp + level * 10 + total;
        }
        set => maxMp = value;
    }

    public float hpIncRate
    {
        get
        {
            float total = 0;
            for (int i = 0; i < equipment.items.Length; i++)
            {
                if (equipment.items[i] != null) total += equipment.items[i].hpIncRate;
            }
            return playerInfo.hpIncRate + level * 0.1f + total;
        }
        private set => hpIncRate = value;
    }

    public float mpIncRate
    {
        get
        {
            float total = 0;
            for (int i = 0; i < equipment.items.Length; i++)
            {
                if (equipment.items[i] != null) total += equipment.items[i].mpIncRate;
            }
            return playerInfo.mpIncRate + level * 0.1f + total;
        }
        private set => mpIncRate = value;
    }

    public float maxSaturation
    {
        get => 10;
        set => maxSaturation = value;
    }
    
    
    public int[] attackSkillLevel;
    public int[] utilSkillLevel;

    public float maxSpeed;
    public float accel;
    public float jumpPower;

    public float atk 
    {
        get
        {
            float total = 0;
            for (int i = 0; i < equipment.items.Length; i++)
            {
                if (equipment.items[i] != null) total += equipment.items[i].atk;
            }
            return playerInfo.atk + level * 0.1f + total;
        }
        private set => atk = value;
    }
    public float def 
    {
        get
        {
            float total = 0;
            for (int i = 0; i < equipment.items.Length; i++)
            {
                if (equipment.items[i] != null) total += equipment.items[i].def;
            }
            return playerInfo.def + level * 0.1f + total;
        }
        private set => def = value;
    }

    public float stance
    {
        get
        {
            float total = 0;
            for (int i = 0; i < equipment.items.Length; i++)
            {
                if (equipment.items[i] != null) total += equipment.items[i].stance;
            }
            return playerInfo.stance + level * 0.1f + total;
        }
        private set => stance = value;
    }
    
    public int money;

    public Inventory inventory;
    public Equipment equipment;

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
