
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MobSpawnManager : MonoBehaviour
{
    public static MobSpawnManager instance;
    public MapManager sceneinfo
    {
        get => GameManager.Instance.MapManager;
    }

    public int count;
    public Transform[] points;
    private bool isStop;
    public int[] curMobs;
    public int[] maxMobs;
    public bool mix;
    public int[] floorMobs;
    public int[] floorMax;
    
    private SimplePoolManager pooler;
    public int curMob;
    private void Awake()
    {
        curMobs = new int[4];
        maxMobs = new int[4];
        instance = this;
        pooler = GetComponent<SimplePoolManager>();
        count = 0;
        floorMobs = new int[5];
        floorMax = new int[] { 5, 3, 3, 3 };
    }
    
    private void Update()
    {
        if (pooler.poolReady == false) return;
        if (mix)
        {
            if(sceneinfo.currentSceneName == "Bastion")
                maxMobs = BastionMobUpdate();
            if (sceneinfo.currentSceneName == "Fork")
                maxMobs = ForkMobUpdate();
            
            for (int index = 0; index < 4; index++)
            {
                if (curMobs[index] < maxMobs[index])
                {
                    curMobs[index]++;
                    int pos = 0;
                    if (floorMobs[index] < floorMax[index])
                    {
                        pos = index;
                        floorMobs[index]++;
                    }
                    else
                    {
                        pos = Shuffle(index);
                    }
                    Spawn(index, pos);
                }
            }
        }
        else
        {
            curMob = MobUpdate(sceneinfo.currentSceneName);

            if (count < curMob)
            {
                count++;
                Spawn();
            }    
        }
    }

    private int Shuffle(int index)
    {
        int pos = 4;
        switch (index)
        {
            case 0:
                for (int i = 1; i < 4; i++)
                {
                    if (floorMobs[i] < floorMax[i])
                    {
                        floorMobs[i]++;
                        pos = i;
                        break;
                    } 
                }
                break;
            case 1:
                if (floorMobs[0] < floorMax[0])
                {
                    floorMobs[0]++;
                    pos = 0;
                    break;
                } 
                for (int i = 2; i < 4; i++)
                {
                    if (floorMobs[i] < floorMax[i])
                    {
                        floorMobs[i]++;
                        pos = i;
                        break;
                    } 
                }
                break;
            case 2:
                for (int i = 0; i < 2; i++)
                {
                    if (floorMobs[i] < floorMax[i])
                    {
                        floorMobs[i]++;
                        pos = i;
                        break;
                    } 
                }
                if (floorMobs[3] < floorMax[3])
                {
                    floorMobs[3]++;
                    pos = 3;
                    break;
                } 
                break;
            case 3:
                for (int i = 0; i < 3; i++)
                {
                    if (floorMobs[i] < floorMax[i])
                    {
                        floorMobs[i]++;
                        pos = i;
                        break;
                    } 
                }
                break;
        }

        return pos;
    }

    void Spawn()
    {
        int index = Random.Range(0, pooler.prefabs.Length);
        while (pooler.Get(index) == null)
        {
            Debug.Log(index);
            index++;
            if (index >= pooler.prefabs.Length)
                index = 0;
        }
        Debug.Log("SPAWN!");
        AIPlayer newMob = pooler.Get(index).GetComponent<AIPlayer>();
        newMob.gameObject.SetActive(true);
        newMob.transform.position = new Vector3(points[index].position.x + Random.Range(-2f, 2f), points[index].position.y, 0);
    }

    void Spawn(int idx, int pos)
    {
        AIPlayer newMob = pooler.Get(idx).GetComponent<AIPlayer>();
        newMob.gameObject.SetActive(true);
        newMob.transform.position = new Vector3(points[pos].position.x + Random.Range(-2f, 2f), points[pos].position.y, 0);
    }

    private int[] BastionMobUpdate()
    {
        int[] mobs = new[]
            { sceneinfo.bastion.wolf, sceneinfo.bastion.white, sceneinfo.bastion.stone, sceneinfo.bastion.zombie };
        return mobs;
    }
    
    private int[] ForkMobUpdate()
    {
        int[] mobs = new[]
            { sceneinfo.fork.wolf, sceneinfo.fork.white, sceneinfo.fork.stone, sceneinfo.fork.zombie };
        return mobs;
    }
    private int MobUpdate(String name)
    {
        switch (name)
        {
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

    public void MobDie(String name, String idName)
    {
        switch (name)
        {
            case "Bastion":
                sceneinfo.bastion.curMob--;
                switch (idName)
                {
                    case "Wolf":
                        maxMobs[0]--;
                        curMobs[0]--;
                        sceneinfo.bastion.wolf--;
                        break;
                    case "White":
                        maxMobs[1]--;
                        curMobs[1]--;
                        sceneinfo.bastion.white--;
                        break;
                    case "Stone":
                        maxMobs[2]--;
                        curMobs[2]--;
                        sceneinfo.bastion.stone--;
                        break;
                    case "Zombie":
                        maxMobs[3]--;
                        curMobs[3]--;
                        sceneinfo.bastion.zombie--;
                        break;
                }
                break;
            case "Fork":
                sceneinfo.fork.curMob--;
                switch (idName)
                {
                    case "Wolf":
                        maxMobs[0]--;
                        curMobs[0]--;
                        sceneinfo.fork.wolf--;
                        break;
                    case "White":
                        maxMobs[1]--;
                        curMobs[1]--;
                        sceneinfo.fork.white--;
                        break;
                    case "Stone":
                        maxMobs[2]--;
                        curMobs[2]--;
                        sceneinfo.fork.stone--;
                        break;
                    case "Zombie":
                        maxMobs[3]--;
                        curMobs[3]--;
                        sceneinfo.fork.zombie--;
                        break;
                }
                break;
            case "WolfA":
                curMob--;
                count--;
                sceneinfo.dungeons[0].curMob--;
                break;
            case "WolfB":
                curMob--;
                count--;
                sceneinfo.dungeons[1].curMob--;
                break;
            case "WhiteA":
                curMob--;
                count--;
                sceneinfo.dungeons[2].curMob--;
                break;
            case "WhiteB":
                curMob--;
                count--;
                sceneinfo.dungeons[3].curMob--;
                break;
            case "StoneA":
                curMob--;
                count--;
                sceneinfo.dungeons[4].curMob--;
                break;
            case "StoneB":
                curMob--;
                count--;
                sceneinfo.dungeons[5].curMob--;
                break;
            case "ZombieA":
                curMob--;
                count--;
                sceneinfo.dungeons[6].curMob--;
                break;
            case "ZombieB":
                curMob--;
                count--;
                sceneinfo.dungeons[7].curMob--;
                break;
        }
    }
    
    public int GetRandomNotContain(int _min, int _max, int[] _notContainValue)
    {
        HashSet<int> exclude = new HashSet<int>();
        for (int i = 0; i < _notContainValue.Length; i++)
        {
            exclude.Add(_notContainValue[i]);
        }

        var range = Enumerable.Range(_min, _max).Where(i => !exclude.Contains(i));
        var rand = new System.Random();
        int index = rand.Next(_min, _max - exclude.Count);
        return range.ElementAt(index);
    }
}
