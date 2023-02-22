using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    public SpriteRenderer logo;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Destroy(GameManager.Instance.gameObject);
        }
        catch (Exception e)
        {
            
        }
        StartCoroutine(LogoAnimation());
    }

    IEnumerator LogoAnimation()
    {
        Color tmp = new Color(1, 1, 1, 0);
        logo.color = tmp;

        while (tmp.a < 1)
        {
            tmp.a += Time.fixedDeltaTime;
            logo.color = tmp;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1);
        
        while (tmp.a > 0)
        {
            tmp.a -= Time.fixedDeltaTime;
            logo.color = tmp;
            yield return new WaitForFixedUpdate();
        }
        
        OnMenu();
    }

    public void OnMenu()
    {
        SceneManager.LoadScene("Scenes/Minseo/MainMenu");
    }
}
