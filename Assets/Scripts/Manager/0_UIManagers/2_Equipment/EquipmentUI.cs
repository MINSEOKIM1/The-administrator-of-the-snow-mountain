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

    private void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < itemImages.Count; i++)
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
        

        string total = "";
        float atk = 0;
        float def = 0;
        float hp = 0;
        float mp = 0;
        float hpIncRate = 0;
        float mpIncRate = 0;
        float stance = 0;
        
        for (int i = 0; i < Equipment.items.Length; i++)
        {

            if (Equipment.items[i] != null)
            {
                atk += Equipment.items[i].atk;
                def += Equipment.items[i].def;
                hp += Equipment.items[i].hp;
                mp += Equipment.items[i].mp;
                hpIncRate += Equipment.items[i].hpIncRate;
                mpIncRate += Equipment.items[i].mpIncRate;
                stance += Equipment.items[i].stance;
            }
        }

        if (atk > 0) total += "ATK +" + atk + "\n";
        if (def > 0) total += "DEF +" + def + "\n";
        if (hp > 0) total += "HP +" + hp + "\n";
        if (mp > 0) total += "MP +" + mp + "\n";
        if (hpIncRate > 0) total += "HP 회복 +" + hpIncRate + "\n";
        if (mpIncRate > 0) total += "MP 회복 +" + mpIncRate + "\n";
        if (stance > 0) total += "STANCE +" + stance + "\n";

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
            selectedItemDescription.text = selectedItem.itemDescription;
        }
    }
}
