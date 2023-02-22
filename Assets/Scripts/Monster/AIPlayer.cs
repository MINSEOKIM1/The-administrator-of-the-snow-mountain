
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class AIPlayer : MonoBehaviour
{
   public string idName;

   public int agentIndex;
   
   public MapManager sceneinfo
   {
      get => GameManager.Instance.MapManager;
   }
   private MobSpawnManager _mobSpawnManager;

   private void Awake()
   {
      _mobSpawnManager = MobSpawnManager.instance;
   }

   public void MobDie()
   {
      if (_mobSpawnManager != null)
      {
         if (idName.Equals("Boss"))
         {
            _mobSpawnManager.BossDie(_mobSpawnManager.sceneinfo.currentSceneName);
            Invoke("SetActiveFalse", 0.2f);
            GameManager.Instance.PlayerDataManager.agentAvailable[agentIndex] = true;
         }
         else
         {
            _mobSpawnManager.MobDie(_mobSpawnManager.sceneinfo.currentSceneName, idName);
            Invoke("SetActiveFalse", 2);
         }
      }
   }

   void SetActiveFalse()
   {
      gameObject.SetActive(false);
   }

   public void OnEnable()
   {
      Init();
   }

   void Init()
   {
      Vector2 pos = transform.position;
      var aa = Physics2D.RaycastAll(transform.position, Vector2.down, 100);
      foreach (var a in aa)
      {
         if (a.collider.CompareTag("Ground"))
         {
            transform.position -=  (Vector3) ((Vector2) GetComponent<Monster>().footPos.position - a.point);
            break;
         }
      }
      
      var mon = GetComponent<Monster>();
      mon._target = null;
      mon.gameObject.layer = 8;
      mon.hpbar.value = 1;
      mon.mpbar.value = 1;
      mon.hp = mon.entityInfo.maxHp;
      mon.mp = mon.entityInfo.maxMp;
      mon.isDie = false;
      StartCoroutine(mon.SpawnInit());
   }
}
