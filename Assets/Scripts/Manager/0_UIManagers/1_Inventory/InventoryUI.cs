using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Options")]
    [Range(0, 20)]
    [SerializeField] private int _horizontalSlotCount = 8;  // 슬롯 가로 개수
    [Range(0, 10)]
    [SerializeField] private int _verticalSlotCount = 8;      // 슬롯 세로 개수
    [SerializeField] private float _slotMargin = 8f;          // 한 슬롯의 상하좌우 여백
    [SerializeField] private float _contentAreaPadding = 20f; // 인벤토리 영역의 내부 여백
    [Range(32, 128)]
    [SerializeField] private float _slotSize = 64f;      // 각 슬롯의 크기

    [Header("Connected Objects")]
    [SerializeField] private RectTransform _contentAreaRT; // 슬롯들이 위치할 영역
    [SerializeField] private GameObject _slotUiPrefab;     // 슬롯의 원본 프리팹

    private List<ItemSlotUI> _slotUIList;

    [SerializeField] private TMP_Text optionText;

    [SerializeField] private GameObject dropItemUI;
    [SerializeField] private Slider dropItemCountSlider;
    [SerializeField] private TMP_Text dropItemCountText;
    [SerializeField] private bool isDropping;

    public int selectedItemIndex;
    public Image selectedItemImage;
    public GameObject[] selectedItemOption;
    public TMP_Text selectedItemName, selectedItemDescription;

    private int _dragBeginSlotIndex, _dragEndSlotIndex;
    private bool _isItemDragging;

    private int _useOption;

    [SerializeField] private GraphicRaycaster graphic;
    [SerializeField] private Image dragImage;
    [SerializeField] private RectTransform dragImagePos;

#if UNITY_EDITOR
    [SerializeField] private bool __showPreview = false;

    [Range(0.01f, 1f)]
    [SerializeField] private float __previewAlpha = 0.1f;

    private List<GameObject> __previewSlotGoList = new List<GameObject>();
    private int __prevSlotCountPerLine;
    private int __prevSlotLineCount;
    private float __prevSlotSize;
    private float __prevSlotMargin;
    private float __prevContentPadding;
    private float __prevAlpha;
    private bool __prevShow = false;
    private bool __prevMouseReversed = false;

    private void Start()
    {
        InitSlots();
        selectedItemImage.sprite = null;
        selectedItemName.text = "";
        selectedItemDescription.text = "";
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;

        if (__showPreview && !__prevShow)
        {
            CreateSlots();
        }
        __prevShow = __showPreview;

        if (Unavailable())
        {
            ClearAll();
            return;
        }
        if (CountChanged())
        {
            ClearAll();
            CreateSlots();
            __prevSlotCountPerLine = _horizontalSlotCount;
            __prevSlotLineCount = _verticalSlotCount;
        }
        if (ValueChanged())
        {
            DrawGrid();
            __prevSlotSize = _slotSize;
            __prevSlotMargin = _slotMargin;
            __prevContentPadding = _contentAreaPadding;
        }
        if (AlphaChanged())
        {
            SetImageAlpha();
            __prevAlpha = __previewAlpha;
        }

        bool Unavailable()
        {
            return !__showPreview ||
                    _horizontalSlotCount < 1 ||
                    _verticalSlotCount < 1 ||
                    _slotSize <= 0f ||
                    _contentAreaRT == null ||
                    _slotUiPrefab == null;
        }
        bool CountChanged()
        {
            return _horizontalSlotCount != __prevSlotCountPerLine ||
                    _verticalSlotCount != __prevSlotLineCount;
        }
        bool ValueChanged()
        {
            return _slotSize != __prevSlotSize ||
                    _slotMargin != __prevSlotMargin ||
                    _contentAreaPadding != __prevContentPadding;
        }
        bool AlphaChanged()
        {
            return __previewAlpha != __prevAlpha;
        }
        void ClearAll()
        {
            foreach (var go in __previewSlotGoList)
            {
                Destroyer.Destroy(go);
            }
            __previewSlotGoList.Clear();
        }
        void CreateSlots()
        {
            int count = _horizontalSlotCount * _verticalSlotCount;
            __previewSlotGoList.Capacity = count;

            // 슬롯의 피벗은 Left Top으로 고정
            RectTransform slotPrefabRT = _slotUiPrefab.GetComponent<RectTransform>();
            slotPrefabRT.pivot = new Vector2(0f, 1f);

            for (int i = 0; i < count; i++)
            {
                GameObject slotGo = Instantiate(_slotUiPrefab);
                slotGo.transform.SetParent(_contentAreaRT.transform);
                slotGo.SetActive(true);
                slotGo.AddComponent<PreviewItemSlot>();

                slotGo.transform.localScale = Vector3.one; // 버그 해결

                HideGameObject(slotGo);

                __previewSlotGoList.Add(slotGo);
            }

            DrawGrid();
            SetImageAlpha();
        }
        void DrawGrid()
        {
            Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
            Vector2 curPos = beginPos;

            // Draw Slots
            int index = 0;
            for (int j = 0; j < _verticalSlotCount; j++)
            {
                for (int i = 0; i < _horizontalSlotCount; i++)
                {
                    GameObject slotGo = __previewSlotGoList[index++];
                    RectTransform slotRT = slotGo.GetComponent<RectTransform>();

                    slotRT.anchoredPosition = curPos;
                    slotRT.sizeDelta = new Vector2(_slotSize, _slotSize);
                    __previewSlotGoList.Add(slotGo);

                    // Next X
                    curPos.x += (_slotMargin + _slotSize);
                }

                // Next Line
                curPos.x = beginPos.x;
                curPos.y -= (_slotMargin + _slotSize);
            }
        }
        void HideGameObject(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;

            Transform tr = go.transform;
            for (int i = 0; i < tr.childCount; i++)
            {
                tr.GetChild(i).gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
        }
        void SetImageAlpha()
        {
            foreach (var go in __previewSlotGoList)
            {
                var images = go.GetComponentsInChildren<Image>();
                foreach (var img in images)
                {
                    img.color = new Color(img.color.r, img.color.g, img.color.b, __previewAlpha);
                    var outline = img.GetComponent<Outline>();
                    if (outline)
                        outline.effectColor = new Color(outline.effectColor.r, outline.effectColor.g, outline.effectColor.b, __previewAlpha);
                }
            }
        }
    }

    private class PreviewItemSlot : MonoBehaviour { }

    [UnityEditor.InitializeOnLoad]
    private static class Destroyer
    {
        private static Queue<GameObject> targetQueue = new Queue<GameObject>();

        static Destroyer()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                for (int i = 0; targetQueue.Count > 0 && i < 100000; i++)
                {
                    var next = targetQueue.Dequeue();
                    DestroyImmediate(next);
                }
            };
        }
        public static void Destroy(GameObject go) => targetQueue.Enqueue(go);
    }
