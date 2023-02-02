using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public RectTransform selectionMark;
    public Button[] buttons;
    public GameObject title;

    public Vector3 selectionPos, targetPos;

    public float transitionSpeed;

    public int selection = 0;

    private void Start()
    {
        selectionPos = selectionMark.transform.position;
        targetPos = buttons[0].GetComponent<RectTransform>().anchoredPosition;;
        selectionMark.position = targetPos;
    }

    private void Update()
    {
        selectionPos = Vector3.Lerp(selectionPos, targetPos, transitionSpeed * Time.deltaTime);
        selectionMark.anchoredPosition = selectionPos;
    }

    public void ChangeSelection(InputAction.CallbackContext value)
    {
        if (!value.started) return;
        int num = (int) value.ReadValue<float>();
        
        ChangeSelection(selection + num);
    }

    public void ChangeSelection(int num)
    {
        GameManager.Instance.AudioManager.PlaySfx(0);
        buttons[selection].GetComponent<ButtonTest>().SetHighlightOff();
        
        selection = num;
        selection = Mathf.Clamp(selection, 0, 3);
        
        buttons[selection].GetComponent<ButtonTest>().SetHighlightOn();

        targetPos = buttons[selection].GetComponent<RectTransform>().anchoredPosition;
    }
}
