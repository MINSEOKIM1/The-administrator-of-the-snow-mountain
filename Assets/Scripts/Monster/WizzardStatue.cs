using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizzardStatue : Monster
{
    public MonsterInfo _monsterInfo;
    private delegate void Attack();
    private Attack[] _attackMethods;

    private MonsterAttack _monsterAttack;

    // from Monster's info... (MonsterInfo class will be made later)
    public Vector2[] attackDetectBoxes => ((MonsterInfo)entityInfo).attackDetectBoxes;
    public Vector2[] attackBoundaryBoxes => ((MonsterInfo)entityInfo).attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets => ((MonsterInfo)entityInfo).attackBoundaryOffsets;
    
    // tmp variable for store current state
    public GameObject[] effects;

    public int bulletCount;
    public Transform bulletSpawner;
    public float bulletSpeed;

    public float lighteningCount, lighteningTime;


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
        if (_isAttack || isDie) return;
        Collider2D[] tmp;

        if (mp >= _monsterInfo.attackMp[0])
        {
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

                    int d = Random.Range(0, 2);

                    mp -= _monsterInfo.attackMp[d];
                    _attackMethods[d]();
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
           
            _animator.SetTrigger("attack");
            StartCoroutine(SpawnBullets(bulletCount, bulletSpawner.position, 1f));
            GameManager.Instance.AudioManager.PlaySfx(40);

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

            _speed = 0;
           
            _animator.SetTrigger("attack");
            StartCoroutine(SpawnLightening((int)lighteningCount, lighteningTime));
           

            _monsterAttack.SetAttackBox(attackBoundaryBoxes[0], attackBoundaryOffsets[0]);
            _monsterAttack.SetAttackInfo(
                _monsterInfo.attackKnockback[0], 
                _monsterInfo.atk * _monsterInfo.attackCoefficient[0],
                _monsterInfo.attackStunTime[0]);
        }
    }

    public IEnumerator SpawnBullets(int count, Vector2 pos, float time)
    {
        yield return new WaitForSeconds(time);

        for (float j = 0; j <= 360; j += 360.0f / count)
        {
            var i = Instantiate(effects[0], pos , Quaternion.identity);
            i.GetComponentInChildren<MonsterProjectile>().SetInfo(
                false, new Vector2(Mathf.Cos(j*Mathf.Deg2Rad), Mathf.Sin(j*Mathf.Deg2Rad)) * bulletSpeed,
                _monsterInfo.attackKnockback[0], _monsterInfo.atk * _monsterInfo.attackCoefficient[0],
                _monsterInfo.attackStunTime[0], true);
        }
    }

    public IEnumerator SpawnLightening(int count, float time)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = _target.transform.position;
            var aa = Physics2D.RaycastAll(_target.transform.position, Vector2.down, 100);
            foreach (var a in aa)
            {
                if (a.collider.CompareTag("Ground"))
                {
                    pos = a.point;
                    break;
                }
            }
            var j = Instantiate(effects[1], pos, Quaternion.identity);
            j.GetComponentInChildren<EffectAttack>().SetInfo(
                _monsterInfo.atk * _monsterInfo.attackCoefficient[1], 
                _monsterInfo.attackStunTime[1],
                _monsterInfo.attackKnockback[1]);
            
            yield return new WaitForSeconds(time);
        }
    }
    
    protected override void OnCollisionStay2D(Collision2D col)
    {
        base.OnCollisionStay2D(col);
        
    }
}
