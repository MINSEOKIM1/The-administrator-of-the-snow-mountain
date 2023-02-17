using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class DataPack
{
    public List<ItemSlot> inventory;
    public EquipmentItemInfo[] equipment;
    public int lv;
    
    public int[] attackSkillLevel;
    public int[] utilSkillLevel;
    
    public float hp;
    public float mp;
    public float saturation;
    public float exp;
    
    public string beforeSceneName;
    public string currentSceneName;
    public DungeonData[] dungeons;
    public VillageData village;
    public ForkData fork;
    public BastionData bastion;

    public float[] spawnRate;

    public float globalRate;
    
    public float playTime;

    public DataPack()
    {
        inventory = new List<ItemSlot>(Inventory.InventoryCapacity);
        equipment = new EquipmentItemInfo[4];
        lv = 1;
        attackSkillLevel = new int[5];
        utilSkillLevel = new int[5];

        hp = 100;
        mp = 100;
        saturation = 100;
        exp = 0;

        beforeSceneName = "START";
        currentSceneName = "Village";

        globalRate = 1;
        playTime = 0;
    }
}