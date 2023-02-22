using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentUI : MonoBehaviour
{
    public AgentData agent;
    public TMP_Text agentText;
    public Image agentImage, cooldown;
    

    public void SetAgent(AgentData agent)
    {
        this.agent = agent;
    }
    
    float RoundTo(float c, int a)
    {
        return Mathf.Round(c * Mathf.Pow(10, a)) / Mathf.Pow(10, a);
    }
    private void Update()
    {
        if (agent != null)
        {
            agentImage.sprite = agent.agentImage;
            cooldown.fillAmount = agent.timeElapsed / (agent.timeTakenToHunt * agent.timeRate);
            agentText.text = String.Format("{0}\n사냥 시간 {1,4} / {2,4} (초)", agent.agentName, 
                RoundTo(agent.timeElapsed, 2),
                RoundTo(agent.timeTakenToHunt * agent.timeRate, 2));
        }
    }
}
