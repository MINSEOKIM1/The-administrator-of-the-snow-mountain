using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [field: SerializeField] public TitleUI TitleUI { get; private set; }
    [field: SerializeField] public InventoryUI InventoryUI { get; private set; }
    [field: SerializeField] public EquipmentUI EquipmentUI { get; private set; }
    [field: SerializeField] public ConversationUI ConservationUI { get; private set; }

    [field: SerializeField] public MapUI MapUI { get; private set; }
    [field: SerializeField] public GameObject PlayerDataUI { get; private set; }

    public GameObject fadeCanvas;

    [Tooltip("0: inventory\n1: equipment\n2:MapUI")]
    public GameObject[] uiCanvas;
    private void Start()
    {
        // ToEquipmentUI();
    }

    public void ToInventoryUI()
    {
        AccessUICanvas(0);
    }
    
    public void ToEquipmentUI()
    {
        AccessUICanvas(1);
    }
    
    public void ToMapUI()
    {
        AccessUICanvas(2);
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
