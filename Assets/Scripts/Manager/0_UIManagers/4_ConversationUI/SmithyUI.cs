using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmithyUI : MonoBehaviour
{
    public GameObject slotUI;
    public RectTransform tmpRectTransform;
    public ItemInfo selectedItem;
    public Image selectedItemImage;
    public TMP_Text selectedItemName, selectedItemDescription;

    public RectTransform menuList, ingredientList;

    public EquipmentItemInfo[] itemsInMenu;
    public List<CookSlotUI> menuSlots;
    public List<CookSlotUI> ingredientSlots;

    public Vector2 menuBeginPos;
    public Vector2 ingredientBeginPos;
    public float ingredientBeginHeight;
    public float slotMargin;

    public TMP_Text progressText;
    
    private void Start()
    {
        selectedItemImage.color = new Color(1, 1, 1, 0);
        InitMenu();
    }
    
    private void InitMenu()
    {
        Vector2 curPos = menuBeginPos;
        float slotHeight = slotUI.GetComponent<RectTransform>().sizeDelta.y;
        int slotIndex = 0;
        
        foreach (var i in itemsInMenu)
        {
            var slot = CloneMenuSlot();
            var slotUI = slot.GetComponent<CookSlotUI>();
            slotUI.SetItem(i, slotIndex++);
            menuSlots.Add(slotUI);
            slot.pivot = new Vector2(0.5f, 1);
            slot.anchoredPosition = curPos;
            slot.localScale = Vector3.one;
            slot.gameObject.SetActive(true);
            slot.gameObject.name = "slot" + (slotIndex - 1);
            slotUI.setItemEvent.AddListener(() => SelectItem(slotUI.index));
            curPos.Set(curPos.x, curPos.y - (slotHeight + slotMargin));
        }

        menuList.sizeDelta = new Vector2(menuList.sizeDelta.x, menuList.sizeDelta.y * (slotIndex));
    }

    private void SetIngredient()
    {
        foreach (var i in ingredientSlots)
        {
            Destroy(i.gameObject);
        }

        ingredientList.sizeDelta = new Vector2(ingredientList.sizeDelta.x, ingredientBeginHeight);
        
        ingredientSlots = new List<CookSlotUI>();
        
        Vector2 curPos = ingredientBeginPos;
        float slotHeight = slotUI.GetComponent<RectTransform>().sizeDelta.y;
        int slotIndex = 0;
        
        foreach (var i in ((EquipmentItemInfo) selectedItem).IngredientCountPairs)
        {
            var slot = CloneIngredientSlot();
            var slotUI = slot.GetComponent<CookSlotUI>();
            slotUI.SetItem(i.item, slotIndex++, i.item.itemName + " x " + i.count);
            ingredientSlots.Add(slotUI);
            slot.pivot = new Vector2(0.5f, 1);
            slot.anchoredPosition = curPos;
            slot.localScale = Vector3.one;
            slot.gameObject.SetActive(true);
            slot.gameObject.name = "ingredient " + (slotIndex - 1);
            curPos.Set(curPos.x, curPos.y - (slotHeight + slotMargin));
        }

        ingredientList.sizeDelta = new Vector2(ingredientList.sizeDelta.x, ingredientBeginHeight * (slotIndex));
    }

    RectTransform CloneMenuSlot()
    {
        var slot = Instantiate(slotUI);
        slot.transform.SetParent(menuList);
        return slot.GetComponent<RectTransform>();
    }
    
    RectTransform CloneIngredientSlot()
    {
        var slot = Instantiate(slotUI);
        slot.transform.SetParent(ingredientList);
        return slot.GetComponent<RectTransform>();
    }

    public void SelectItem(int index)
    {
        Debug.Log("index : " + index);
        selectedItem = menuSlots[index].item;
        selectedItemImage.color = new Color(1, 1, 1, 1);
        selectedItemImage.sprite = selectedItem.itemIcon;
        selectedItemName.text = selectedItem.itemName;
        var ii = ((EquipmentItemInfo)selectedItem);
        string total = "\n\n";
        if (GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart] != null)
        {
            if (ii.atk != 0)
                total += "ATK " + GameManager.Instance.PlayerDataManager.atk + "->" +
                         (GameManager.Instance.PlayerDataManager.atk
                          - GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart].atk
                          + ii.atk) + "\n";
            if (ii.def != 0)
                total += "DEF " + GameManager.Instance.PlayerDataManager.def + "->" +
                         (GameManager.Instance.PlayerDataManager.def
                          - GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart].def
                          + ii.def) + "\n";
            if (ii.hp != 0)
                total += "HP " + GameManager.Instance.PlayerDataManager.maxHp + "->" +
                         (GameManager.Instance.PlayerDataManager.maxHp
                          - GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart].hp
                          + ii.hp) + "\n";
            if (ii.mp != 0)
                total += "MP " + GameManager.Instance.PlayerDataManager.maxMp + "->" +
                         (GameManager.Instance.PlayerDataManager.maxMp
                          - GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart].mp
                          + ii.mp) + "\n";
            if (ii.hpIncRate != 0)
                total += "HP 회복 " + GameManager.Instance.PlayerDataManager.hpIncRate + "->" +
                         (GameManager.Instance.PlayerDataManager.hpIncRate
                          - GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart].hpIncRate
                          + ii.hpIncRate) + "\n";
            if (ii.mpIncRate != 0)
                total += "MP 회복 " + GameManager.Instance.PlayerDataManager.mpIncRate + "->" +
                         (GameManager.Instance.PlayerDataManager.mpIncRate
                          - GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart].mpIncRate
                          + ii.mpIncRate) + "\n";
            if (ii.stance != 0)
                total += "STANCE " + GameManager.Instance.PlayerDataManager.stance + "->" +
                         (GameManager.Instance.PlayerDataManager.stance
                          - GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart].stance
                          + ii.stance) + "\n";
            if (ii.atkSpeed != 0)
                total += "공격속도 " + GameManager.Instance.PlayerDataManager.attackSpeed + "->" +
                         (GameManager.Instance.PlayerDataManager.attackSpeed
                          - GameManager.Instance.PlayerDataManager.equipment.items[ii.equipmentPart].atkSpeed
                          + ii.atkSpeed) + "\n";
        }
        else
        {
            if (ii.atk != 0)
                total += "ATK " + GameManager.Instance.PlayerDataManager.atk + "->" +
                         (GameManager.Instance.PlayerDataManager.atk
                          + ii.atk) + "\n";
            if (ii.def != 0)
                total += "DEF " + GameManager.Instance.PlayerDataManager.def + "->" +
                         (GameManager.Instance.PlayerDataManager.def
                          + ii.def) + "\n";
            if (ii.hp != 0)
                total += "HP " + GameManager.Instance.PlayerDataManager.maxHp + "->" +
                         (GameManager.Instance.PlayerDataManager.maxHp
                          + ii.hp) + "\n";
            if (ii.mp != 0)
                total += "MP " + GameManager.Instance.PlayerDataManager.maxMp + "->" +
                         (GameManager.Instance.PlayerDataManager.maxMp
                          + ii.mp) + "\n";
            if (ii.hpIncRate != 0)
                total += "HP 회복 " + GameManager.Instance.PlayerDataManager.hpIncRate + "->" +
                         (GameManager.Instance.PlayerDataManager.hpIncRate
                          + ii.hpIncRate) + "\n";
            if (ii.mpIncRate != 0)
                total += "MP 회복 " + GameManager.Instance.PlayerDataManager.mpIncRate + "->" +
                         (GameManager.Instance.PlayerDataManager.mpIncRate
                          + ii.mpIncRate) + "\n";
            if (ii.stance != 0)
                total += "STANCE " + GameManager.Instance.PlayerDataManager.stance + "->" +
                         (GameManager.Instance.PlayerDataManager.stance
                          + ii.stance) + "\n";
            if (ii.atkSpeed != 0)
                total += "공격속도 " + GameManager.Instance.PlayerDataManager.attackSpeed + "->" +
                         (GameManager.Instance.PlayerDataManager.attackSpeed
                          + ii.atkSpeed) + "\n";
        }
    

        selectedItemDescription.text = selectedItem.itemDescription + total;
        SetIngredient();
    }

    public void OnMake()
    {
        if (selectedItem == null) return;
        else
        {
            foreach (var i in ((EquipmentItemInfo)selectedItem).IngredientCountPairs)
            {
                if (GameManager.Instance.PlayerDataManager.inventory.CountItem(i.item) < i.count)
                {
                    // FAIL! -- there is no sufficient ingredients...
                    Debug.Log("there is no sufficient ingredients...");
                    StopCoroutine("PopMessage");
                    StartCoroutine(PopMessage("충분한 재료가 없습니다..."));
                    throw new NotImplementedException();
                }
            }

            if (GameManager.Instance.PlayerDataManager.inventory.AddItem(selectedItem, 1))
            {
                StopCoroutine("PopMessage");
                StartCoroutine(PopMessage(selectedItem.itemName + " 제작 완료!"));
                foreach (var i in ((EquipmentItemInfo)selectedItem).IngredientCountPairs)
                {
                    GameManager.Instance.PlayerDataManager.inventory.DeleteItem(i.item, i.count);
                }
            }
            else
            {
                // FAIL! -- there is no space for storing the result item...
                Debug.Log("there is no space for storing the result item...");
                StopCoroutine("PopMessage");
                StartCoroutine(PopMessage("인벤토리에 남은 공간이 없습니다..."));
                throw new NotImplementedException();
            }
        }
    }

    public void OnCancel()
    {
        GameManager.Instance.UIManager.ConservationUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    IEnumerator PopMessage(string message)
    {
        progressText.text = message;
        progressText.alpha = 1;
        while (progressText.alpha > 0)
        {
            progressText.alpha -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
