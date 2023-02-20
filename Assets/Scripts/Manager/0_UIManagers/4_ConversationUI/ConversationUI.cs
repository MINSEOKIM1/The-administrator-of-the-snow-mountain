using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ConversationUI : MonoBehaviour
{
    public ConversationClip currentClip;
    public ConversationClip[] currentConversationArray;

    public Image speakerImage;
    public TMP_Text speakerName, contents;
    public int conversationKind;

    public GameObject conversationWindow;
    public GameObject cookWindow;
    public GameObject smithyWindow;

    public void UIUpdate()
    {
        if (currentClip == null) return;

        gameObject.SetActive(true);
        conversationWindow.SetActive(true);
        speakerImage.sprite = currentClip.speakerImage;
        speakerName.text = currentClip.speakerName;
        contents.text = currentClip.contents;
        conversationKind = currentClip.conservationKind;
    }

    public void SetCurrentConservationArray(ConversationClip[] ctx, int index)
    {
        currentConversationArray = ctx;
        currentClip = currentConversationArray[index];
        
        UIUpdate();
    }

    public void OnNext(InputAction.CallbackContext value)
    {
        if (value.started) OnNext();
    }
    
    public void OnPrev(InputAction.CallbackContext value)
    {
        if (value.started) OnPrev();
    }

    public void OnNext()
    {
        if (!conversationWindow.activeSelf)
        {
            if (cookWindow.activeSelf)
            {
                cookWindow.GetComponent<CookUI>().OnMake();
            } else if (smithyWindow.activeSelf)
            {
                smithyWindow.GetComponent<SmithyUI>().OnMake();
            }
        }
        else
        {
            if (currentClip == null || currentConversationArray == null) return;
            if (currentClip.nextClipIndex == -1)
            {
                gameObject.SetActive(false);
            }
            else if (currentClip.nextClipIndex == -2)
            {
                conversationWindow.SetActive(false);
                cookWindow.SetActive(true);
            }
            else if (currentClip.nextClipIndex == -3)
            {
                conversationWindow.SetActive(false);
                smithyWindow.SetActive(true);
            } else if (currentClip.nextClipIndex == -4)
            {
                conversationWindow.SetActive(false);
                gameObject.SetActive(false);
                GameManager.Instance.UIManager.MapUI.isAllocating = true;
                GameManager.Instance.UIManager.AccessUICanvas(2);
            }
            else
            {
                currentClip = currentConversationArray[currentClip.nextClipIndex];
                UIUpdate();
            }
        }
    }

    public void OnPrev()
    {
        if (!conversationWindow.activeSelf)
        {
            if (cookWindow.activeSelf)
            {
                cookWindow.GetComponent<CookUI>().OnCancel();
            }
            else if (smithyWindow.activeSelf)
            {
                smithyWindow.GetComponent<SmithyUI>().OnCancel();
            }
        }
        else
        {
            if (currentClip == null || currentConversationArray == null) return;
            if (currentClip.prevClipIndex == -1)
            {
                gameObject.SetActive(false);
            }
            else
            {
                currentClip = currentConversationArray[currentClip.prevClipIndex];
                UIUpdate();
            }
        }
    }
}
