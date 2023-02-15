using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CookUI : MonoBehaviour
{
    public GameObject slotUI;
    public RectTransform tmpRectTransform;
    public ItemInfo selectedItem;
    public Image selectedItemImage;
    public TMP_Text selectedItemName, selectedItemDescription;

    public RectTransform menuList, ingredientList;

    public ConsumableItemInfo[] itemsInMenu;
    public List<CookSlotUI> menuSlots;
    public List<CookSlotUI> ingredientSlots;

    public Vector2 menuBeginPos;
    public Vector2 ingredientBeginPos;
    public float ingredientBeginHeight;
    public float slotMargin;

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
        
        foreach (var i in ((ConsumableItemInfo) selectedItem).IngredientCountPairs)
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
        selectedItemDescription.text = selectedItem.itemDescription;
        SetIngredient();
    }

    public void OnMake()
    {
        if (selectedItem == null) return;
        else
        {
            foreach (var i in ((ConsumableItemInfo)selectedItem).IngredientCountPairs)
            {
                if (GameManager.Instance.PlayerDataManager.inventory.CountItem(i.item) < i.count)
                {
                    // FAIL! -- there is no sufficient ingredients...
                    throw new NotImplementedException();
                }
            }

            if (GameManager.Instance.PlayerDataManager.inventory.AddItem(selectedItem, 1))
            {
                foreach (var i in ((ConsumableItemInfo)selectedItem).IngredientCountPairs)
                {
                    GameManager.Instance.PlayerDataManager.inventory.DeleteItem(i.item, i.count);
                }
            }
            else
            {
                // FAIL! -- there is no space for storing the result item...
                throw new NotImplementedException();
            }
        }
    }

    public void OnCancel()
    {
        GameManager.Instance.UIManager.ConservationUI.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
