using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TitleUI TitleUI { get; private set; }
    public InventoryUI InventoryUI { get; private set; }
    public EquipmentUI EquipmentUI { get; private set; }

    [Tooltip("0: inventory\n1: equipment")]
    public GameObject[] uiCanvas;
    private void Start()
    {
        TitleUI = GetComponentInChildren<TitleUI>();
        InventoryUI = GetComponentInChildren<InventoryUI>();
        EquipmentUI = GetComponentInChildren<EquipmentUI>();
        
        ToEquipmentUI();
    }

    public void ToInventoryUI()
    {
        uiCanvas[0].SetActive(true);
        uiCanvas[1].SetActive(false);
    }
    
    public void ToEquipmentUI()
    {
        uiCanvas[0].SetActive(false);
        uiCanvas[1].SetActive(true);
    }
}
