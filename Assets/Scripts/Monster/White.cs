using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class White : Monster
{
    private delegate void Attack();
    private Attack[] _attackMethods;

    private MonsterAttack _monsterAttack;
    
    // from Monster's info... (MonsterInfo class will be made later)
    public Vector2[] attackDetectBoxes => ((MonsterInfo)entityInfo).attackDetectBoxes;
    public Vector2[] attackBoundaryBoxes => ((MonsterInfo)entityInfo).attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets => ((MonsterInfo)entityInfo).attackBoundaryOffsets;

    protected override void Start()
    {
        base.Start();
        _monsterAttack = GetComponentInChildren<MonsterAttack>();
        _attackMethods = new Attack[2];
        _attackMethods[0] = Attack0;
        _attackMethods[1] = Attack0;
    }

    protected override void Update()
    {
        base.Update();
        AttackCheck();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        foreach (var i in attackDetectBoxes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)transform.position, i);
        }

        for (int i = 0; i < attackBoundaryBoxes.Length; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((Vector2)transform.position + attackBoundaryOffsets[i], attackBoundaryBoxes[i]);
        }
    }
    
    private void AttackCheck()
    {
        _isAttack = _animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");
    }

    protected override void ApplyAnimation()
    {
        _animator.SetFloat("speed", _speed);
    }

    protected override void AttackInputDetect()
    {
        Collider2D[] tmp;
        for(int i = 0; i<attackDetectBoxes.Length; i++)
        {
            tmp = Physics2D.OverlapBoxAll(transform.position, attackDetectBoxes[i], 0);
            foreach (var j in tmp)
            {
                if (j.CompareTag("Player"))
                {
                    if (_target == null) _target = j.gameObject;
                    if (_target.transform.position.x < transform.position.x)
                    {
                        _graphicLocalScale.Set(1, 1, 1);
                        playerGraphicTransform.localScale = _graphicLocalScale;
                    }
                    else
                    {
                        _graphicLocalScale.Set(-1, 1, 1);
                        playerGraphicTransform.localScale = _graphicLocalScale;
                    }
                    _attackMethods[i]();
                }
            }
        }
    }

    public override void Die()
    {
        _animator.SetTrigger("die");
        base.Die();
    }

    public void Attack0()
    {
        if (CanAttackLogic())
        {
            _capsuleCollider.sharedMaterial = little;
            
            _speed = 0;
            _animator.SetFloat("attackNum", 0);
            _animator.SetTrigger("attack");
            
            _monsterAttack.SetAttackBox(attackBoundaryBoxes[0], attackBoundaryOffsets[0]);
        }
    }

    public void Attack1()
    {
        if (CanAttackLogic())
        {
            _capsuleCollider.sharedMaterial = little;
            
            _speed = 0;
            _animator.SetFloat("attackNum", 1);
            _animator.SetTrigger("attack");
        }
    }
}
