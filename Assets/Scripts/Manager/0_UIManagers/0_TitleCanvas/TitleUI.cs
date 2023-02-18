using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public RectTransform selectionMark;
    public Button[] buttons;
    public GameObject title;

    public Vector3 selectionPos, targetPos;

    public float transitionSpeed;

    public int titleOption;

    public TMP_Text[] dataTexts;

    /**
     * 0 : main menu
     * 1 : data panel
     * 2 : credit
     */
    public GameObject[] titlePanels;

    public bool newGame;

    public int selection = 0;

    private String _loadScene;

    public void DataTextUpdate()
    {
        for(int i = 0; i<dataTexts.Length; i++)
        {
            dataTexts[i].text = "PLAYTIME. " + GameManager.Instance.DataManager.dataPacks[i].playTime;
        }
    }

    private void Start()
    {
        selectionPos = selectionMark.transform.position;
        targetPos = buttons[0].GetComponent<RectTransform>().anchoredPosition;;
        selectionMark.position = targetPos;
    }

    private void Update()
    {
        selectionPos = Vector3.Lerp(selectionPos, targetPos, transitionSpeed * Time.deltaTime);
        selectionMark.anchoredPosition = selectionPos;
    }

    public void OnGameStart(int idx)
    {
        if (newGame)
        {
            NewGameStart(idx);
        }
        else
        {
            LoadGameStart(idx);
        }
    }

    public void GameStart()
    {
        GameManager.Instance.UIManager.TitleUI.gameObject.SetActive(false);
        GameManager.Instance.UIManager.PlayerDataUI.SetActive(true);
        GameManager.Instance.GameSceneManager.LoadScene(GameManager.Instance.MapManager.currentSceneName);
        GameManager.Instance.MapManager.gameStart = true;
    }
    public void NewGameStart(int idx)
    {
        GameManager.Instance.DataManager.SaveCurrentState(idx);
        GameManager.Instance.DataManager.SaveData(idx);
        GameManager.Instance.DataManager.currentDataSlot = idx;
        GameStart();
    }

    public void LoadGameStart(int idx)
    {
        GameManager.Instance.DataManager.LoadToGame(idx);
        GameManager.Instance.DataManager.currentDataSlot = idx;
        GameStart();
    }

    public void GoMain()
    {
        for (int i = 0; i < titlePanels.Length; i++)
        {
            titlePanels[i].SetActive(false);
        }
        titlePanels[0].SetActive(true);
        titleOption = 0;
    }
    
    public void GoData()
    {
        for (int i = 0; i < titlePanels.Length; i++)
        {
            titlePanels[i].SetActive(false);
        }
        titlePanels[1].SetActive(true);
        DataTextUpdate();
        titleOption = 1;
    }
    
    public void GoCredit()
    {
        for (int i = 0; i < titlePanels.Length; i++)
        {
            titlePanels[i].SetActive(false);
        }
        titlePanels[2].SetActive(true);
        titleOption = 2;
    }

    public void ChangeSelection(InputAction.CallbackContext value)
    {
        if (!value.started || titleOption != 0) return;
        int num = (int) value.ReadValue<float>();
        
        ChangeSelection(selection + num);
    }

    public void NewGame()
    {
        newGame = true;
        GoData();
    }

    public void LoadGame()
    {
        newGame = false;
        GoData();
    }

    public void QuitGame()
    {
        Debug.Log("!");
    }
    public void Select(InputAction.CallbackContext value)
    {
        if (value.started && titleOption == 0)
        {
            switch (selection)
            {
                case 0:
                    NewGame();
                    break;
                case 1:
                    LoadGame();
                    break;
                case 2:
                    QuitGame();
                    break;
                case 3:
                    GoCredit();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void ChangeSelection(int num)
    {
        GameManager.Instance.AudioManager.PlaySfx(0);
        buttons[selection].GetComponent<ButtonTest>().SetHighlightOff();
        
        selection = num;
        selection = Mathf.Clamp(selection, 0, 3);
        
        buttons[selection].GetComponent<ButtonTest>().SetHighlightOn();

        targetPos = buttons[selection].GetComponent<RectTransform>().anchoredPosition;
    }
}
