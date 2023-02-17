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

    public void LoadScene(string name)
    {
        if (changing == false) StartCoroutine(ChangeScene(name));
    }

    IEnumerator ChangeScene(string name)
    {
        changing = true;
        Color c = fade.color;
        var a = GameManager.Instance.MapManager.GetMapWithString(name);
        var b = GameManager.Instance.MapManager.GetMapWithString(SceneManager.GetActiveScene().name);
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
        
        SceneManager.LoadScene(name);
        
        while (c.a > 0)
        {
            c.a -= fadeRate * Time.fixedDeltaTime;
            fade.color = c;
            yield return new WaitForFixedUpdate();
        }

        changing = false;
    }
}
