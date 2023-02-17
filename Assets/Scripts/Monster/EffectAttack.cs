using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EffectAttack : MonoBehaviour
{
    private float damage, stun;
    private Vector2 knockback;

    public Vector2 boxOffset, boxSize, _boxOffsetWithLocalscale;
    private CinemachineImpulseSource _ctx;

    public float power;
    
    protected void OnDrawGizmos()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + _boxOffsetWithLocalscale, boxSize);
    }
    
    public void SetInfo(float damage, float stun, Vector2 knockback)
    {
        this.damage = damage;
        this.stun = stun;
        this.knockback = knockback;
    }
    
    public void SetInfo(float damage, float stun, Vector2 knockback, Vector3 localScale)
    {
        this.damage = damage;
        this.stun = stun;
        this.knockback = knockback;
        transform.localScale = localScale;
    }

    public void AttackCheck()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + _boxOffsetWithLocalscale, boxSize, 0);

        foreach (var i in collider2Ds)
        {
            if (i.CompareTag("Player"))
            {
                var k = knockback;
                k.Set(i.transform.position.x < transform.parent.position.x ? -k.x : k.x, k.y);
                i.GetComponent<PlayerBehavior>().Hit(
                    damage,
                    (k), 
                    stun,
                    transform.parent.position);
            }
        }
    }

    public void Impulse()
    {
        _ctx = GetComponent<CinemachineImpulseSource>();
        _ctx.GenerateImpulseWithVelocity(Vector3.up * power);
    }
    public void Destroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
