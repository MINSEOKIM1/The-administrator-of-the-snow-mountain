using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    public delegate void ProjectileMethod();
    // tmp variable
    private Vector3 _graphicLocalScale;
    private Vector2 _boxOffsetWithLocalscale;
    public Vector2 boxSize;
    public Vector2 boxOffset;

    public void AttackBoxCheck()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + _boxOffsetWithLocalscale, boxSize, 0);

        foreach (var i in collider2Ds)
        {
            if (i.CompareTag("Player"))
            {
                i.GetComponent<PlayerBehavior>().Hit(10, ((i.transform.position - transform.position)*2 + Vector3.up*4), 1);
            }
        }
    }

    public void CanAttack()
    {
        
    }

    public void MakeProjectile(ProjectileMethod method)
    {
        method();
    }

    public void SetAttackBox(Vector2 boxSize, Vector2 boxOffset)
    {
        this.boxSize = boxSize;
        this.boxOffset = boxOffset;
    }
}
