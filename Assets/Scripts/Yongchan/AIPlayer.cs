
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class AIPlayer : MonoBehaviour
{
   public string idName;
   public SceneInfo sceneinfo;
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
         _mobSpawnManager.curMob--;
         _mobSpawnManager.count--;
      }
   }

   public void MobDie()
   {
      gameObject.SetActive(false);
   }

   public void OnEnable()
   {
      Init();
   }

   void Init()
   {
      
   }
}
