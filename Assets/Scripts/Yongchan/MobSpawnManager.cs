
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MobSpawnManager : MonoBehaviour
{
    public static MobSpawnManager instance;
    public SceneInfo sceneinfo;

    public int count;
    public Transform[] points;
    private bool isStop;

    private SimplePoolManager pooler;
    public int curMob;
    private void Awake()
    {
        instance = this;
        pooler = GetComponent<SimplePoolManager>();
        curMob = MobUpdate(sceneinfo.currentSceneName);
        count = 0;
    }
    
    private void Update()
    {
        curMob = MobUpdate(sceneinfo.currentSceneName);
        if (count == curMob)
            return;

        if (count < curMob)
        {
            count++;
            Spawn();
        }
    }

    void Spawn()
    {
        int index = Random.Range(0, pooler.prefabs.Length);
        AIPlayer newMob = pooler.Get(index).GetComponent<AIPlayer>();
        newMob.gameObject.SetActive(true);
        newMob.transform.position = new Vector3(points[index].position.x + Random.Range(-2f, 2f), points[index].position.y, 0);
    }
    
    private int MobUpdate(String name)
    {
        switch (name)
        {
            case "Village":
                return  sceneinfo.village.curMob;
            case "Bastion":
                return  sceneinfo.bastion.curMob;
            case "Fork":
                return  sceneinfo.fork.curMob;
            case "WolfA":
                return  sceneinfo.dungeons[0].curMob;
            case "WolfB":
                return  sceneinfo.dungeons[1].curMob;
            case "WhiteA":
                return  sceneinfo.dungeons[2].curMob;
            case "WhiteB":
                return  sceneinfo.dungeons[3].curMob;
            case "StoneA":
                return  sceneinfo.dungeons[4].curMob;
            case "StoneB":
                return  sceneinfo.dungeons[5].curMob;
            case "ZombieA":
                return  sceneinfo.dungeons[6].curMob;
            case "ZombieB":
                return  sceneinfo.dungeons[7].curMob;
        }

        return 0;
    }
}
