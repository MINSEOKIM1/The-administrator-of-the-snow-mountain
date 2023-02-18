using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(GameManager.Instance.gameObject);
    }

    public void OnMenu()
    {
        SceneManager.LoadScene("Scenes/Minseo/MainMenu");
    }
}
