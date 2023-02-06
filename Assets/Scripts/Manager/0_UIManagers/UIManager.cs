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
        AccessUICanvas(0);
    }
    
    public void ToEquipmentUI()
    {
        AccessUICanvas(1);
    }

    public void AccessUICanvas(int index)
    {
        for (int i = 0; i < uiCanvas.Length; i++)
        {
            if (i == index)
            {
                uiCanvas[i].SetActive(!uiCanvas[i].activeSelf);
            }
            else
            {
                uiCanvas[i].SetActive(false);
            }
        }
    }
}
