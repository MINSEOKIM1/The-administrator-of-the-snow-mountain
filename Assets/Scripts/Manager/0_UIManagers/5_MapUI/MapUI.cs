using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    private MapManager sceneInfo
    {
        get => GameManager.Instance.MapManager;
    }
    [SerializeField] private GameObject[] points;
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private Image[] bossMarks;
    [SerializeField] private Image[] spawnTimeMark;
    [SerializeField] private Image[] bossSpawnTimeMark;
    private void Start()
    {
         // # UI Update
         
    }

    private void Update()
    {
        MopNumUpdate();
        TurnOnPoint(sceneInfo.currentSceneName);
        TurnOffPoint(sceneInfo.beforeSceneName);
        for (int i = 0; i < 4; i++)
        {
            bossMarks[i].gameObject.SetActive(sceneInfo.dungeons[1+2*i].boss);
            spawnTimeMark[i].fillAmount =
                sceneInfo.dungeons[1 + 2 * i].time / sceneInfo.dungeons[1 + 2 * i].respawnTime;
            bossSpawnTimeMark[i].fillAmount =
                sceneInfo.dungeons[1 + 2 * i].bossTime / sceneInfo.dungeons[1 + 2 * i].bossRespawnTime;
        }
    }

    private void TurnOnPoint(string state)
    {
        switch (state)
        {
            case "Village":
                points[0].SetActive(true);
                break;
            case "Bastion":
                points[1].SetActive(true);
                break;
            case "Fork":
                points[2].SetActive(true);
                break;
            case "WolfA":
                points[3].SetActive(true);
                break;
            case "WolfB":
                points[4].SetActive(true);
                break;
            case "WhiteA":
                points[5].SetActive(true);
                break;
            case "WhiteB":
                points[6].SetActive(true);
                break;
            case "StoneA":
                points[7].SetActive(true);
                break;
            case "StoneB":
                points[8].SetActive(true);
                break;
            case "ZombieA":
                points[9].SetActive(true);
                break;
            case "ZombieB":
                points[10].SetActive(true);
                break;
        }
    }
    
    private void TurnOffPoint(string state)
    {
        switch (state)
        {
            case "Village":
                points[0].SetActive(false);
                break;
            case "Bastion":
                points[1].SetActive(false);
                break;
            case "Fork":
                points[2].SetActive(false);
                break;
            case "WolfA":
                points[3].SetActive(false);
                break;
            case "WolfB":
                points[4].SetActive(false);
                break;
            case "WhiteA":
                points[5].SetActive(false);
                break;
            case "WhiteB":
                points[6].SetActive(false);
                break;
            case "StoneA":
                points[7].SetActive(false);
                break;
            case "StoneB":
                points[8].SetActive(false);
                break;
            case "ZombieA":
                points[9].SetActive(false);
                break;
            case "ZombieB":
                points[10].SetActive(false);
                break;
        }
    }

    private void MopNumUpdate()
    {
        texts[0].text = "" + sceneInfo.village.curMob + "/" + sceneInfo.village.maxMob;
        texts[1].text = "" + sceneInfo.bastion.curMob + "/" + sceneInfo.bastion.maxMob;
        texts[2].text = "" + sceneInfo.fork.curMob + "/" + sceneInfo.fork.maxMob;
        for (int index = 0; index < sceneInfo.dungeons.Length; index++)
        {
            texts[index+3].text = "" + sceneInfo.dungeons[index].curMob + "/" + sceneInfo.dungeons[index].maxMob;
        }
        
    }
}
