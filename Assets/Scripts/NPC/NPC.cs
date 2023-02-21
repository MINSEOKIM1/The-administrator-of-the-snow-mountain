using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int conversationStart;
    public ConversationClip[] conversationClips;

    public int GetConversationStart()
    {
        return conversationStart;
    }
}
