using System;
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
    
    [SerializeField] private GraphicRaycaster graphic;

    public GameObject selectedMapInfo;
    public TMP_Text selectedMapInfoText;
    public Slider[] selectedMapInfoSliders;

    public GameObject agentInfo;
    public Button agentAllocateButton;

    public bool isAllocating;
    public AgentData currentAllocatedAgent;

    public string selectedMapName;

    public bool isTeleporting;
    public Button teleportButton;

    public delegate void AgentNPCdelegate();

    public event AgentNPCdelegate agentNPCEvent;
    private void Start()
    {
         // # UI Update
         
    }

    public void TeleportToMap()
    {
        GameManager.Instance.GameSceneManager.LoadScene(selectedMapName);
        gameObject.SetActive(false);
        GameManager.Instance.PlayerDataManager.inventory.DeleteItem(
            GameManager.Instance.ScriptableObjectManager.GetWithIndex(200), 1);
        GameManager.Instance.UIManager.MapUI.UpdateMapPoint();
    }

    float RoundTo(float c, int a)
    {
        return Mathf.Round(c * Mathf.Pow(10, a)) / Mathf.Pow(10, a);
    }

    public void UpdateMapPoint()
    {
        for (int i = 0; i < GameManager.Instance.MapManager.dungeons.Length; i++)
        {
            if (i < GameManager.Instance.MapManager.dungeons.Length)
            {
                TurnOffPoint(GameManager.Instance.MapManager.dungeons[i].name);
            }
        }
        
        TurnOffPoint("Village");
        TurnOnPoint(sceneInfo.currentSceneName);
    }

    private void OnDisable()
    {
        isTeleporting = false;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) isTeleporting = false;
        if (isTeleporting)
        {
            teleportButton.gameObject.SetActive(true);
        }
        else
        {
            teleportButton.gameObject.SetActive(false);
        }
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
        
        if (selectedMapName.Equals(""))
        {
            selectedMapInfo.SetActive(false);
        }
        else
        {
            selectedMapInfo.SetActive(true);
            var map = GameManager.Instance.MapManager.GetMapWithString(selectedMapName);
            if (isAllocating)
            {
                agentAllocateButton.gameObject.SetActive(true);
            }
            else
            {
                agentAllocateButton.gameObject.SetActive(false);
            }
            if (map.agent.timeTakenToHunt != 0)
            {
                agentInfo.SetActive(true);
                agentInfo.GetComponent<AgentUI>().SetAgent(map.agent);
            }
            else
            {
                agentInfo.SetActive(false);
            }
            if (GameManager.Instance.MapManager.GetMapWithString(selectedMapName).respawnTime != 0)
            {
                float rate;
                switch (selectedMapName)
                {
                    case "WolfB" :
                        rate = GameManager.Instance.MapManager.globalRate *
                               GameManager.Instance.MapManager.spawnRate[1];
                        break;
                    case "WhiteB" :
                        rate = GameManager.Instance.MapManager.globalRate *
                               GameManager.Instance.MapManager.spawnRate[3];
                        break;
                    case "StoneB" :
                        rate = GameManager.Instance.MapManager.globalRate *
                               GameManager.Instance.MapManager.spawnRate[5];
                        break;
                    case "ZombieB" :
                        rate = GameManager.Instance.MapManager.globalRate *
                               GameManager.Instance.MapManager.spawnRate[7];
                        break;
                    default:
                        rate = 1;
                        break;
                }

                
                selectedMapInfoText.text = String.Format(
                    "{0}\n몬스터 수 - {5} / {6}\n다음 몬스터 생성까지..\n{1,8} / {2} (초)\n\n다음 보스 생성까지.. \n{3,8} / {4} (초)",
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName).explicitName,
                    RoundTo(GameManager.Instance.MapManager.GetMapWithString(selectedMapName).time / rate, 2),
                    RoundTo(GameManager.Instance.MapManager.GetMapWithString(selectedMapName).respawnTime / rate, 2),
                    RoundTo(GameManager.Instance.MapManager.GetMapWithString(selectedMapName).bossTime / GameManager.Instance.MapManager.globalRate, 2),
                    RoundTo(GameManager.Instance.MapManager.GetMapWithString(selectedMapName).bossRespawnTime / GameManager.Instance.MapManager.globalRate, 2),
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName).curMob,
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName).maxMob);

                selectedMapInfoSliders[0].gameObject.SetActive(true);
                selectedMapInfoSliders[1].gameObject.SetActive(true);
                
                selectedMapInfoSliders[0].value =
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName).time /
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName)
                        .respawnTime;
                selectedMapInfoSliders[1].value =
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName).bossTime /
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName)
                        .bossRespawnTime;
            }
            else
            {
                selectedMapInfoText.text = String.Format(
                    "{0}\n몬스터 수 - {1} / {2}\n",
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName).explicitName,
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName).curMob,
                    GameManager.Instance.MapManager.GetMapWithString(selectedMapName).maxMob);
                selectedMapInfoSliders[0].gameObject.SetActive(false);
                selectedMapInfoSliders[1].gameObject.SetActive(false);
            }
        }
    }

    public void AllocateAgent()
    {
        var goToMap = GameManager.Instance.MapManager.GetMapWithString(selectedMapName);
        var curMap = GameManager.Instance.MapManager.GetMapWithString(GameManager.Instance.MapManager.currentSceneName);
        if (goToMap.agent.timeTakenToHunt != 0)
        {
            GameManager.Instance.UIManager.PopMessage("이미 대리인이 있습니다.", 3);
        }
        else
        {
            isAllocating = false;
            currentAllocatedAgent.currentMapName = goToMap.name;
            currentAllocatedAgent.timeRate = goToMap.agentTimeRate;
            goToMap.agent = currentAllocatedAgent;
            curMap.agent = new AgentData();
            agentNPCEvent.Invoke();
        }
    }

    public void SelectMap(string name)
    {
        selectedMapName = name;
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
        for (int index = 0; index < 8; index++)
        {
            texts[index+3].text = "" + sceneInfo.dungeons[index].curMob + "/" + sceneInfo.dungeons[index].maxMob;
        }
        
    }
}
