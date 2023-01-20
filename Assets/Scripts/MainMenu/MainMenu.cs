using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneInfo sceneInfo;
    public void NewStartButton()
    {
        sceneInfo.beforeSceneName = "MainMenu";
        sceneInfo.currentSceneName = "Village";
        SceneManager.LoadScene("Village");
    }

    public void LoadButton()
    {
        
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
