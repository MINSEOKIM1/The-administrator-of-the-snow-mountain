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
        }
    }


}
