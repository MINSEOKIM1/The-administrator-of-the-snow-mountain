using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PortalManager : MonoBehaviour
{
    // # sceneInfo : scriptable objects
    private MapManager sceneInfo
    {
        get => GameManager.Instance.MapManager;
    }

    private GameSceneManager GameSceneManager => GameManager.Instance.GameSceneManager;
    [SerializeField] private GameObject[] portals;

    public void ChangeScene(string portalName)
    {
        
        // # sceneInfo Update
        sceneInfo.beforeSceneName = SceneManager.GetActiveScene().name;
        sceneInfo.currentSceneName = portalName;
        
        
        // # Village or Dungeon Scene Load and 
        switch (portalName)
        {
            case "Village":
                GameSceneManager.LoadScene("Village");
                break;
            case "Bastion":
                GameSceneManager.LoadScene("Bastion");
                break;
            case "Fork":
                GameSceneManager.LoadScene("Fork");
                break;
            case "WolfA":
                GameSceneManager.LoadScene("WolfA");
                break;
            case "WolfB":
                GameSceneManager.LoadScene("WolfB");
                break;
            case "WhiteA":
                GameSceneManager.LoadScene("WhiteA");
                break;
            case "WhiteB":
                GameSceneManager.LoadScene("WhiteB");
                break;
            case "StoneA":
                GameSceneManager.LoadScene("StoneA");
                break;
            case "StoneB":
                GameSceneManager.LoadScene("StoneB");
                break;
            case "ZombieA":
                GameSceneManager.LoadScene("ZombieA");
                break;
            case "ZombieB":
                GameSceneManager.LoadScene("ZombieB");
                break;
        }
    }

    public Vector3 SpawnPosition(string curScene, string prevScene)
    {
        switch (curScene)
        {
            case "Village":
                switch (prevScene)
                {
                    case "Bastion" :
                        return portals[0].transform.position;
                    default:
                        return portals[1].transform.position;
                }
            case "Bastion":
                return prevScene == "Village" ? portals[0].transform.position : portals[1].transform.position;
            case "Fork":
                switch (prevScene)
                {
                    case "Bastion":
                        return portals[0].transform.position;
                    case "WolfA":
                        return portals[1].transform.position;
                    case "WhiteA":
                        return portals[2].transform.position;
                    case "StoneA":
                        return portals[3].transform.position;
                    case "ZombieA":
                        return portals[4].transform.position;
                    default:
                        return new Vector3(0, 0, 0);
                }
            case "WolfA":
                return prevScene == "Fork" ? portals[0].transform.position : portals[1].transform.position;
            case "WolfB":
                return portals[0].transform.position;
            case "WhiteA":
                return prevScene == "Fork" ? portals[0].transform.position : portals[1].transform.position;
            case "WhiteB":
                return portals[0].transform.position;
            case "StoneA":
                return prevScene == "Fork" ? portals[0].transform.position : portals[1].transform.position;
            case "StoneB":
                return portals[0].transform.position;
            case "ZombieA":
                return prevScene == "Fork" ? portals[0].transform.position : portals[1].transform.position;
            case "ZombieB":
                return portals[0].transform.position;
            case "Tutorial":
                return portals[0].transform.position;
            default:
                return new Vector3(0, 0, 0);
        }
    }
}
