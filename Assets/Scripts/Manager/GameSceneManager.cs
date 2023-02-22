using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public Image fade;
    public bool changing;

    public float fadeRate;

    public bool gameover;

    public void LoadScene(string name)
    {
        if (changing == false) StartCoroutine(ChangeScene(name));
    }

    public void GameOver()
    {
        StartCoroutine(IGameOver());
    }

    IEnumerator IGameOver()
    {
        gameover = true;
        GameManager.Instance.UIManager.PlayerDataUI.SetActive(false);
        while (Time.timeScale > 0.4f)
        {
            Time.timeScale -= Time.fixedDeltaTime*2;
            yield return new WaitForFixedUpdate();
        }
        
        StartCoroutine(ChangeScene("GameOverScene"));
    }

    IEnumerator ChangeScene(string name)
    {
        changing = true;
        Color c = fade.color;
        var a = GameManager.Instance.MapManager.GetMapWithString(name);
        var b = GameManager.Instance.MapManager.GetMapWithString(SceneManager.GetActiveScene().name);
        GameManager.Instance.MapManager.currentSceneName = name;
        GameManager.Instance.MapManager.beforeSceneName = b.name;
        if (a != null && b != null)
        {
            if (a.bgmIndex != b.bgmIndex)
            {
                GameManager.Instance.AudioManager.ChangeBgm(a.bgmIndex);
            }
        }
        while (c.a < 1)
        {
            c.a += fadeRate * Time.fixedDeltaTime;
            fade.color = c;
            yield return new WaitForFixedUpdate();
        }

        if (gameover) Time.timeScale = 1;
        SceneManager.LoadScene(name);
        if (name.Equals("MainMenu"))
        {
            GameManager.Instance.UIManager.TitleUI.gameObject.SetActive(true);
        }
        
        while (c.a > 0)
        {
            c.a -= fadeRate * Time.fixedDeltaTime;
            fade.color = c;
            yield return new WaitForFixedUpdate();
        }

        gameover = false;
        changing = false;
    }
}
