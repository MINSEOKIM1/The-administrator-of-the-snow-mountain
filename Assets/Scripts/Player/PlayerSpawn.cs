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
        gameObject.transform.position =
            portalManager.SpawnPosition(sceneInfo.currentSceneName, sceneInfo.beforeSceneName);
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Portal") && Input.GetKeyDown(KeyCode.UpArrow))
        {
            portalManager.ChangeScene(col.name);    
        }
    }
}
