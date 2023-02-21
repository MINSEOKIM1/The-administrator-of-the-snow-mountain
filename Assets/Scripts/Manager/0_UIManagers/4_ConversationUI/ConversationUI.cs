using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

    public NPC currentNpc;

    public EquipmentItemInfo woodSword;
    public ItemInfo monsterMeat;

    public void UIUpdate()
    {
        if (currentClip == null) return;

        gameObject.SetActive(true);
        conversationWindow.SetActive(true);
        if (currentNpc != null)
        {
            if (currentNpc.transform.position.x < GameObject.FindWithTag("Player").transform.position.x)
            {
                currentNpc.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                currentNpc.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        speakerImage.sprite = currentClip.speakerImage;
        speakerName.text = currentClip.speakerName;
        contents.text = currentClip.contents;
        conversationKind = currentClip.conservationKind;
    }

    public void SetCurrentConservationArray(ConversationClip[] ctx, int index, NPC npc)
    {
        currentConversationArray = ctx;
        currentClip = currentConversationArray[index];
        currentNpc = npc;
        
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
            if (currentClip.eventIndex == -1)
            {
                GameManager.Instance.PlayerDataManager.inventory.AddItem(woodSword, 1);
            } else if (currentClip.eventIndex == -2)
            {
                GameManager.Instance.PlayerDataManager.inventory.AddItem(monsterMeat, 1);
                GameManager.Instance.PlayerDataManager.hp -= GameManager.Instance.PlayerDataManager.maxHp / 2;
            } else if (currentClip.eventIndex == -3)
            {
                TutorialManager.Instance.tutorialNpc.transform.position = TutorialManager.Instance.npcPos[0].position;
                TutorialManager.Instance.confiners[0].SetActive(false);
                TutorialManager.Instance.cam.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D =
                    TutorialManager.Instance.camConfiners[1];
            } else if (currentClip.eventIndex == -4)
            {
                TutorialManager.Instance.SpawnMonster(0);
                GameManager.Instance.PlayerDataManager.tutorial++;
            } else if (currentClip.eventIndex == -5)
            {
                GameManager.Instance.PlayerDataManager.tutorial++;
            } else if (currentClip.eventIndex == -6)
            {
                TutorialManager.Instance.SpawnMonster(1);
                GameManager.Instance.PlayerDataManager.tutorial++;
            }
            else if (currentClip.eventIndex == -7)
            {
                TutorialManager.Instance.monsters[2].SetActive(true);
                GameManager.Instance.PlayerDataManager.tutorial++;
            }

            if (currentNpc.conversationStart < currentClip.nextStartIndex) currentNpc.conversationStart = currentClip.nextStartIndex;
            
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
