using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationUI : MonoBehaviour
{
    public ConversationClip currentClip;
    public ConversationClip[] currentConversationArray;

    public Image speakerImage;
    public TMP_Text speakerName, contents;
    public int conversationKind;

    public void UIUpdate()
    {
        if (currentClip == null) return;

        gameObject.SetActive(true);
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

    public void OnNext()
    {
        if (currentClip == null || currentConversationArray == null) return;
        if (currentClip.nextClipIndex == -1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            currentClip = currentConversationArray[currentClip.nextClipIndex];
            UIUpdate();
        }
    }
    
    public void OnPrev()
    {
        if (currentClip == null || currentConversationArray == null) return;
        if (currentClip.nextClipIndex == -1)
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
