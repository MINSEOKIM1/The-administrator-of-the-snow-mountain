using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public delegate void ProjectileMethod();

    public event ProjectileMethod projectileMethod;
    // tmp variable
    private Vector3 _graphicLocalScale;
    public Vector2 _boxOffsetWithLocalscale;
    public Vector2 boxSize;
    public Vector2 boxOffset;
    public float damage;
    public Vector2 atkKnockback;
    public float atkStunTime;

    public void AttackBoxCheck()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + _boxOffsetWithLocalscale, boxSize, 0);

        foreach (var i in collider2Ds)
        {
            if (i.CompareTag("Player"))
            {
                var k = atkKnockback;
                k.Set(i.transform.position.x < transform.parent.position.x ? -k.x : k.x, k.y);
                i.GetComponent<PlayerBehavior>().Hit(
                    damage,
                    (k), 
                    atkStunTime,
                    transform.parent.position);
            }
        }
    }

    public void CanAttack()
    {
        
    }

    public void MakeProjectile()
    {
        projectileMethod.Invoke();
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

    public void Die()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
