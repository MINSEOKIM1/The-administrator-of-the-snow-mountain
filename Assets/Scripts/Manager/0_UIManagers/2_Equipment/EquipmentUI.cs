using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    [SerializeField] private List<Image> itemImages;
    [SerializeField] private TMP_Text totalInfo;

    [SerializeField] private Image selectedItemImage;
    [SerializeField] private TMP_Text selectedItemName;
    [SerializeField] private TMP_Text selectedItemDescription;

    public EquipmentItemInfo selectedItem;

    private Equipment Equipment
    {
        get => GameManager.Instance.PlayerDataManager.equipment;
        set => Equipment = value;
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < itemImages.Count; i++)
        {
            try
            {
                var item = GameManager.Instance.PlayerDataManager.equipment.items[i];
                if (item == null)
                {
                    itemImages[i].gameObject.SetActive(false);
                }
                else
                {
                    itemImages[i].gameObject.SetActive(true);
                    itemImages[i].sprite = item.itemIcon;
                }
            }
            catch (NullReferenceException e)
            {
                return;
            }
        }
        

        string total = "";
        float atk = 0;
        float def = 0;
        float hp = 0;
        float mp = 0;
        float hpIncRate = 0;
        float mpIncRate = 0;
        float stance = 0;

        total += "ATK : " + GameManager.Instance.PlayerDataManager.atk + "\n";
        total += "DEF : " + GameManager.Instance.PlayerDataManager.def + "\n";
        total += "HP : " + GameManager.Instance.PlayerDataManager.maxHp + "\n";
        total += "MP : " + GameManager.Instance.PlayerDataManager.maxMp + "\n";
        total += "HP 회복 : " + GameManager.Instance.PlayerDataManager.hpIncRate + "\n";
        total += "MP 회복 : " + GameManager.Instance.PlayerDataManager.mpIncRate + "\n";
        total += "STANCE : " + (GameManager.Instance.PlayerDataManager.stance*100) + "%\n";
        total += "공격속도 : " + (GameManager.Instance.PlayerDataManager.attackSpeed) + "\n";

        totalInfo.text = total;
    }

    public void SelectItem(int index)
    {
        if (index < 0)
        {
            selectedItem = null;
            selectedItemImage.color = new Color(0, 0, 0, 0);
            selectedItemName.text = "";
            selectedItemDescription.text = "";
        }
        else
        {
            selectedItem = Equipment.items[index];

            if (selectedItem == null)
            {
                SelectItem(-1);
                return;
            }
            selectedItemImage.color = new Color(1, 1, 1, 1);
            selectedItemImage.sprite = selectedItem.itemIcon;
            selectedItemName.text = selectedItem.itemName;
            var ii = selectedItem;
            string total = "\n\n";
            if (ii.atk != 0) total += "ATK +" + ii.atk + "\n";
            if (ii.def != 0) total += "DEF +" + ii.def + "\n";
            if (ii.hp != 0) total += "HP +" + ii.hp + "\n";
            if (ii.mp != 0) total += "MP +" + ii.mp + "\n";
            if (ii.hpIncRate != 0) total += "HP 회복 +" + ii.hpIncRate + "\n";
            if (ii.mpIncRate != 0) total += "MP 회복 +" + ii.mpIncRate + "\n";
            if (ii.stance != 0) total += "STANCE +" + (ii.stance * 100) + "%\n";
            if (ii.atkSpeed != 0) total += "공격속도 :" + ii.atkSpeed + "\n";
            selectedItemDescription.text = selectedItem.itemDescription + total;
        }
    }
}
