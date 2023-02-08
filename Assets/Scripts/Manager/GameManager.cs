using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [field: SerializeField] public PlayerDataManager PlayerDataManager { get; private set; }
    [field: SerializeField] public UIManager UIManager { get; private set; }
    [field: SerializeField] public AudioManager AudioManager { get; private set; }
    [field: SerializeField] public EffectManager EffectManager { get; private set; }
    /*
    public GameStateManager GameStateManager { get; private set; }

    */

    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true, 60);
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        DontDestroyOnLoad(gameObject);
        
        PlayerDataManager = GetComponentInChildren<PlayerDataManager>();
        UIManager = GetComponentInChildren<UIManager>();
        AudioManager = GetComponentInChildren<AudioManager>();
        EffectManager = GetComponentInChildren<EffectManager>();
    }
}