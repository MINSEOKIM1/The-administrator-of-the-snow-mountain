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
    
    public PlayerDataManager PlayerDataManager
    {
        get => GameManager.Instance.PlayerDataManager;
        set => PlayerDataManager = value;
    }
    public void Update()
    {
        hpBar.value = Mathf.Lerp(
            hpBar.value, 
            PlayerDataManager.hp / PlayerDataManager.maxHp, 
            changeRate);
        mpBar.value = Mathf.Lerp(
            mpBar.value, 
            PlayerDataManager.mp / PlayerDataManager.maxMp, 
            changeRate);
        fullnessBar.value = Mathf.Lerp(
            fullnessBar.value, 
            PlayerDataManager.saturation / PlayerDataManager.maxSaturation, 
            changeRate);
        expBar.value = Mathf.Lerp(
            expBar.value, 
            PlayerDataManager.exp / PlayerDataManager.maxExp, 
            changeRate);

        hpText.text = "" + (int)PlayerDataManager.hp + " / " + (int)PlayerDataManager.maxHp;
        mpText.text = "" + (int)PlayerDataManager.mp + " / " + (int)PlayerDataManager.maxMp;
        fullnessText.text = "" + (int)PlayerDataManager.saturation + " / " + (int)PlayerDataManager.maxSaturation;
        expText.text = "" + (int)PlayerDataManager.exp + " / " + (int)PlayerDataManager.maxExp;
        lvText.text = "LEVEL. " + PlayerDataManager.level;
    }
}
