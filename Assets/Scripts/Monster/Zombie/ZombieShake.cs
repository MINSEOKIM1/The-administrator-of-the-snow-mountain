using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieShake : MonoBehaviour
{
    private bool _gravityEffect;
    private Vector2 _velocity, _knockback;
    private float _damage, _stunTime;
    private Rigidbody2D _rigidbody;
    private float _time;

    private void Update()
    {
        _time -= Time.deltaTime;
        
        if (_time < 0) Destroy(gameObject);
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            var k = _knockback;
            k.Set(col.transform.position.x < transform.position.x ? -k.x : k.x, k.y);
            col.collider.GetComponent<PlayerBehavior>().Hit(
                _damage, k, _stunTime, transform.position);
            
            Destroy(gameObject);
        } else if (col.collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
