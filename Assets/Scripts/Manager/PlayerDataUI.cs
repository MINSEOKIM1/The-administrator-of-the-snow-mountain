using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataUI : MonoBehaviour
{
    public Slider hpBar, mpBar;
    public void Update()
    {
        hpBar.value = GameManager.Instance.PlayerDataManager.hp / GameManager.Instance.PlayerDataManager.maxHp;
        mpBar.value = GameManager.Instance.PlayerDataManager.mp / GameManager.Instance.PlayerDataManager.maxMp;
    }
}
