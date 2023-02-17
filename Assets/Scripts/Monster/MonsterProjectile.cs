using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    private bool _gravityEffect;
    private Vector2 _velocity, _knockback;
    private float _damage, _stunTime;
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private float _time;
    private bool _explosion;

    private void Update()
    {
        _time -= Time.deltaTime;
        
        if (_time < 0) Destroy(gameObject);
        
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_rigidbody.velocity.y, _rigidbody.velocity.x) * Mathf.Rad2Deg);
    }

    public void SetInfo(bool gravity, Vector2 velocity, Vector2 knockback, float damage, float stunTime)
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _gravityEffect = gravity;
        _velocity = velocity;
        _knockback = knockback;
        _damage = damage;
        _stunTime = stunTime;

        _rigidbody.velocity = _velocity;
        if (!_gravityEffect) _rigidbody.gravityScale = 0;

        _time = 10f;
    }
    public void SetInfo(bool gravity, Vector2 velocity, Vector2 knockback, float damage, float stunTime, bool explosion)
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _gravityEffect = gravity;
        _velocity = velocity;
        _knockback = knockback;
        _damage = damage;
        _stunTime = stunTime;
        _explosion = explosion;
        _animator = GetComponent<Animator>();

        _rigidbody.velocity = _velocity;
        if (!_gravityEffect) _rigidbody.gravityScale = 0;

        _time = 10f;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            var k = _knockback;
            k.Set(col.transform.position.x < transform.position.x ? -k.x : k.x, k.y);
            col.collider.GetComponent<PlayerBehavior>().Hit(
                _damage, k, _stunTime, transform.position);

            if (_explosion)
            {
                _animator.SetTrigger("explosion");
                GetComponent<Collider2D>().isTrigger = true;
                _rigidbody.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                Destroy(gameObject);
            }
        } else if (col.collider.CompareTag("Ground"))
        {
            if (_explosion)
            {
                _animator.SetTrigger("explosion");
                GetComponent<Collider2D>().isTrigger = true;
                _rigidbody.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
