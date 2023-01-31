using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;// Required when using Event data.

public class ButtonTest : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private int num;
    private TMP_Text _text;
    public bool isHighlighted { get; private set; }

    private void Start()
    {
        _text = GetComponentInChildren<TMP_Text>();
        isHighlighted = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.UIManager.TitleUI.ChangeSelection(num);
    }

    public void SetHighlightOn()
    {
        isHighlighted = true;
        _text.color = Color.gray;
    }
    
    public void SetHighlightOff()
    {
        isHighlighted = false;
        _text.color = Color.black;
    }
}