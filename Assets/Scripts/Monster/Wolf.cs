using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Monster
{
    private MonsterInfo _monsterInfo;
    private delegate void Attack();
    private Attack[] _attackMethods;

    private MonsterAttack _monsterAttack;

    
    // from Monster's info... (MonsterInfo class will be made later)
    public Vector2[] attackDetectBoxes => ((MonsterInfo)entityInfo).attackDetectBoxes;
    public Vector2[] attackBoundaryBoxes => ((MonsterInfo)entityInfo).attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets => ((MonsterInfo)entityInfo).attackBoundaryOffsets;

    public bool dash;
    public float dashAttack;
    public float dashPower;
    public float jumpElpased;

    protected override void Start()
    {
        base.Start();
        _monsterInfo = (MonsterInfo)entityInfo;
        _monsterAttack = GetComponentInChildren<MonsterAttack>();
        _attackMethods = new Attack[2];
        _attackMethods[0] = Attack0;
        _attackMethods[1] = Attack1;
    }

    protected override void Update()
    {
        base.Update();
        AttackCheck();
        jumpElpased -= Time.deltaTime;
        
        if (!dash && dashAttack > 0.0f)
        {
            StartCoroutine(Dash());
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
        if (_isAttack || isDie) return;
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

                    if (Random.Range(0, 1f) < 0.6f)
                    {
                        mp -= _monsterInfo.attackMp[i];
                        _attackMethods[i]();
                    }
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
            _capsuleCollider.sharedMaterial = little;
            
            _animator.SetFloat("attackNum", 1);
            _animator.SetTrigger("attack");
            
            Debug.Log("Attack1");
            _rigidbody.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
            dash = false;
            dashAttack += 1.0f;
            jumpElpased = 0.5f;

            _monsterAttack.SetAttackBox(attackBoundaryBoxes[1], attackBoundaryOffsets[1]);
            _monsterAttack.SetAttackInfo(
                _monsterInfo.attackKnockback[1], 
                _monsterInfo.atk * _monsterInfo.attackCoefficient[1],
                _monsterInfo.attackStunTime[1]);
            
            
        }
    }

    IEnumerator Dash()
    {
        float tmp = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0;
        
        Vector2 dir = (_target.transform.position - transform.position).normalized;

        _rigidbody.gravityScale = tmp;
        if (dir.x != 0)
        {
            dash = true;
            while (dash && jumpElpased > 0.0f)
            {
                yield return new WaitForFixedUpdate();
                _rigidbody.MovePosition(transform.position + (Vector3)dir * Time.fixedDeltaTime * dashPower);
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        if (col.collider.CompareTag("Player") && dash && dashAttack > 0.0f && jumpElpased> 0.0f)
        {
            dash = false;
            dashAttack = 0.0f;
            jumpElpased = 0.0f;
            var i = col.collider.GetComponent<PlayerBehavior>();
            var k = ((MonsterInfo)entityInfo).attackKnockback[1];
            k.Set(i.transform.position.x < transform.position.x ? -k.x : k.x, k.y);
            i.Hit(
                ((MonsterInfo)entityInfo).atk * ((MonsterInfo)entityInfo).attackCoefficient[1],
                (k), 
                ((MonsterInfo)entityInfo).attackStunTime[1],
                transform.position);
        }
    }

}
