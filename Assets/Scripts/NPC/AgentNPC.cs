using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentNPC : NPC
{
    public AgentData agentData;

    private void Start()
    {
        SetActiveWithCondition();
        GameManager.Instance.UIManager.MapUI.agentNPCEvent += () => SetActiveWithCondition();
    }

    private void SetActiveWithCondition()
    {
        gameObject.SetActive(agentData.currentMapName.Equals(GameManager.Instance.MapManager.currentSceneName));
    }
}
