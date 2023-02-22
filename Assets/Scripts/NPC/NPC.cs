using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int conversationStart;
    public ConversationClip[] conversationClips;
    public bool tutorial;
    public TMP_Text textt;


    public int GetConversationStart()
    {
        return conversationStart;
    }

    private void Update()
    {
        if (tutorial)
        {
            textt.transform.localScale = transform.localScale;
        }
    }
}
