using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class PlayerDataManager : MonoBehaviour
{
    public PlayerInfo playerInfo;
    
    public int level;
    
    public float hp;
    public float mp;
    public float saturation;
    public float exp;

    public ConsumableItemInfo[] quickSlotItems;
    public bool[] agentAvailable;

    public bool loadPosFromData;
    public Vector3 loadPos;
    [field: SerializeField] public float attackSpeed
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
            return playerInfo.maxHp + level * 30 + total;
        }
        set => maxHp = value;
    }

    public float hpDecRate, mpDecRate;
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
            float to = playerInfo.hpIncRate + level * 0.1f + total;
            if (saturation > 0) to *= (saturation / maxSaturation);
            else
            {
                return -hpDecRate;
            }
            return to;
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
            float to = playerInfo.mpIncRate + level * 0.1f + total;
            if (saturation > 0) to *= (saturation / maxSaturation);
            else
            {
                return -mpDecRate;
            }
            return to;
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
        get => 100 * (1 + 0.2f * (level-1));
    }
    
    public int[] attackSkillLevel;
    public int[] utilSkillLevel;

    public float maxSpeed;
    public float accel;
    public float jumpPower;

    public bool canControl
    {
        get => !GameManager.Instance.UIManager.ConservationUI.gameObject.activeSelf || GameManager.Instance.GameSceneManager.gameover;
        set => canControl = value;
    }

    public int tutorial;

    public float atk 
    {
        get
        {
            float total = 0;
            for (int i = 0; i < equipment.items.Length; i++)
            {
                if (equipment.items[i] != null) total += equipment.items[i].atk;
            }
            return playerInfo.atk + level * 10f + total;
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
            return playerInfo.stance + total;
        }
        private set => stance = value;
    }

    public bool isDie;

    public Inventory inventory;
    public Equipment equipment;

    public Vector3 camOffset;

    private void Start()
    {
        hp = maxHp;
        mp = maxMp;
        saturation = maxSaturation;
        volume.profile.TryGet<Vignette>(out tmp);
    }

    private bool a;
    public Volume volume;
    private Vignette tmp;
    private float _vignetteIntensity;
    public float vignettePulseInterval;
    public float vignetteAmplitude;

    private bool satOk;

    private void Update()
    {
        hp = Mathf.Clamp(hp, 0, maxHp);
        mp = Mathf.Clamp(mp, 0, maxMp);
        saturation = Mathf.Clamp(saturation, 0, maxSaturation);
        _vignetteIntensity += Time.deltaTime * vignettePulseInterval;

        if (hp < maxHp / 3)
        {
            if (tmp)
            {
                tmp.intensity.Interp(tmp.intensity.value, vignetteAmplitude * Mathf.Sin(_vignetteIntensity) + vignetteAmplitude * 1.1f, Time.deltaTime);
            }
        }
        else
        {
            if (tmp)
            {
                tmp.intensity.Interp(tmp.intensity.value, 0, Time.deltaTime);
            }
        }
        if (tutorial >= 18) saturation -= Time.deltaTime * playerInfo.saturationDecrementRate;
        if (saturation <= 0)
        {
            if (satOk)
            {
                satOk = false;
                GameManager.Instance.UIManager.PopMessage("포만감이 0이 되어 HP와 MP가 감소합니다.", 3);
                GameManager.Instance.UIManager.PopMessage("음식을 먹어 포만감을 채우세요.", 3);
            }
        }
        else
        {
            satOk = true;
        }

        if (!isDie)
        {
            hp += hpIncRate * Time.deltaTime;
            mp += mpIncRate * Time.deltaTime;
        }

        if (exp >= maxExp)
        {
            LevelUP();
        }
        

        if (hp <= 0 || GameManager.Instance.MapManager.gameover)
        {
            if (!a)
            {
                a = true;
                GameManager.Instance.GameSceneManager.GameOver();
            }
        }
    }
    

    private void LevelUP()
    {
        level++;
        hp = maxHp;
        mp = maxMp;
        exp = 0;
    }
}
