using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private SceneInfo sceneInfo;
    [SerializeField] private GameObject[] points;
    [SerializeField] private TextMeshProUGUI[] texts;
    private void Start()
    {
         // # UI Update
         TurnOnPoint(sceneInfo.currentSceneName);
         TurnOffPoint(sceneInfo.beforeSceneName);
    }

    private void Update()
    {
        MopNumUpdate();
    }

    private void TurnOnPoint(string state)
    {
        switch (state)
        {
            case "Village":
                points[0].SetActive(true);
                break;
            case "DungeonA":
                points[1].SetActive(true);
                break;
            case "DungeonB":
                points[2].SetActive(true);
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
            case "DungeonA":
                points[1].SetActive(false);
                break;
            case "DungeonB":
                points[2].SetActive(false);
                break;
        }
    }

    private void MopNumUpdate()
    {
        texts[0].text = "" + sceneInfo.village.curMob + "/" + sceneInfo.village.maxMob;
        for (int index = 0; index < sceneInfo.dungeons.Length; index++)
        {
            texts[index+1].text = "" + sceneInfo.dungeons[index].curMob + "/" + sceneInfo.dungeons[index].maxMob;
        }
        
    }
}
