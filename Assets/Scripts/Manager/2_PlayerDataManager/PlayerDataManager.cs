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
    public float exp;

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
        get => 100;
        set => maxSaturation = value;
    }

    public float maxExp
    {
        get => 100 * (1 + 0.02f * (level-1));
    }
    
    
    public int[] attackSkillLevel;
    public int[] utilSkillLevel;

    public float maxSpeed;
    public float accel;
    public float jumpPower;

    public bool canControl
    {
        get => !GameManager.Instance.UIManager.ConservationUI.gameObject.activeSelf;
        set => canControl = value;
    }

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

    public bool isDie;

    public Inventory inventory;
    public Equipment equipment;

    public Vector3 camOffset;

    private void Start()
    {
        hp = maxHp;
        mp = maxMp;
        saturation = maxSaturation;
    }

    private void Update()
    {
        hp = Mathf.Clamp(hp, 0, maxHp);
        mp = Mathf.Clamp(mp, 0, maxMp);
        saturation = Mathf.Clamp(saturation, 0, maxSaturation);

        saturation -= Time.deltaTime * playerInfo.saturationDecrementRate;

        if (!isDie)
        {
            hp += hpIncRate * Time.deltaTime;
            mp += mpIncRate * Time.deltaTime;
        }

        if (exp >= maxExp)
        {
            LevelUP();
        }
    }

    private void LevelUP()
    {
        level++;
        exp = 0;
    }
}
