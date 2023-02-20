using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class MapManager : MonoBehaviour
{
    public float playTime;
    public bool gameStart;
    public bool gameover;
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

    public float[] spawnRate;

    public float globalRate;

    public DungeonData GetMapWithString(string name)
    {
        foreach (var i in dungeons)
        {
            if (name.Equals(i.name)) return i;
        }

        if (name.Equals(village.name)) return village;
        if (name.Equals(fork.name)) return fork;
        if (name.Equals(bastion.name)) return bastion;
        if (name.Equals("MainMenu")) return new DungeonData();
        return null;
    }

    public bool a;
    private void Update()
    {
        if (!gameStart) return;
        globalRate += Time.deltaTime * 0.0001f;
        playTime += Time.deltaTime;
        for (int idx = 0; idx < dungeons.Length; idx++)
        {
            if (dungeons[idx].agent.timeTakenToHunt != 0)
            {
                var agent = dungeons[idx].agent;
                if (agent.timeElapsed < agent.timeTakenToHunt * agent.timeRate)
                {
                    agent.timeElapsed += Time.deltaTime;
                }
                else if (dungeons[idx].curMob > 0 && !GameManager.Instance.MapManager.currentSceneName.Equals(dungeons[idx].name))
                {
                    dungeons[idx].curMob--;
                    agent.timeElapsed = 0;
                }
            }
        }
        for (int idx = 1; idx < 8; idx += 2)
        {
            if (sceneInfo.dungeons[idx].boss) sceneInfo.spawnRate[idx] = 2;
            else sceneInfo.spawnRate[idx] = 1;
            sceneInfo.dungeons[idx].time += Time.deltaTime * globalRate * spawnRate[idx];
            if (sceneInfo.dungeons[idx].bossRespawnTime != 0 && !sceneInfo.dungeons[idx].boss) 
                sceneInfo.dungeons[idx].bossTime += Time.deltaTime * globalRate;
            if (sceneInfo.dungeons[idx].time > sceneInfo.dungeons[idx].respawnTime)
            {
                sceneInfo.dungeons[idx].time = 0;
                Spawn(idx);
            }
            
            if (sceneInfo.dungeons[idx].bossTime > sceneInfo.dungeons[idx].bossRespawnTime 
                && !sceneInfo.dungeons[idx].boss)
            {
                sceneInfo.dungeons[idx].bossTime = 0;
                sceneInfo.dungeons[idx].boss = true;
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
            if (!a)
            {
                gameover = true;
                GameManager.Instance.GameSceneManager.GameOver();
                Debug.Log("GameOver");
                a = true;
            }
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
