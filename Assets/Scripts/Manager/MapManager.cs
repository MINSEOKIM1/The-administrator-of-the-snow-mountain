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

    private void Start()
    {
        dungeons[8] = fork;
        dungeons[9] = bastion;
    }

    public bool a;
    private void Update()
    {
        if (!gameStart) return;
        playTime += Time.deltaTime;

        if (currentSceneName.Equals("Village") && GameManager.Instance.PlayerDataManager.tutorial < 18)
        {
            GameManager.Instance.PlayerDataManager.tutorial++;
        }

        if (GameManager.Instance.PlayerDataManager.tutorial < 18) return;
        globalRate += Time.deltaTime * 0.0001f;
        
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
                    
                    
                    if (dungeons[idx].name.Equals("Fork"))
                    {
                        fork.curMob--;
                        if (fork.white > 0) fork.white--;
                        else if (fork.wolf > 0) fork.wolf--;
                        else if (fork.stone > 0) fork.stone--;
                        else fork.zombie--;
                        agent.timeElapsed = 0;
                    } else if (dungeons[idx].name.Equals("Bastion"))
                    {
                        bastion.curMob--;
                        if (fork.white > 0) fork.white--;
                        else if (fork.wolf > 0) fork.wolf--;
                        else if (fork.stone > 0) fork.stone--;
                        else fork.zombie--;
                        agent.timeElapsed = 0;
                    }
                    else
                    {
                        dungeons[idx].curMob--;
                        agent.timeElapsed = 0;
                    }
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
                GameManager.Instance.UIManager.PopMessage("<#818AFF>" + sceneInfo.dungeons[idx].explicitName +
                                                          "</color> 에 보스 몬스터가 출현했습니다.", 3);
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
        if (fork.curMob == 10)
        {
            GameManager.Instance.UIManager.PopMessage("<#FF7F7F>" + fork.explicitName +
                                                      " 에 몬스터 개체 수가 10 마리가 되었습니다. 관리가 필요합니다.</color>", 3);
        } 
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
        if (bastion.curMob == 1)
        {
            GameManager.Instance.UIManager.PopMessage("<#FF7F7F>" + bastion.explicitName +
                                                      " 에 몬스터가 출현했습니다. 관리가 필요합니다. </color>", 3);
        } else if (bastion.curMob >= 6)
        {
            GameManager.Instance.UIManager.PopMessage("<#FF7F7F>" + bastion.explicitName +
                                                      " 에 몬스터가 가득 차고 있습니다. 신속히 관리가 필요합니다.</color>", 3);
        }
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
