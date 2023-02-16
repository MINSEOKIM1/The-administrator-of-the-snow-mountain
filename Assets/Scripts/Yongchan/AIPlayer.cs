
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class AIPlayer : MonoBehaviour
{
   public string idName;

   public MapManager sceneinfo
   {
      get => GameManager.Instance.MapManager;
   }
   private MobSpawnManager _mobSpawnManager;

   private void Awake()
   {
      _mobSpawnManager = MobSpawnManager.instance;
   }

   public void OnTriggerEnter2D(Collider2D col)
   {
      if (col.CompareTag("Player"))
      {
         gameObject.SetActive(false);
         _mobSpawnManager.MobDie(_mobSpawnManager.sceneinfo.currentSceneName, idName);
      }
   }

   public void MobDie()
   {
      gameObject.SetActive(false);
      _mobSpawnManager.MobDie(_mobSpawnManager.sceneinfo.currentSceneName, idName);
   }

   public void OnEnable()
   {
      Init();
   }

   void Init()
   {
      var mon = GetComponent<Monster>();
      mon._target = null;
      mon.hpbar.value = 1;
      mon.mpbar.value = 1;
      mon.hp = mon.entityInfo.maxHp;
      mon.mp = mon.entityInfo.maxMp;
      mon.isDie = false;
      StartCoroutine(mon.SpawnInit());
   }
}
