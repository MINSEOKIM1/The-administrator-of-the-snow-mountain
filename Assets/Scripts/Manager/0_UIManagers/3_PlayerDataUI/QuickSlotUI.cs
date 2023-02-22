using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickSlotUI : MonoBehaviour
{
    public ConsumableItemInfo item;
    public TMP_Text textt;
    public Image imagee;

    private void Start()
    {
        SetItem(item);
    }

    private void Update()
    {
        /**
        int a = GameManager.Instance.PlayerDataManager.inventory.CountItem(item);
        if (item != null)
        {
            if (a > 0)
                textt.text = "" + GameManager.Instance.PlayerDataManager.inventory.CountItem(item);
            else
                textt.text = "";
        }
        */
    }

    public void OnValidate()
    {
        SetItem(item);
    }

    public void SetItem(ConsumableItemInfo item)
    {
        if (item != null)
        {
            this.item = item;
            imagee.sprite = item.itemIcon;
            textt.text = "" + GameManager.Instance.PlayerDataManager.inventory.CountItem(item);
            imagee.color = new Color(1, 1, 1, 1);
        }
        else
        {
            textt.text = "";
            imagee.color = new Color(1, 1, 1, 0);
        }
    }

    public void OnUse(InputAction.CallbackContext value)
    {
        if (value.started) Use();
    }
    public void Use()
    {
        if (item != null)
        {
            GameManager.Instance.UIManager.InventoryUI.SelectItem(item);
            GameManager.Instance.UIManager.InventoryUI.Use();

            int count = GameManager.Instance.PlayerDataManager.inventory.CountItem(item);
            if (count > 0)
            {
                textt.text = "" + GameManager.Instance.PlayerDataManager.inventory.CountItem(item);
            }
            else
            {
                imagee.color = new Color(1, 1, 1, 0);
                textt.text = "";
            }
        }
    }
}
