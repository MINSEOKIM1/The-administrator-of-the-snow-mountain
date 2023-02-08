using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private SceneInfo sceneInfo;
    [SerializeField] private PortalManager portalManager;

    // # player spawn point setting by sceneInfo (currentScene and beforeScene)
    private void Awake()
    {
        switch (sceneInfo.currentSceneName)
        {
            case "Village":
                gameObject.transform.position = sceneInfo.beforeSceneName == "MainMenu"
                    ? new Vector3(-7, 0, 0)
                    : new Vector3(7, 0, 0);
                break;
            case "DungeonA":
                gameObject.transform.position = sceneInfo.beforeSceneName == "Village"
                    ? new Vector3(-7, 0, 0)
                    : new Vector3(7, 0, 0);
                break;
            case "DungeonB":
                gameObject.transform.position = new Vector3(-7, 0, 0);
                break;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Portal") && Input.GetKeyDown(KeyCode.UpArrow))
        {
            portalManager.ChangeScene(col.name);    
        }
    }
}
