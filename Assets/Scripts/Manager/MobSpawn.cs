using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawn : MonoBehaviour
{
    private float playTime;
    private float[] time;
    [SerializeField] private SceneInfo sceneInfo;
    [SerializeField] private RespawnTimeInfo respawnTimeInfo;

    private void Awake()
    {
        time = new float[10];
        for (int index = 0; index < 10; index++)
        {
            time[index] = 0;
        }
    }

    private void Update()
    {
        time[0] += Time.deltaTime;
        time[1] += Time.deltaTime;
        time[2] += Time.deltaTime;
        time[3] += Time.deltaTime;
        time[4] += Time.deltaTime;
        time[5] += Time.deltaTime;
        time[6] += Time.deltaTime;
        time[7] += Time.deltaTime;
        time[8] += Time.deltaTime;
        time[9] += Time.deltaTime;
        
        if (time[0] > respawnTimeInfo.respawnTime[0])
        {
            time[0] = 0;
            SpawnA();
        }

        if (time[1] >  respawnTimeInfo.respawnTime[1])
        {
            time[1] = 0;
            SpawnB();
        }
        
        if (time[2] >  respawnTimeInfo.respawnTime[2])
        {
            time[2] = 0;
            SpawnC();
        }
        
        if (time[3] >  respawnTimeInfo.respawnTime[3])
        {
            time[3] = 0;
            SpawnD();
        }
        
        if (time[4] >  respawnTimeInfo.respawnTime[4])
        {
            time[4] = 0;
            SpawnE();
        }
        
        if (time[5] >  respawnTimeInfo.respawnTime[5])
        {
            time[5] = 0;
            SpawnF();
        }
        
        if (time[6] >  respawnTimeInfo.respawnTime[6])
        {
            time[6] = 0;
            SpawnG();
        }
        
        if (time[7] >  respawnTimeInfo.respawnTime[7])
        {
            time[7] = 0;
            SpawnH();
        }
        
        if (time[8] >  respawnTimeInfo.respawnTime[8])
        {
            time[8] = 0;
            SpawnI();
        }
        
        if (time[9] >  respawnTimeInfo.respawnTime[9])
        {
            time[9] = 0;
            SpawnJ();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }
    private void SpawnVillage()
    {
        if (sceneInfo.village.maxMob > sceneInfo.village.curMob)
        {
            sceneInfo.village.curMob++;    
        }
        else
        {
            Debug.Log("GameOver");
        }
        
    }
    private void SpawnA()
    {
        if (sceneInfo.dungeons[0].maxMob > sceneInfo.dungeons[0].curMob)
        {
            sceneInfo.dungeons[0].curMob++;    
        }
        else
        {
            SpawnVillage();
        }
        
    }

    private void SpawnB()
    {
        if (sceneInfo.dungeons[1].maxMob > sceneInfo.dungeons[1].curMob)
        {
            sceneInfo.dungeons[1].curMob++;    
        }
        else
        {
            SpawnA();
        }
    }
    
    private void SpawnC()
    {
        if (sceneInfo.dungeons[2].maxMob > sceneInfo.dungeons[2].curMob)
        {
            sceneInfo.dungeons[2].curMob++;    
        }
        else
        {
            SpawnB();
        }
    }
    
    private void SpawnD()
    {
        if (sceneInfo.dungeons[3].maxMob > sceneInfo.dungeons[3].curMob)
        {
            sceneInfo.dungeons[3].curMob++;    
        }
        else
        {
            SpawnC();
        }
    }
    
    private void SpawnE()
    {
        if (sceneInfo.dungeons[4].maxMob > sceneInfo.dungeons[4].curMob)
        {
            sceneInfo.dungeons[4].curMob++;    
        }
        else
        {
            SpawnD();
        }
    }

    private void SpawnF()
    {
        if (sceneInfo.dungeons[5].maxMob > sceneInfo.dungeons[5].curMob)
        {
            sceneInfo.dungeons[5].curMob++;    
        }
        else
        {
            SpawnD();
        }
    }
    
    private void SpawnG()
    {
        if (sceneInfo.dungeons[6].maxMob > sceneInfo.dungeons[6].curMob)
        {
            sceneInfo.dungeons[6].curMob++;    
        }
        else
        {
            SpawnB();
        }
    }
    
    private void SpawnH()
    {
        if (sceneInfo.dungeons[7].maxMob > sceneInfo.dungeons[7].curMob)
        {
            sceneInfo.dungeons[7].curMob++;    
        }
        else
        {
            SpawnG();
        }
    }
    
    private void SpawnI()
    {
        if (sceneInfo.dungeons[8].maxMob > sceneInfo.dungeons[8].curMob)
        {
            sceneInfo.dungeons[8].curMob++;    
        }
        else
        {
            SpawnG();
        }
    }
    
    private void SpawnJ()
    {
        if (sceneInfo.dungeons[9].maxMob > sceneInfo.dungeons[9].curMob)
        {
            sceneInfo.dungeons[9].curMob++;    
        }
        else
        {
            SpawnI();
        }
    }
    private void Reset()
    {
        sceneInfo.village.curMob = 0;
        for (int index = 0; index < sceneInfo.dungeons.Length; index++)
        {
            sceneInfo.dungeons[index].curMob = 0;
        }
    }
}