#endif

    /// <summary> 지정된 개수만큼 슬롯 영역 내에 슬롯들 동적 생성 </summary>
    private void InitSlots()
    {
        // 슬롯 프리팹 설정
        _slotUiPrefab.TryGetComponent(out RectTransform slotRect);
        slotRect.sizeDelta = new Vector2(_slotSize, _slotSize);

        _slotUiPrefab.TryGetComponent(out ItemSlotUI itemSlot);
        if (itemSlot == null)
            _slotUiPrefab.AddComponent<ItemSlotUI>();

        _slotUiPrefab.SetActive(false);

        // --
        Vector2 beginPos = new Vector2(_contentAreaPadding, -_contentAreaPadding);
        Vector2 curPos = beginPos;

        _slotUIList = new List<ItemSlotUI>(_verticalSlotCount * _horizontalSlotCount);

        // 슬롯들 동적 생성
        for (int j = 0; j < _verticalSlotCount; j++)
        {
            for (int i = 0; i < _horizontalSlotCount; i++)
            {
                int slotIndex = (_horizontalSlotCount * j) + i;

                var slotRT = CloneSlot();
                slotRT.pivot = new Vector2(0f, 1f); // Left Top
                slotRT.anchoredPosition = curPos;
                slotRT.gameObject.SetActive(true);
                slotRT.gameObject.name = $"Item Slot [{slotIndex}]";
                slotRT.transform.localScale = Vector3.one;

                var slotUI = slotRT.GetComponent<ItemSlotUI>();
                slotUI.SetSlotIndex(slotIndex);
                _slotUIList.Add(slotUI);

                // Next X
                curPos.x += (_slotMargin + _slotSize);
            }

            // Next Line
            curPos.x = beginPos.x;
            curPos.y -= (_slotMargin + _slotSize);
        }

        // 슬롯 프리팹 - 프리팹이 아닌 경우 파괴
        if(_slotUiPrefab.scene.rootCount != 0)
            Destroy(_slotUiPrefab);

        // -- Local Method --
        RectTransform CloneSlot()
        {
            GameObject slotGo = Instantiate(_slotUiPrefab);
            RectTransform rt = slotGo.GetComponent<RectTransform>();
            rt.SetParent(_contentAreaRT);

            return rt;
        }
    }

    public void OnPointerDown(PointerEventData pt)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphic.Raycast(pt, results);

        foreach (var i in results)
        {
            var isui = i.gameObject.GetComponent<ItemSlotUI>();
            if (isui != null)
            {
                if (pt.button == PointerEventData.InputButton.Left)
                {
                    if (isui.item != null && isui.item.count > 0)
                    {
                        SelectItem(isui.item, isui.index);
                        _dragBeginSlotIndex = isui.index;
                        _isItemDragging = true;

                        dragImage.gameObject.SetActive(true);
                        dragImage.sprite = isui.item.item.itemIcon;
                        dragImage.rectTransform.pivot = new Vector2(0, 1);
                    }
                } else if (pt.button == PointerEventData.InputButton.Right)
                {
                    SelectItem(isui.item, isui.index);
                    Use();
                    SelectItem(isui.item, isui.index);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData pt)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        graphic.Raycast(pt, results);
        
        foreach (var i in results)
        {
            var isui = i.gameObject.GetComponent<ItemSlotUI>();
            if (isui != null && _isItemDragging)
            {
                _dragEndSlotIndex = isui.index;
                GameManager.Instance.PlayerDataManager.inventory.Swap(_dragBeginSlotIndex, _dragEndSlotIndex);
                SelectItem(GameManager.Instance.PlayerDataManager.inventory.items[_dragEndSlotIndex], _dragEndSlotIndex);
            }
        }
        
        _isItemDragging = false;
        dragImage.gameObject.SetActive(false);
    }

    public void SelectItem(ItemSlot item, int index)
    {
        if (item == null || item.count == 0)
        {
            selectedItemIndex = index;
            selectedItemImage.sprite = null;
            selectedItemImage.color = new Color(0, 0, 0, 0);
            selectedItemName.text = "";
            selectedItemDescription.text = "";
            optionText.text = "";
        }
        else
        {
            selectedItemIndex = index;
            selectedItemImage.sprite = item.item.itemIcon;
            selectedItemImage.color = new Color(1, 1, 1, 1);
            selectedItemName.text = item.item.itemName;
            selectedItemDescription.text = item.item.itemDescription;
            switch (item.item.GetUseOption())
            {
                case 0 :
                    optionText.text = "장착";
                    break;
                case 1 :
                    optionText.text = "사용";
                    break;
                default:
                    optionText.text = "";
                    break;
            }
        }
    }

    public void Use()
    {
        int index = selectedItemIndex;
        var item = GameManager.Instance.PlayerDataManager.inventory.items[index];

        if (item != null && item.count > 0 && item.item != null)
        {
            switch (item.item.GetUseOption())
            {
                case 0:
                    Equip();
                    return;
                case 1:
                    Debug.Log("??");
                    Consume();
                    return;
                case 2:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    public void Equip()
    {
        int index = selectedItemIndex;
        var item = GameManager.Instance.PlayerDataManager.inventory.items[index];
        
        GameManager.Instance.UIManager.EquipmentUI.UpdateUI();
        
        if (item != null && item.count > 0 && item.item != null)
        {
            int part = ((EquipmentItemInfo)item.item).equipmentPart;
            var equipment = GameManager.Instance.PlayerDataManager.equipment.items[part];
            if (equipment == null)
            {
                GameManager.Instance.PlayerDataManager.inventory.items[index] = null;
                GameManager.Instance.PlayerDataManager.equipment.items[part] = (EquipmentItemInfo)item.item;
                SelectItem(null, index);
            }
            else
            {
                EquipmentItemInfo tmp = equipment;
                GameManager.Instance.PlayerDataManager.equipment.items[part] = (EquipmentItemInfo)item.item;
                item.item = tmp;
                item.count = 1;
                SelectItem(item, index);
            }
        }
    }

    public void Consume()
    {
        int index = selectedItemIndex;
        var item = GameManager.Instance.PlayerDataManager.inventory.items[index];
        
        GameManager.Instance.UIManager.EquipmentUI.UpdateUI();
        
        if (item != null && item.count > 0 && item.item != null)
        {
            Debug.Log("!!");
            var consumable = (ConsumableItemInfo)item.item;
            float hp = consumable.hpRecovery;
            float mp = consumable.mpRecovery;
            float saturation = consumable.saturationRecovery;

            GameManager.Instance.PlayerDataManager.hp += hp;
            GameManager.Instance.PlayerDataManager.mp += mp;
            GameManager.Instance.PlayerDataManager.saturation += saturation;

            GameManager.Instance.PlayerDataManager.inventory.DeleteItem(selectedItemIndex, 1);
            SelectItem(item, selectedItemIndex);
        }
    }

    public void DropItem()
    {
        if (GameManager.Instance.PlayerDataManager.inventory.items[selectedItemIndex].count <= 1)
        {
            DropItem(1);
        }
        else
        {
            if (!dropItemUI.activeSelf)
            {
                isDropping = true;
                dropItemUI.SetActive(true);
                DropItemCountChanged();
            }
            else
            {
                int num = Mathf.Clamp(
                    (int) (dropItemCountSlider.value * _slotUIList[selectedItemIndex].item.count), 
                    0, _slotUIList[selectedItemIndex].item.count);
                DropItem(num);
            }
        }
    }
    
    public void DropItem(int count)
    {
        dropItemUI.SetActive(false);
        if (GameManager.Instance.PlayerDataManager.inventory.DeleteItem(selectedItemIndex, count))
        {
            SelectItem(GameManager.Instance.PlayerDataManager.inventory.items[selectedItemIndex],
                selectedItemIndex);
            Debug.Log("Success to Drop");
        }
        else
        {
            Debug.Log("Fail to Drop");
        }
    }

    public void DropItemCountChanged()
    {
        int num = Mathf.Clamp(
            (int) (dropItemCountSlider.value * _slotUIList[selectedItemIndex].item.count), 
            0, _slotUIList[selectedItemIndex].item.count);
        dropItemCountText.text = "" + num + " / " + _slotUIList[selectedItemIndex].item.count;
    }

    private void Update()
    {
        if (_isItemDragging)
        {
            dragImagePos.position = Input.mousePosition;
        }
    }
}

