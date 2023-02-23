using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class DataPack
{
    public Tuple<int, int>[] inventory;
    public int[] inventoryIndex;
    public int[] inventoryCount;
    public int[] equipment;
    public int lv;
    
    public int[] attackSkillLevel;
    public int[] utilSkillLevel;
    
    public float hp;
    public float mp;
    public float saturation;
    public float exp;

    public Vector3 position;

    public bool[] agentAvailable;

    public int tutorial;
    
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
        inventory = new Tuple<int,int>[Inventory.InventoryCapacity];
        inventoryIndex = new int[Inventory.InventoryCapacity];
        inventoryCount = new int[Inventory.InventoryCapacity];
        equipment = new int[4];
        lv = 1;
        attackSkillLevel = new int[5];
        utilSkillLevel = new int[5];

        hp = 100;
        mp = 100;
        saturation = 100;
        exp = 0;

        tutorial = 0;
        
        position = Vector3.zero;

        beforeSceneName = "START";
        currentSceneName = "Tutorial";

        globalRate = 1;
        playTime = 0;
    }

    public void SetInvenIndexCount()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] != null)
            {
                inventoryIndex[i] = inventory[i].Item1;
                inventoryCount[i] = inventory[i].Item2;
            }
        }
    }

    public Tuple<int, int>[] GetInvenSaveData()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new Tuple<int, int>(inventoryIndex[i], inventoryCount[i]);
        }

        return inventory;
    }
}