using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Tooltip("아이템 아이콘 이미지")]
    [SerializeField] private Image _iconImage;

    [Tooltip("아이템 개수 텍스트")]
    [SerializeField] private TMP_Text _amountText;

    public ItemSlot item;

    /// <summary> 슬롯의 인덱스 </summary>
    public int index;

    /// <summary> 슬롯이 아이템을 보유하고 있는지 여부 </summary>
    public bool HasItem => item != null && item.count != 0;

    public void SetSlotIndex(int index) => this.index = index;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (HasItem)
        {
            GameManager.Instance.UIManager.InventoryUI.selectedItemImage.sprite = item.item.itemIcon;
            GameManager.Instance.UIManager.InventoryUI.selectedItemName.text = item.item.itemName;
            GameManager.Instance.UIManager.InventoryUI.selectedItemDescription.text = item.item.itemDescription;
        }
    }

    private void Update()
    {
        item = GameManager.Instance.PlayerDataManager.inventory.items[index];

        if (item != null && item.count != 0)
        {
            _iconImage.color = new Color(1, 1, 1, 1);
            _iconImage.sprite = item.item.itemIcon;
            _amountText.text = "" + item.count;
        }
        else
        {
            _iconImage.color = new Color(1, 1, 1, 0);
            _amountText.text = "";
        }
    }
}
