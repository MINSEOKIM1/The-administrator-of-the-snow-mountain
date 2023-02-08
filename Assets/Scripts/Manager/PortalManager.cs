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

    public void Update()
    {
        
    }

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


}
