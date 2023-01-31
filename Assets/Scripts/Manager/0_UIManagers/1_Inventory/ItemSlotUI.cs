using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Tooltip("아이템 아이콘 이미지")]
    [SerializeField] private Image _iconImage;

    [Tooltip("아이템 개수 텍스트")]
    [SerializeField] private Text _amountText;

    [Tooltip("슬롯이 포커스될 때 나타나는 하이라이트 이미지")]
    [SerializeField] private Image _highlightImage;

    [Space]
    [Tooltip("하이라이트 이미지 알파 값")]
    [SerializeField] private float _highlightAlpha = 0.5f;

    [Tooltip("하이라이트 소요 시간")]
    [SerializeField] private float _highlightFadeDuration = 0.2f;


    /// <summary> 슬롯의 인덱스 </summary>
    public int Index { get; private set; }

    /// <summary> 슬롯이 아이템을 보유하고 있는지 여부 </summary>
    public bool HasItem => _iconImage.sprite != null;

    /// <summary> 접근 가능한 슬롯인지 여부 </summary>
    public bool IsAccessible => _isAccessibleSlot && _isAccessibleItem;

    public RectTransform SlotRect => _slotRect;
    public RectTransform IconRect => _iconRect;


    private InventoryUI _inventoryUI;

    private RectTransform _slotRect;
    private RectTransform _iconRect;
    private RectTransform _highlightRect;

    private GameObject _iconGo;
    private GameObject _textGo;
    private GameObject _highlightGo;

    private Image _slotImage;

// 현재 하이라이트 알파값
    private float _currentHLAlpha = 0f;

    private bool _isAccessibleSlot = true; // 슬롯 접근가능 여부
    private bool _isAccessibleItem = true; // 아이템 접근가능 여부

    /// <summary> 비활성화된 슬롯의 색상 </summary>
    private static readonly Color InaccessibleSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    /// <summary> 비활성화된 아이콘 색상 </summary>
    private static readonly Color InaccessibleIconColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    
    
    private void ShowIcon() => _iconGo.SetActive(true);
    private void HideIcon() => _iconGo.SetActive(false);

    private void ShowText() => _textGo.SetActive(true);
    private void HideText() => _textGo.SetActive(false);

    public void SetSlotIndex(int index) => Index = index;

    /// <summary> 슬롯 자체의 활성화/비활성화 여부 설정 </summary>
    public void SetSlotAccessibleState(bool value)
    {
        // 중복 처리는 지양
        if (_isAccessibleSlot == value) return;

        if (value)
        {
            _slotImage.color = Color.black;
        }
        else
        {
            _slotImage.color = InaccessibleSlotColor;
            HideIcon();
            HideText();
        }

        _isAccessibleSlot = value;
    }

    /// <summary> 아이템 활성화/비활성화 여부 설정 </summary>
    public void SetItemAccessibleState(bool value)
    {
        if(_isAccessibleItem == value) return;

        if (value)
        {
            _iconImage.color = Color.white;
            _amountText.color = Color.white;
        }
        else
        {
            _iconImage.color  = InaccessibleIconColor;
            _amountText.color = InaccessibleIconColor;
        }

        _isAccessibleItem = value;
    }

    /// <summary> 다른 슬롯과 아이템 아이콘 교환 </summary>
    public void SwapOrMoveIcon(ItemSlotUI other)
    {
        if (other == null) return;
        if (other == this) return; // 자기 자신과 교환 불가
        if (!this.IsAccessible) return;
        if (!other.IsAccessible) return;

        var temp = _iconImage.sprite;

        // 1. 대상에 아이템이 있는 경우 : 교환
        if (other.HasItem) SetItem(other._iconImage.sprite);

        // 2. 없는 경우 : 이동
        else RemoveItem();

        other.SetItem(temp);
    }

    /// <summary> 슬롯에 아이템 등록 </summary>
    public void SetItem(Sprite itemSprite)
    {
        if (itemSprite != null)
        {
            _iconImage.sprite = itemSprite;
            ShowIcon();
        }
        else
        {
            RemoveItem();
        }
    }

    /// <summary> 슬롯에서 아이템 제거 </summary>
    public void RemoveItem()
    {
        _iconImage.sprite = null;
        HideIcon();
        HideText();
    }

    /// <summary> 아이템 이미지 투명도 설정 </summary>
    public void SetIconAlpha(float alpha)
    {
        _iconImage.color = new Color(
            _iconImage.color.r, _iconImage.color.g, _iconImage.color.b, alpha
        );
    }

    /// <summary> 아이템 개수 텍스트 설정(amount가 1 이하일 경우 텍스트 미표시) </summary>
    public void SetItemAmount(int amount)
    {
        if (HasItem && amount > 1)
            ShowText();
        else
            HideText();

        _amountText.text = amount.ToString();
    }
}
