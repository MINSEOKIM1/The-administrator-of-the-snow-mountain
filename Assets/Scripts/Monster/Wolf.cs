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
        dashAttack -= Time.deltaTime;
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

        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, Vector3.one*50); 
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
                        if (i == 1)
                        {
                            _graphicLocalScale.Set(1, 1, 1);
                            graphicTransform.localScale = _graphicLocalScale;
                            mp -= _monsterInfo.attackMp[0];
                            _attackMethods[0]();
                            return;
                        }
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
                        return;
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
            
            
            var tmp = Physics2D.OverlapBoxAll(transform.position, Vector2.one*50, 0);
            foreach (var j in tmp)
            {
                Wolf c;
                if ((c = j.GetComponent<Wolf>()) != null)
                {
                    c.perceivePlayerTimeElapsed = _monsterInfo.perceiveTime;
                    c._target = _target;
                }
            }
            
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
            dash = false;
            dashAttack = 1.0f;
            jumpElpased = 0.5f;
            StartCoroutine(Dash());
            

            _monsterAttack.SetAttackBox(attackBoundaryBoxes[1], attackBoundaryOffsets[1]);
            _monsterAttack.SetAttackInfo(
                _monsterInfo.attackKnockback[1], 
                _monsterInfo.atk * _monsterInfo.attackCoefficient[1],
                _monsterInfo.attackStunTime[1]);
            
            
        }
    }

    IEnumerator Dash()
    {
        Debug.Log("ASD");
        Vector2 dir = (_target.transform.position - transform.position).normalized;
        dir.Set(dir.x, 0);
        dir = dir.normalized;
        _rigidbody.AddForce(Vector2.up * entityInfo.jumpPower, ForceMode2D.Impulse);
        
        if (dir.x != 0)
        {
            dash = true;
            while (jumpElpased > 0.0f || !_isGround)
            {
                yield return new WaitForFixedUpdate();
                dashSpeed = dir.x  * dashPower;
            }

            dash = false;
            _animator.SetTrigger("land");
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);
        if (col.collider.CompareTag("Player") && dash)
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
