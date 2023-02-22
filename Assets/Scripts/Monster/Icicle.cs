using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Icicle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private Animator ani;

    public float damage;
    public Vector2 knockback;
    public float stunTime;

    public bool isFalling = false;
    private void Start()
    {
        StartCoroutine(FadeIn());
        rigidbody.bodyType = RigidbodyType2D.Static;
    }

    public void StartFalling()
    {
        rigidbody.bodyType = RigidbodyType2D.Dynamic;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        rigidbody.gravityScale = Random.Range(1.5f, 2.3f);
        isFalling = true;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    IEnumerator FadeIn()
    {
        sprite.material.color = new Color(1, 1, 1, 0);
        var c = sprite.material.color;

        while (sprite.material.color.a < 1)
        {
            c.a += 0.01f;
            sprite.material.color = c;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (isFalling && ani.GetCurrentAnimatorStateInfo(0).IsTag("go"))
        {
            if (col.collider.CompareTag("Player"))
            {
                col.gameObject.GetComponent<PlayerBehavior>().Hit(damage, knockback, stunTime, transform.position);
                GameManager.Instance.AudioManager.PlaySfx(36);
                ani.SetTrigger("explosion");
                rigidbody.bodyType = RigidbodyType2D.Static;
            }
            else if (col.collider.CompareTag("Ground") && col.contacts[0].normal.y > 0.9)
            {
                ani.SetTrigger("explosion");
                GameManager.Instance.AudioManager.PlaySfx(36);
                rigidbody.bodyType = RigidbodyType2D.Static;
            }
            else if (!col.collider.CompareTag("Icicle"))
            {
                ani.SetTrigger("explosion");
                GameManager.Instance.AudioManager.PlaySfx(36);
                rigidbody.bodyType = RigidbodyType2D.Static;
            }
        }
    }
}
