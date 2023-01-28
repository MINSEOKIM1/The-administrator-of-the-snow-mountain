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
            case "DungeonA":
                SceneManager.LoadScene("DungeonA");
                break;
            case "DungeonB":
                SceneManager.LoadScene("DungeonB");
                break;
            case "DungeonC":
                SceneManager.LoadScene("DungeonC");
                break;
            case "DungeonD":
                SceneManager.LoadScene("DungeonD");
                break;
            case "DungeonE":
                SceneManager.LoadScene("DungeonE");
                break;
            case "DungeonF":
                SceneManager.LoadScene("DungeonF");
                break;
            case "DungeonG":
                SceneManager.LoadScene("DungeonG");
                break;
            case "DungeonH":
                SceneManager.LoadScene("DungeonH");
                break;
            case "DungeonI":
                SceneManager.LoadScene("DungeonI");
                break;
            case "DungeonJ":
                SceneManager.LoadScene("DungeonJ");
                break;
        }
    }

    public void NewStart()
    {
        sceneInfo.beforeSceneName = "MainMenu";
        sceneInfo.currentSceneName = "Tutorial";
        SceneManager.LoadScene("Tutorial");
    }

    public void Exit()
    {
        Application.Quit();
    }


}
