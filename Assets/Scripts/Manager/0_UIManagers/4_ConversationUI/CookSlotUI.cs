using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CookSlotUI : MonoBehaviour
{
    public ItemInfo item;
    public int index;
    public TMP_Text itemName;
    public Image itemImage;

    public UnityEvent setItemEvent;

    public void SetItem(ItemInfo item, int index)
    {
        this.item = item;
        this.index = index;
        itemName.text = item.itemName;
        itemImage.sprite = item.itemIcon;

        GetComponent<Button>().onClick.AddListener(() => OnClick());
    }

    public void SetItem(ItemInfo item, int index, string itemName)
    {
        SetItem(item, index);
        this.itemName.text = itemName;
    }

    public void OnClick()
    {
        setItemEvent.Invoke();
    }
}
