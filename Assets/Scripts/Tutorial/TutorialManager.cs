using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public ConversationClip[] conversationClips;
    private void Start()
    {
        GameManager.Instance.UIManager.ConservationUI.SetCurrentConservationArray(
            conversationClips, 
            0);
    }
}
