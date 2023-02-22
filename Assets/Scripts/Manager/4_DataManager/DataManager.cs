using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private string path;
    public DataPack[] dataPacks;
    public int currentDataSlot;
    
    public DataPack DataPack { get; private set; }

    private void Awake()
    {
        dataPacks = new DataPack[3];
        path = Application.persistentDataPath;
        
        Debug.Log(path);

        for (int i = 0; i < 3; i++)
        {
            LoadData(i);
        }
    }

    public void SaveData(int index)
    {
        string data = JsonUtility.ToJson(dataPacks[index]);
        File.WriteAllText(path + "/Data" + index, data);
    }
    

    private void LoadData(int index)
    {
        if (File.Exists(path + "/Data" + index))
        {
            string data;
            
            data = File.ReadAllText(path + "/Data" + index);
            dataPacks[index] = JsonUtility.FromJson<DataPack>(data);
        }
        else
        {
            dataPacks[index] = new DataPack();
        }
    }

    public void SaveCurrentState(int index)
    {
        dataPacks[index].bastion = GameManager.Instance.MapManager.bastion;
        dataPacks[index].dungeons = GameManager.Instance.MapManager.dungeons;
        dataPacks[index].globalRate = GameManager.Instance.MapManager.globalRate;
        dataPacks[index].fork = GameManager.Instance.MapManager.fork;
        dataPacks[index].beforeSceneName = GameManager.Instance.MapManager.beforeSceneName;
        dataPacks[index].currentSceneName = GameManager.Instance.MapManager.currentSceneName;
        dataPacks[index].spawnRate = GameManager.Instance.MapManager.spawnRate;
        dataPacks[index].playTime = GameManager.Instance.MapManager.playTime;

        try
        {
            dataPacks[index].position = GameObject.FindWithTag("Player").transform.position;
        }
        catch (Exception e)
        {
            dataPacks[index].position = Vector3.zero;
        }


        dataPacks[index].agentAvailable = GameManager.Instance.PlayerDataManager.agentAvailable;
        dataPacks[index].equipment = GameManager.Instance.PlayerDataManager.equipment.GameToDataPack();
        dataPacks[index].inventory = GameManager.Instance.PlayerDataManager.inventory.GameToDataPack();
        dataPacks[index].SetInvenIndexCount();
        dataPacks[index].exp = GameManager.Instance.PlayerDataManager.exp;
        dataPacks[index].hp = GameManager.Instance.PlayerDataManager.hp;
        dataPacks[index].mp = GameManager.Instance.PlayerDataManager.mp;
        dataPacks[index].saturation = GameManager.Instance.PlayerDataManager.saturation;
        dataPacks[index].lv = GameManager.Instance.PlayerDataManager.level;
        dataPacks[index].attackSkillLevel = GameManager.Instance.PlayerDataManager.attackSkillLevel;
        dataPacks[index].utilSkillLevel = GameManager.Instance.PlayerDataManager.utilSkillLevel;
        
        SaveData(index);
    }

    public void SaveCurrentState()
    {
        SaveCurrentState(currentDataSlot);
        Debug.Log("SAVE OK!");
    }

    public void LoadToGame(int index)
    {
        GameManager.Instance.MapManager.bastion = dataPacks[index].bastion;
        GameManager.Instance.MapManager.dungeons = dataPacks[index].dungeons;
        GameManager.Instance.MapManager.globalRate = dataPacks[index].globalRate;
        GameManager.Instance.MapManager.fork = dataPacks[index].fork;
        GameManager.Instance.MapManager.beforeSceneName = dataPacks[index].beforeSceneName;
        GameManager.Instance.MapManager.currentSceneName = dataPacks[index].currentSceneName;
        GameManager.Instance.MapManager.spawnRate = dataPacks[index].spawnRate;
        GameManager.Instance.MapManager.playTime = dataPacks[index].playTime;

        GameManager.Instance.PlayerDataManager.loadPos = dataPacks[index].position;
        GameManager.Instance.PlayerDataManager.loadPosFromData = true;
        
        GameManager.Instance.PlayerDataManager.agentAvailable = dataPacks[index].agentAvailable;
        GameManager.Instance.PlayerDataManager.equipment.SetFromDataPack(dataPacks[index].equipment);
        GameManager.Instance.PlayerDataManager.inventory.SetFromDataPack(dataPacks[index].GetInvenSaveData());
        GameManager.Instance.PlayerDataManager.exp = dataPacks[index].exp;
        GameManager.Instance.PlayerDataManager.hp = dataPacks[index].hp;
        GameManager.Instance.PlayerDataManager.mp = dataPacks[index].mp;
        GameManager.Instance.PlayerDataManager.saturation = dataPacks[index].saturation;
        GameManager.Instance.PlayerDataManager.level = dataPacks[index].lv;
        GameManager.Instance.PlayerDataManager.attackSkillLevel = dataPacks[index].attackSkillLevel;
        GameManager.Instance.PlayerDataManager.utilSkillLevel = dataPacks[index].utilSkillLevel;
    }

    public void ResetData()
    {
        for (int i = 0; i < 3; i++)
        {
            dataPacks[i] = new DataPack();
            SaveData(i);
        }
    }
}
