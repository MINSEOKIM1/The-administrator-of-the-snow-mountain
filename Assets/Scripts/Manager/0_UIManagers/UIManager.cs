using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [field: SerializeField] public TitleUI TitleUI { get; private set; }
    [field: SerializeField] public InventoryUI InventoryUI { get; private set; }
    [field: SerializeField] public EquipmentUI EquipmentUI { get; private set; }
    [field: SerializeField] public ConversationUI ConservationUI { get; private set; }

    [field: SerializeField] public MapUI MapUI { get; private set; }
    [field: SerializeField] public GameObject PlayerDataUI { get; private set; }

    public GameObject fadeCanvas;
    public float fps;
    public TMP_Text fpsTEXT;

    [Tooltip("0: inventory\n1: equipment\n2:MapUI")]
    public GameObject[] uiCanvas;

    public TMP_Text[] messageText;
    public float[] messageTime;

    public GameObject optionWindow;
    public Slider[] soundSlider;

    public void SetVolume(int idx)
    {
        if (idx == 0) GameManager.Instance.AudioManager.bgmPlayer.volume = soundSlider[idx].value;
        if (idx == 1) GameManager.Instance.AudioManager.sfxPlayer.volume = soundSlider[idx].value;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GotoMain()
    {
        Time.timeScale = 1;
        optionWindow.SetActive(false);
        GameManager.Instance.MapManager.gameStart = false;
        GameManager.Instance.GameSceneManager.LoadScene("LogoScene");
    }

    public delegate void ESCAction();

    public event ESCAction ESCEvents;

    public Volume tmpa;
    public DepthOfField tmpb;
    
    private void Start()
    {
        tmpa.profile.TryGet<DepthOfField>(out tmpb);
        soundSlider[0].value = GameManager.Instance.AudioManager.bgmPlayer.volume;
        soundSlider[1].value = GameManager.Instance.AudioManager.sfxPlayer.volume;
        optionWindow.SetActive(false);
        // ToEquipmentUI();
        GameManager.Instance.UIManager.ESCEvents += () =>
        {
            bool ok = false;
            for (int i = 0; i < uiCanvas.Length; i++)
            {
                if (uiCanvas[i].activeSelf)
                {
                    ok = true;
                    uiCanvas[i].SetActive(false);
                }
            }

            if (!ok && !ConservationUI.gameObject.activeSelf && GameManager.Instance.MapManager.gameStart)
            {
                if (optionWindow.activeSelf)
                {
                    optionWindow.SetActive(false);
                    tmpb.active = false;
                    Time.timeScale = 1;
                }
                else
                {
                    optionWindow.SetActive(true);
                    tmpb.active = true;
                    Time.timeScale = 0;
                }
            }
            
            
        };
    }

    private void Update()
    {
        for (int i = 0; i < messageText.Length; i++)
        {
            messageTime[i] -= Time.deltaTime;
            if (messageTime[i] < 0) messageText[i].alpha -= Time.deltaTime;
        }
    }


    public void ToInventoryUI()
    {
        AccessUICanvas(0);
    }
    
    public void ToEquipmentUI()
    {
        AccessUICanvas(1);
    }
    
    public void ToMapUI()
    {
        AccessUICanvas(2);
    }

    public void AccessUICanvas(int index)
    {
        if (!GameManager.Instance.MapManager.gameStart || !GameManager.Instance.PlayerDataManager.canControl) return;
        for (int i = 0; i < uiCanvas.Length; i++)
        {
            if (i == index)
            {
                uiCanvas[i].SetActive(!uiCanvas[i].activeSelf);
            }
            else
            {
                uiCanvas[i].SetActive(false);
            }
        }

        if (MapUI.gameObject.activeSelf == false)
        {
            MapUI.isAllocating = false;
        }
    }

    public void OnESC(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            ESCEvents.Invoke();
            Debug.Log(ESCEvents.Target);
        }
    }

    public void PopMessage(String message, float time)
    {
        for (int i = 0; i < messageText.Length; i++)
        {
            if (messageText[i].alpha > 0) continue;
            messageText[i].alpha = 1;
            messageTime[i] = time;
            messageText[i].text = message;
            return;
        }
    }
}
