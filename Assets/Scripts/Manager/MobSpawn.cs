using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MobSpawn : MonoBehaviour
{
    private float playTime;
    private float timeA;
    private float timeB;
    [SerializeField] private SceneInfo sceneInfo;
    private void Update()
    {
        timeA += Time.deltaTime;
        timeB += Time.deltaTime;
        if (timeA > sceneInfo.dungeons[0].respawnTime)
        {
            SpawnA();
        }

        if (timeB >  sceneInfo.dungeons[1].respawnTime)
        {
            SpawnB();
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
            SceneManager.LoadScene("GameOver");
        }
        
    }
    private void SpawnA()
    {
        timeA = 0;
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
        timeB = 0;
        if (sceneInfo.dungeons[1].maxMob > sceneInfo.dungeons[1].curMob)
        {
            sceneInfo.dungeons[1].curMob++;    
        }
        else
        {
            SpawnA();
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
