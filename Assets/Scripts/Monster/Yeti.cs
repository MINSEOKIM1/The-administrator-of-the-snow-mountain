using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class Yeti : Monster
{
    public MonsterInfo _monsterInfo;
    private delegate void Attack();
    private Attack[] _attackMethods;

    private MonsterAttack _monsterAttack;
    private YetiAttackSignal _yetiAttack;
    [SerializeField] private CinemachineImpulseSource ctx;
    
    // from Monster's info... (MonsterInfo class will be made later)
    public Vector2[] attackDetectBoxes => ((MonsterInfo)entityInfo).attackDetectBoxes;
    public Vector2[] attackBoundaryBoxes => ((MonsterInfo)entityInfo).attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets => ((MonsterInfo)entityInfo).attackBoundaryOffsets;
    
    // tmp variable for store current state
    

    public float atkDash;

    public float rollPower;
    public float rollRate;

    public bool isRolling;

    protected override void Start()
    {
        base.Start();
        _monsterInfo = (MonsterInfo)entityInfo;
        _monsterAttack = GetComponentInChildren<MonsterAttack>();
        _yetiAttack = GetComponentInChildren<YetiAttackSignal>();
        _attackMethods = new Attack[3];
        _attackMethods[0] = Attack0;
        _attackMethods[1] = Attack1;
        _attackMethods[2] = Attack2;
        _yetiAttack.CanAttack();
    }

    protected override void Update()
    {
        base.Update();
        AttackCheck();
        _animator.SetFloat("speed", _speed);

        if (_yetiAttack.isRoll)
        {
            rollPower += Time.deltaTime * rollRate;
            dashSpeed = rollPower * graphicTransform.localScale.x;
        }
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
            _boxOffsetWithLocalscale.Set(attackBoundaryOffsets[i].x * graphicTransform.localScale.x, attackBoundaryOffsets[i].y);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((Vector2)graphicTransform.position + _boxOffsetWithLocalscale, attackBoundaryBoxes[i]);
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
        if (!_yetiAttack.canAttack || isDie) return;
        Collider2D[] tmp;
        
        tmp = Physics2D.OverlapBoxAll(transform.position, attackDetectBoxes[0], 0);
        foreach (var j in tmp)
        {
            if (j.CompareTag("Player"))
            {
                if (_target == null)
                {
                    perceivePlayerTimeElapsed = _monsterInfo.perceiveTime;
                    _target = j.gameObject;
                }

                int index; 
                
                do
                {
                    index = Random.Range(0, 3);
                } while (_monsterInfo.attackMp[index] > mp);
                
                mp -= _monsterInfo.attackMp[index];
                _attackMethods[index]();
                return;
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
            if (_target.transform.position.x < transform.position.x)
            {
                _graphicLocalScale.Set(1, 1, 1);
                graphicTransform.localScale = _graphicLocalScale;
            }
            else
            {
                _graphicLocalScale.Set(-1, 1, 1);
                graphicTransform.localScale = _graphicLocalScale;
            }
            _capsuleCollider.sharedMaterial = little;
            
            ctx.m_DefaultVelocity = Vector3.up * 0.4f;
            
            _speed = 0;
            _animator.SetInteger("attackNum",0 );
            _animator.SetTrigger("attack");
            _yetiAttack.SetCanAttack(false);
            
            _monsterAttack.SetAttackBox(attackBoundaryBoxes[0], attackBoundaryOffsets[0]);
            _monsterAttack.SetAttackInfo(
                _monsterInfo.attackKnockback[0], 
                _monsterInfo.atk * _monsterInfo.attackCoefficient[0],
                _monsterInfo.attackStunTime[0]);
        }
    }
    
    public void Attack1()
    {
        if (CanAttackLogic())
        {
            if (_target.transform.position.x < transform.position.x)
            {
                _graphicLocalScale.Set(1, 1, 1);
                graphicTransform.localScale = _graphicLocalScale;
            }
            else
            {
                _graphicLocalScale.Set(-1, 1, 1);
                graphicTransform.localScale = _graphicLocalScale;
            }
            
            _capsuleCollider.sharedMaterial = little;
            
            ctx.m_DefaultVelocity = Vector3.up * 0.1f;
            
            _speed = 0;
            _animator.SetInteger("attackNum", 1);
            _animator.SetTrigger("attack");
            _yetiAttack.SetCanAttack(false);

            

            _monsterAttack.SetAttackBox(attackBoundaryBoxes[1], attackBoundaryOffsets[1]);
            _monsterAttack.SetAttackInfo(
                _monsterInfo.attackKnockback[1], 
                _monsterInfo.atk * _monsterInfo.attackCoefficient[1],
                _monsterInfo.attackStunTime[1]);
        }
    }

    public void Attack2()
    {
        if (CanAttackLogic())
        {
            if (_target.transform.position.x < transform.position.x)
            {
                _graphicLocalScale.Set(1, 1, 1);
                graphicTransform.localScale = _graphicLocalScale;
            }
            else
            {
                _graphicLocalScale.Set(-1, 1, 1);
                graphicTransform.localScale = _graphicLocalScale;
            }
            
            _capsuleCollider.sharedMaterial = little;
            
            ctx.m_DefaultVelocity = Vector3.up * 0.1f;
            rollPower = 0;
            
            _speed = 0;
            _animator.SetTrigger("roll");
            _yetiAttack.SetCanAttack(false);
        }
    }
    

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        if (col.collider.CompareTag("Player") && _yetiAttack.isRoll)
        {
            isRolling = false;
            var i = col.collider.GetComponent<PlayerBehavior>();
            var k = ((MonsterInfo)entityInfo).attackKnockback[2];
            k.Set(i.transform.position.x < transform.position.x ? -k.x : k.x, k.y);
            i.Hit(
                ((MonsterInfo)entityInfo).atk * ((MonsterInfo)entityInfo).attackCoefficient[2],
                (k), 
                ((MonsterInfo)entityInfo).attackStunTime[2],
                transform.position);
        } else if (col.collider.CompareTag("Ground") && _yetiAttack.isRoll)
        {
            foreach (var i in col.contacts)
            {
                if (Mathf.Abs(i.normal.x) > 0.9)
                {
                    _yetiAttack.isRoll = false;
                    _animator.SetTrigger("rollout");
                    ctx.m_DefaultVelocity = Vector3.up * 1f;
                    ctx.GenerateImpulse();
                    stunTime = 5;
                    stunTimeElapsed = 5;
                    _yetiAttack.CanAttack();
                }
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D col)
    {
        base.OnCollisionStay2D(col);
        
    }
}
