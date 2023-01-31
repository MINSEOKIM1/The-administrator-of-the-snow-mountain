using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundMove : MonoBehaviour
{
    public GameObject backGround;
    public Image panel;
    public Transform origin;

    public float backGroundSpeed, fadeSpeed, transitionTime;

    public float transitionTimeElapsed;

    private void Update()
    {
        transitionTimeElapsed -= Time.deltaTime;
        backGround.transform.Translate(Vector3.left * (backGroundSpeed * Time.deltaTime));
        if (transitionTimeElapsed < 0)
        {
            transitionTimeElapsed = transitionTime;
            StartCoroutine(Transition());
        }
    }

    IEnumerator Transition()
    {
        Color tmp = panel.color;

        while (tmp.a < 1)
        {
            tmp.a += fadeSpeed * Time.deltaTime;
            panel.color = tmp;
            yield return new WaitForFixedUpdate();
        }

        backGround.transform.position = origin.position;
        
        while (tmp.a > 0)
        {
            tmp.a -= fadeSpeed * Time.deltaTime;
            panel.color = tmp;
            yield return new WaitForFixedUpdate();
        }
    }
}
