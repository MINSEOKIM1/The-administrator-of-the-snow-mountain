using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public ConversationClip[] conversationClips;
    public GameObject[] confiners;
    public Transform[] npcPos;
    public NPC tutorialNpc;
    public PolygonCollider2D[] camConfiners;
    public CinemachineVirtualCamera cam;
    public GameObject[] monsters;
    public Transform[] monsterPos;
    private void Start()
    {
        GameManager.Instance.DataManager.SaveCurrentState();
        GameManager.Instance.UIManager.ConservationUI.SetCurrentConservationArray(
            conversationClips, 
            0,
            new NPC());
        Instance = this;
    }

    public void SpawnMonster(int index)
    {
        var mon = Instantiate(monsters[index], monsterPos[index].position, Quaternion.identity);
        mon.transform.SetParent(transform);
    }
}
