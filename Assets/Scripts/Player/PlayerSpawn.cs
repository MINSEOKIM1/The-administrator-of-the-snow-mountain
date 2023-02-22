using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class PlayerSpawn : MonoBehaviour
{
    private MapManager sceneInfo
    {
        get => GameManager.Instance.MapManager;
    }
    [SerializeField] private PortalManager portalManager;

    // # player spawn point setting by sceneInfo (currentScene and beforeScene)
    private void Start()
    {
        Debug.Log(transform.position);
        Debug.Log(portalManager);
        if (portalManager != null)
        gameObject.transform.position =
            portalManager.SpawnPosition(sceneInfo.currentSceneName, sceneInfo.beforeSceneName);
        if (GameManager.Instance.PlayerDataManager.loadPosFromData)
        {
            GameManager.Instance.PlayerDataManager.loadPosFromData = false;
            transform.position = GameManager.Instance.PlayerDataManager.loadPos;
        }
    }
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Portal") && Input.GetKey(KeyCode.UpArrow))
        {
            portalManager.ChangeScene(col.name);    
        }
    }
}
