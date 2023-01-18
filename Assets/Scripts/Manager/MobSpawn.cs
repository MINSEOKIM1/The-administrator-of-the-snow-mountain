using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (timeA > 2.3f)
        {
            SpawnA();
        }

        if (timeB > 1.5f)
        {
            SpawnB();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    private void SpawnA()
    {
        timeA = 0;
        sceneInfo.dungeons[0].curMob++;
    }

    private void SpawnB()
    {
        timeB = 0;
        sceneInfo.dungeons[1].curMob++;
    }

    private void Reset()
    {
        for (int index = 0; index < sceneInfo.dungeons.Length; index++)
        {
            sceneInfo.dungeons[index].curMob = 0;
        }
    }
}
