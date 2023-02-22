using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentNPC : NPC
{
    public AgentData agentData;
    public int agentIndex;

    private void Start()
    {
        try
        {
            for (int i = 0; i < GameManager.Instance.MapManager.dungeons.Length; i++)
            {
                if (agentData.agentName.Equals(GameManager.Instance.MapManager.dungeons[i].agent.agentName))
                {
                    agentData = GameManager.Instance.MapManager.dungeons[i].agent;
                }
            }
        }
        catch
        {

        }

        SetActiveWithCondition();
    }

    private void FixedUpdate()
    {
        SetActiveWithCondition();
    }

    public void SetActiveWithCondition()
    {
        gameObject.SetActive(agentData.currentMapName.Equals(GameManager.Instance.MapManager.currentSceneName) 
                             && GameManager.Instance.PlayerDataManager.agentAvailable[agentIndex]);
    }
}
