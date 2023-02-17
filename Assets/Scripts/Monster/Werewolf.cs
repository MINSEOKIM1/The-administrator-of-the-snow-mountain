using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Werewolf : Monster
{
    private MonsterInfo _monsterInfo;
    private delegate void Attack();
    private Attack[] _attackMethods;

    private MonsterAttack _monsterAttack;
    private WerewolfAttackSignal _werewolfAttack;
    
    // from Monster's info... (MonsterInfo class will be made later)
    public Vector2[] attackDetectBoxes => ((MonsterInfo)entityInfo).attackDetectBoxes;
    public Vector2[] attackBoundaryBoxes => ((MonsterInfo)entityInfo).attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets => ((MonsterInfo)entityInfo).attackBoundaryOffsets;
    
    // tmp variable for store current state
    public bool jumping;
    public bool diving;
    public bool divingAttack;

    public float diveSpeed;

    public float atkDash;

    public float jumpElpased;

    protected override void Start()
    {
        base.Start();
        _monsterInfo = (MonsterInfo)entityInfo;
        _monsterAttack = GetComponentInChildren<MonsterAttack>();
        _werewolfAttack = GetComponentInChildren<WerewolfAttackSignal>();
        _attackMethods = new Attack[2];
        _attackMethods[0] = Attack0;
        _attackMethods[1] = Attack1;
        _werewolfAttack.CanAttack();
    }

    protected override void Update()
    {
        base.Update();
        AttackCheck();
        _animator.SetFloat("speed", _speed);

        jumpElpased -= Time.deltaTime;
        
        if (jumping && _rigidbody.velocity.y < 0 && !diving && jumpElpased < 0)
        {
            StartCoroutine(Dive());
        }

        if (jumping && _isGround && diving)
        {
            if (gameObject.layer == 3) gameObject.layer = 8;
            jumping = false;
            _animator.SetTrigger("land");
            diving = false;
            divingAttack = false;
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
        if (!_werewolfAttack.canAttack || isDie || _target == null) return;
        Collider2D[] tmp;
        for(int i = 0; i<attackDetectBoxes.Length; i++)
        {
            tmp = Physics2D.OverlapBoxAll(transform.position, attackDetectBoxes[i], 0);
            foreach (var j in tmp)
            {
                if (j.CompareTag("Player") && mp >= _monsterInfo.attackMp[i])
                {
                    if (_target == null)
                    {
                        perceivePlayerTimeElapsed = _monsterInfo.perceiveTime;
                        _target = j.gameObject;
                    }

                    if (i == 1 && _isAttack) return;
                    if (i == 0)
                    {
                        int a = Random.Range(0, 2);
                        if (mp >= _monsterInfo.attackMp[a])
                        {
                            mp -= _monsterInfo.attackMp[a];
                            _attackMethods[a]();
                            return;
                        }
                        else
                        {
                            mp -= _monsterInfo.attackMp[0];
                            _attackMethods[0]();
                            return;
                        }
                    }
                    mp -= _monsterInfo.attackMp[i];
                    _attackMethods[i]();
                    return;
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
            
            _speed = 0;
            dashSpeed = atkDash * graphicTransform.localScale.x;
            _animator.SetFloat("attackNum", _werewolfAttack.consecutiveAttack);
            _animator.SetTrigger("attack");
            _werewolfAttack.SetCanAttack(false);

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
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            _capsuleCollider.sharedMaterial = little;
            
            _speed = 0;
            _animator.SetTrigger("jump");
            jumpElpased = 1f;
            _werewolfAttack.SetCanAttack(false);
            jumping = true;
            diving = false;
            
            _monsterAttack.SetAttackBox(attackBoundaryBoxes[1], attackBoundaryOffsets[1]);
            _monsterAttack.SetAttackInfo(
                _monsterInfo.attackKnockback[1], 
                _monsterInfo.atk * _monsterInfo.attackCoefficient[1],
                _monsterInfo.attackStunTime[1]);
        }
    }

    IEnumerator Dive()
    {
        float tmp = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0;
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

        Vector2 dir = (_target.transform.position - transform.position).normalized;

        _rigidbody.gravityScale = tmp;
        if (dir.y < 0)
        {
            diving = true;
            divingAttack = true;
            _animator.SetTrigger("dive");
            while (!_isGround)
            {
                yield return new WaitForFixedUpdate();
                _rigidbody.MovePosition(transform.position + (Vector3)dir * Time.fixedDeltaTime * diveSpeed);
            }

            transform.gameObject.layer = 8;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        if (col.collider.CompareTag("Player") && diving && divingAttack)
        {
            transform.gameObject.layer = 3;
            divingAttack = false;
            var i = col.collider.GetComponent<PlayerBehavior>();
            var k = ((MonsterInfo)entityInfo).attackKnockback[2];
            k.Set(i.transform.position.x < transform.position.x ? -k.x : k.x, k.y);
            i.Hit(
                ((MonsterInfo)entityInfo).atk * ((MonsterInfo)entityInfo).attackCoefficient[2],
                (k), 
                ((MonsterInfo)entityInfo).attackStunTime[2],
                transform.position);
        }
    }

    protected override void OnCollisionStay2D(Collision2D col)
    {
        base.OnCollisionStay2D(col);
        
    }
}
