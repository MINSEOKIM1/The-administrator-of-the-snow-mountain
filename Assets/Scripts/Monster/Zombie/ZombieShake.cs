using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieShake : MonoBehaviour
{
    private Vector3 _graphicLocalScale;
    private Vector2 _boxOffsetWithLocalscale;
    public Vector2 boxSize;
    public Vector2 boxOffset;
    public float damage;
    public Vector2 atkKnockback;
    public float atkStunTime;
    private float _time;

    private void Start() {
        _time = 100.0f;
    }
    
    private void Update()
    {
        _time -= Time.deltaTime;
        
        if (_time < 0) Destroy(gameObject);
        
    }

    private void OnDrawGizmos()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * _graphicLocalScale.x, boxOffset.y);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + _boxOffsetWithLocalscale, boxSize);
        Gizmos.DrawLine(
            (Vector2)transform.position + _boxOffsetWithLocalscale, 
            (Vector2)transform.position + _boxOffsetWithLocalscale + Vector2.down);
    }

    public void AttackBoxCheck()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + _boxOffsetWithLocalscale, boxSize, 0);
        _time = 1.0f;

        foreach (var i in collider2Ds)
        {
            if (i.CompareTag("Player"))
            {
                var k = atkKnockback;
                k.Set(i.transform.position.x < transform.position.x ? -k.x : k.x, k.y);
                i.GetComponent<PlayerBehavior>().Hit(
                    damage,
                    (k), 
                    atkStunTime);
            }
        }
    }

    public void SetAttackBox(Vector2 boxSize, Vector2 boxOffset)
    {
        this.boxSize = boxSize;
        this.boxOffset = boxOffset;
    }

    public void SetAttackInfo(Vector2 knockback, float damage, float stunTime)
    {
        this.atkKnockback = knockback;
        this.damage = damage;
        this.atkStunTime = stunTime;
    }
}
