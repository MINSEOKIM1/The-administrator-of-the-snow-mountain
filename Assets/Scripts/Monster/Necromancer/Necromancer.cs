using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Monster
{
    private MonsterInfo _monsterInfo;
    private delegate void Attack();
    private Attack[] _attackMethods;

    private MonsterAttack _monsterAttack;
    private NecromancerAttackSignal _necromancerAttack;
    
    public Transform spawnPos;
    
    // from Monster's info... (MonsterInfo class will be made later)
    public Vector2[] attackDetectBoxes => ((MonsterInfo)entityInfo).attackDetectBoxes;
    public Vector2[] attackBoundaryBoxes => ((MonsterInfo)entityInfo).attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets => ((MonsterInfo)entityInfo).attackBoundaryOffsets;
    
    // tmp variable for store current state
    public bool jumping;
    public float flySpeed;
    public float jumpElpased;
    public GameObject projectile;
    public float projSpeed;
    public float height;
    public float armLength;
    public GameObject obj;
    public GameObject summon;
    public bool[] canSkill;
    public float[] coolDown;
    private bool lowHealth;
    public int lowHealthCoefficient = 1;

    protected override void Start()
    {
        base.Start();
        _monsterInfo = (MonsterInfo)entityInfo;
        _monsterAttack = GetComponentInChildren<MonsterAttack>();
        _necromancerAttack = GetComponentInChildren<NecromancerAttackSignal>();
        _attackMethods = new Attack[2];
        _attackMethods[0] = Attack0;
        _attackMethods[1] = Attack1;
        canSkill = new bool[attackDetectBoxes.Length];
        for(int i=0;i<canSkill.Length;i++){
            canSkill[i] = true;
        }
        _necromancerAttack.CanAttack();
        _monsterAttack.projectileMethod += (() => CloneProjectile0());
    }

    protected override void Update()
    {
        base.Update();
        AttackCheck();
        _animator.SetFloat("speed", _speed);
        jumpElpased -= Time.deltaTime;

        if (jumping && jumpElpased > 0)
        {
            StartCoroutine(Fly());
        }
        if(jumping && jumpElpased < 0){
            jumping = false;
        }

        if(!lowHealth && hp < maxHp/2){
            lowHealth = true;
            _animator.SetFloat("lowHP", 1);
            lowHealthCoefficient = 2;
            Debug.Log("Low Health");
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
        if (!_necromancerAttack.canAttack || isDie || _target == null) return;
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
        if (CanAttackLogic() && canSkill[0])
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
            _animator.SetFloat("attackNum", 0);
            _animator.SetTrigger("attack");
            _necromancerAttack.SetCanAttack(false);
            canSkill[0] = false;
            StartCoroutine(SkillCooldown(coolDown[0], 0));

        }
    }

    public GameObject explosionObj; 
    public float explosionDuration;
    
    public void CloneProjectile0()
    {
        var dir = (_target.transform.position - transform.position).normalized;
        var proj = Instantiate(projectile, spawnPos.position, Quaternion.identity);
        dir = (_target.transform.position - spawnPos.position).normalized;
        proj.GetComponentInChildren<MonsterProjectile>().SetInfo(
            false, dir * projSpeed,
            _monsterInfo.attackKnockback[0], _monsterInfo.atk * _monsterInfo.attackCoefficient[0],
            _monsterInfo.attackStunTime[0], true);
    }

    
    public void Attack1()
    {
        if (CanAttackLogic() && canSkill[1])
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
            jumpElpased = 5.0f;
            _animator.SetFloat("attackNum", 1);
            _animator.SetTrigger("attack");
            _necromancerAttack.SetCanAttack(false);
            jumping = true;
            Invoke("InstantiateGameObject", 2.0f);
            canSkill[1] = false;
            StartCoroutine(SkillCooldown(coolDown[1], 1));
            
        }
    }

    IEnumerator Fly()
    {

        while (jumpElpased > 3)
        {
            yield return new WaitForFixedUpdate();
            _rigidbody.MovePosition(transform.position + Vector3.up * Time.fixedDeltaTime * flySpeed);
        }
        while (jumpElpased <= 3 && jumpElpased > 1)
        {
            yield return new WaitForFixedUpdate();
            _rigidbody.MovePosition(transform.position + Vector3.up * Time.fixedDeltaTime * flySpeed * 0.3f);
        }
        
        
    }
    public int lightningRepeat;
    public float lightningDistanceInterval;
    public float lightningPosY;
    public float lightningDuration;
    public float lightningTimeInterval;

    public void InstantiateGameObject()
    {
        var pos = _target.transform.position;
        pos.y += lightningPosY;
        var _obj = Instantiate(obj, pos, Quaternion.identity);
        Destroy(_obj, lightningDuration);
        StartCoroutine(delayLightning(lightningTimeInterval, pos));
    }

    IEnumerator delayLightning(float delayTime, Vector3 pos)
    {
        int repeat = lightningRepeat;
        if(lowHealth){
            repeat = lightningRepeat + 2;
        }

        bool a = false;
        for(int i=1; i< repeat; i++){
            yield return new WaitForSeconds(delayTime);
            if (!a)
            {
                a = true;
                Instantiate(summon, pos, Quaternion.identity);
            }
            if(lowHealth && i == 2){
                Instantiate(summon, pos + Vector3.right * i * lightningDistanceInterval, Quaternion.identity);
                Instantiate(summon, pos + Vector3.left * i * lightningDistanceInterval, Quaternion.identity);
            }
            var _objR = Instantiate(obj, pos + Vector3.right * i * lightningDistanceInterval, Quaternion.identity);
            var _objL = Instantiate(obj, pos + Vector3.left * i * lightningDistanceInterval, Quaternion.identity);
            Destroy(_objR, lightningDuration);
            Destroy(_objL, lightningDuration);
        }
    }

    IEnumerator SkillCooldown(float cooldown, int skillNum)
    {
        yield return new WaitForSeconds(cooldown);
        canSkill[skillNum] = true;
    }
}
