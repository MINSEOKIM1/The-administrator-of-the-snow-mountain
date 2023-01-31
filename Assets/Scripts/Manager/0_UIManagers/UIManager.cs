using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TitleUI TitleUI { get; private set; }
    private void Start()
    {
        TitleUI = GetComponentInChildren<TitleUI>();
    }
}