using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PortalManager : MonoBehaviour
{
    // # sceneInfo : scriptable objects
    [SerializeField] private SceneInfo sceneInfo;
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
                SceneManager.LoadScene("Village");
                break;
            case "Bastion":
                SceneManager.LoadScene("Bastion");
                break;
            case "Fork":
                SceneManager.LoadScene("Fork");
                break;
            case "WolfA":
                SceneManager.LoadScene("WolfA");
                break;
            case "WolfB":
                SceneManager.LoadScene("WolfB");
                break;
            case "WhiteA":
                SceneManager.LoadScene("WhiteA");
                break;
            case "WhiteB":
                SceneManager.LoadScene("WhiteB");
                break;
            case "StoneA":
                SceneManager.LoadScene("StoneA");
                break;
            case "StoneB":
                SceneManager.LoadScene("StoneB");
                break;
            case "ZombieA":
                SceneManager.LoadScene("ZombieA");
                break;
            case "ZombieB":
                SceneManager.LoadScene("ZombieB");
                break;
        }
    }

    public Vector3 SpawnPosition(string curScene, string prevScene)
    {
        switch (curScene)
        {
            case "Village":
                return portals[0].transform.position;
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
            default:
                return new Vector3(0, 0, 0);
        }
    }


}
