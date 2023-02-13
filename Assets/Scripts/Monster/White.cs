using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class White : Monster
{
    private MonsterInfo _monsterInfo;
    private delegate void Attack();
    private Attack[] _attackMethods;

    private MonsterAttack _monsterAttack;

    public GameObject projectile;
    public float projSpeed;
    
    // from Monster's info... (MonsterInfo class will be made later)
    public Vector2[] attackDetectBoxes => ((MonsterInfo)entityInfo).attackDetectBoxes;
    public Vector2[] attackBoundaryBoxes => ((MonsterInfo)entityInfo).attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets => ((MonsterInfo)entityInfo).attackBoundaryOffsets;

    protected override void Start()
    {
        base.Start();
        _monsterInfo = (MonsterInfo)entityInfo;
        _monsterAttack = GetComponentInChildren<MonsterAttack>();
        _attackMethods = new Attack[2];
        _attackMethods[0] = Attack0;
        _attackMethods[1] = Attack1;
        _monsterAttack.projectileMethod += (() => CloneProjectile0());
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
            Debug.Log("White Attack0");
            
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
            
            _speed = 0;
            _animator.SetFloat("attackNum", 1);
            _animator.SetTrigger("attack");
            Debug.Log("White Attack1");
        }
    }

    public void CloneProjectile0()
    {
        var proj = Instantiate(projectile, transform.position, Quaternion.identity);
        var dir = (_target.transform.position - transform.position).normalized;
        proj.GetComponent<MonsterProjectile>().SetInfo(
            false, 
            dir * projSpeed,
            _monsterInfo.attackKnockback[1],
            _monsterInfo.atk * _monsterInfo.attackCoefficient[1],
            _monsterInfo.attackStunTime[1]);
    }
}
