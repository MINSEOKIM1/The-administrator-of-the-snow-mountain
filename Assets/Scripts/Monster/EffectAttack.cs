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
    
    public void SetInfo(float damage, float stun, Vector2 knockback, Vector3 localScale, bool groundEffect)
    {
        this.damage = damage;
        this.stun = stun;
        this.knockback = knockback;

        if (groundEffect)
        {
            Vector2 pos = transform.parent.position;
            while (Physics2D.OverlapPoint(pos).CompareTag("Ground"))
            {
                pos.y += 0.1f;
            }
            var aa = Physics2D.RaycastAll(pos, Vector2.down, 100);
            Debug.DrawLine(pos, pos + Vector2.down*100);
            foreach (var a in aa)
            {
                if (a.collider.CompareTag("Ground"))
                {
                    transform.parent.position = a.point;
                    break;
                }
            }
        }
    }

    public void AttackCheck()
    {
        MonsterProjectile ac;
        if ((ac = GetComponent<MonsterProjectile>()) != null)
        {
            damage = ac.Damage;
            knockback = ac.Knockback;
            stun = ac.Stun;
        }
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + _boxOffsetWithLocalscale, boxSize, 0);

        foreach (var i in collider2Ds)
        {
            if (i.CompareTag("Player"))
            {
                var k = knockback;
                Vector3 c;
                if (transform.parent == null) c = transform.position;
                else c = transform.parent.position;
                k.Set(i.transform.position.x < c.x ? -k.x : k.x, k.y);
                i.GetComponent<PlayerBehavior>().Hit(
                    damage,
                    (k), 
                    stun,
                    c);
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
        if (transform.parent == null) Destroy(gameObject);
        else
        {
            Destroy(transform.parent.gameObject);
        }
        
    }
    public void PlaySFX(int index)
    {
        GameManager.Instance.AudioManager.PlaySfx(index);
    }
}
