using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private float playTime;
    public MapManager sceneInfo
    {
        get => this;
    }
    
    public string beforeSceneName;
    public string currentSceneName;
    public DungeonData[] dungeons;
    public VillageData village;
    public ForkData fork;
    public BastionData bastion;

    public float globalRate;

    private void Update()
    {
        for (int idx = 1; idx < 8; idx += 2)
        {
            sceneInfo.dungeons[idx].time += Time.deltaTime * globalRate;
            if (sceneInfo.dungeons[idx].time > sceneInfo.dungeons[idx].respawnTime)
            {
                sceneInfo.dungeons[idx].time = 0;
                Spawn(idx);
            }
        }
    }

    private void SpawnVillage(int id)
    {
        if (sceneInfo.village.maxMob > sceneInfo.village.curMob)
        {
            sceneInfo.village.curMob++;
            switch (id)
            {
                case 0:
                    sceneInfo.fork.wolf++;
                    break;
                case 1:
                    sceneInfo.fork.white++;
                    break;
                case 2:
                    sceneInfo.fork.stone++;
                    break;
                case 3:
                    sceneInfo.fork.zombie++;
                    break;
                    
            }
        }
        else
        {
            Debug.Log("GameOver");
        }
    }
    private void Spawn(int idx)
    {
        if (sceneInfo.dungeons[idx].maxMob > sceneInfo.dungeons[idx].curMob)
        {
            sceneInfo.dungeons[idx].curMob++;    
        }
        else
        {
            switch (idx)
            {
                case 0:
                    SpawnFork(0);
                    break;
                case 1:
                    Spawn(0);
                    break;
                case 2:
                    SpawnFork(1);
                    break;
                case 3:
                    Spawn(2);
                    break;
                case 4:
                    SpawnFork(2);
                    break;
                case 5:
                    Spawn(4);
                    break;
                case 6:
                    SpawnFork(3);
                    break;
                case 7:
                    Spawn(6);
                    break;
            }
        }
        
    }

    private void SpawnFork(int id)
    {
        if (sceneInfo.fork.maxMob > sceneInfo.fork.curMob)
        {
            sceneInfo.fork.curMob++;
            switch (id)
            {
                case 0:
                    sceneInfo.fork.wolf++;
                    break;
                case 1:
                    sceneInfo.fork.white++;
                    break;
                case 2:
                    sceneInfo.fork.stone++;
                    break;
                case 3:
                    sceneInfo.fork.zombie++;
                    break;
                    
            }
        }
        else
        {
            SpawnBastion(id);
        }
    }
    
    private void SpawnBastion(int id)
    {
        if (sceneInfo.bastion.maxMob > sceneInfo.bastion.curMob)
        {
            sceneInfo.bastion.curMob++;
            switch (id)
            {
                case 0:
                    sceneInfo.bastion.wolf++;
                    break;
                case 1:
                    sceneInfo.bastion.white++;
                    break;
                case 2:
                    sceneInfo.bastion.stone++;
                    break;
                case 3:
                    sceneInfo.bastion.zombie++;
                    break;
                    
            }
        }
        else
        {
            SpawnVillage(id);
        }
    }

    private void Reset()
    {
        sceneInfo.village.curMob = 0;
        sceneInfo.village.wolf = 0;
        sceneInfo.village.white = 0;
        sceneInfo.village.stone = 0;
        sceneInfo.village.zombie = 0;
        sceneInfo.bastion.curMob = 0;
        sceneInfo.bastion.wolf = 0;
        sceneInfo.bastion.white = 0;
        sceneInfo.bastion.stone = 0;
        sceneInfo.bastion.zombie = 0;
        sceneInfo.fork.curMob = 0;
        sceneInfo.fork.wolf = 0;
        sceneInfo.fork.white = 0;
        sceneInfo.fork.stone = 0;
        sceneInfo.fork.zombie = 0;
        
        
        for (int index = 0; index < sceneInfo.dungeons.Length; index++)
        {
            sceneInfo.dungeons[index].curMob = 0;
        }
    }
}
