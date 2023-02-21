using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialSign : MonoBehaviour
{
    public Color a;
    public TMP_Text text;
    [Multiline(7)] public string textContents;
    public SpriteRenderer _sprite;

    private void Start()
    {
        a = new Color(1, 1, 1, 0);
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _sprite.color = a;
        text.color = a;
        text.text = textContents;
    }

    private void Update()
    {
        if (a.a > 0)
        {
            a.a -= Time.deltaTime;
            _sprite.color = a;
            text.color = a;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && a.a < 1 && !GameManager.Instance.UIManager.ConservationUI.gameObject.activeSelf)
        {
            a.a += Time.deltaTime * 2;
            _sprite.color = a;
            text.color = a;
        }
    }
}
