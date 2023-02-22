using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataUI : MonoBehaviour
{
    public Slider hpBar, mpBar, fullnessBar, expBar;
    public TMP_Text hpText, mpText, fullnessText, expText, lvText;
    public float changeRate;

    public GameObject guide;

    private void OnEnable()
    {
        if (GameManager.Instance.PlayerDataManager.tutorial < 3)
        {
            guide.SetActive(true);
        }
    }

    public PlayerDataManager PlayerDataManager
    {
        get => GameManager.Instance.PlayerDataManager;
        set => PlayerDataManager = value;
    }
    public void Update()
    {
        if (guide.activeSelf && GameManager.Instance.PlayerDataManager.tutorial >= 3)
        {
            guide.SetActive(false);
        }
        hpBar.value = Mathf.Lerp(
            hpBar.value, 
            PlayerDataManager.hp / PlayerDataManager.maxHp, 
            changeRate * Time.deltaTime);
        mpBar.value = Mathf.Lerp(
            mpBar.value, 
            PlayerDataManager.mp / PlayerDataManager.maxMp, 
            changeRate* Time.deltaTime);
        fullnessBar.value = Mathf.Lerp(
            fullnessBar.value, 
            PlayerDataManager.saturation / PlayerDataManager.maxSaturation, 
            changeRate* Time.deltaTime);
        expBar.value = Mathf.Lerp(
            expBar.value, 
            PlayerDataManager.exp / PlayerDataManager.maxExp, 
            changeRate* Time.deltaTime);

        hpText.text = "" + (int)PlayerDataManager.hp + " / " + (int)PlayerDataManager.maxHp;
        mpText.text = "" + (int)PlayerDataManager.mp + " / " + (int)PlayerDataManager.maxMp;
        fullnessText.text = "" + (int)PlayerDataManager.saturation + " / " + (int)PlayerDataManager.maxSaturation;
        expText.text = "" + (int)PlayerDataManager.exp + " / " + (int)PlayerDataManager.maxExp;
        lvText.text = "LEVEL. " + PlayerDataManager.level;
    }
}
