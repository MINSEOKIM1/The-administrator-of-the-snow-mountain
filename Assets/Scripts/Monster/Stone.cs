using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Monster
{
    private MonsterInfo _monsterInfo;
    private delegate void Attack();
    private Attack[] _attackMethods;

    private MonsterAttack _monsterAttack;
    public GameObject projectile;
    public GameObject earthquake;
    public float projSpeed;
    public float projCount;

    
    // from Monster's info... (MonsterInfo class will be made later)
    public Vector2[] attackDetectBoxes => ((MonsterInfo)entityInfo).attackDetectBoxes;
    public Vector2[] attackBoundaryBoxes => ((MonsterInfo)entityInfo).attackBoundaryBoxes;
    public Vector2[] attackBoundaryOffsets => ((MonsterInfo)entityInfo).attackBoundaryOffsets;

    public bool hide;
    public bool hideAttack;
    public float hideElapsed;
    public float hideTime;
    
    public float shakeElapsed;
    public float shakeTime;

    public float earthquakeInterval;
    public float earthquakeElpased;
    
    public float random;
    public float[] dashPower;

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
        hideElapsed -= Time.deltaTime;
        shakeElapsed -= Time.deltaTime;
        earthquakeElpased -= Time.deltaTime;
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

                    var r = Random.Range(0, 2);
                    mp -= _monsterInfo.attackMp[r];
                    _attackMethods[r]();
                    return;
                }
            }
        }
    }

    public override void Hit(float damage, Vector2 knockback, float stunTime)
    {
        if (!hide) base.Hit(damage, knockback, stunTime);
    }
    
    public override void Hit(float damage, Vector2 knockback, float stunTime, Vector3 opponent)
    {
        if (!hide) base.Hit(damage, knockback, stunTime, opponent);
    }

    public override void Die()
    {
        _animator.SetTrigger("die");
        base.Die();
    }

    public void Attack0()
    {
        if (CanAttackLogic() && !hide)
        {
            _capsuleCollider.sharedMaterial = little;
            
            _speed = 0;
            _animator.SetTrigger("hide");
            hide = true;
            random = Random.Range(3.0f,5.0f);
            hideElapsed = random;
            
            StartCoroutine(Hide());
        }
    }

    IEnumerator Hide()
    {
        gameObject.layer = 10;
        while (hideElapsed > 0.0f && hide)
        {
            yield return new WaitForFixedUpdate();
            if(_target){
                Vector2 dir = (_target.transform.position - transform.position).normalized;
                dir.Set(dir.x, 0);
                dir = dir.normalized;
                dashSpeed = dir.x  * dashPower[0];
            }
        }

        if(hideElapsed <= 0.0f && hide)
        {
            _monsterAttack.SetAttackBox(attackBoundaryBoxes[0], attackBoundaryOffsets[0]);
            _monsterAttack.SetAttackInfo(
                _monsterInfo.attackKnockback[0], 
                _monsterInfo.atk * _monsterInfo.attackCoefficient[0],
                _monsterInfo.attackStunTime[0]);
            gameObject.layer = 8;
            dashSpeed = 0;
            _animator.SetFloat("attackNum", 0);
            _animator.SetTrigger("attack");
            Debug.Log("JUMP");
            hideAttack = false;
            hide = false;
            _speed = 0;
            hideElapsed = 3.0f;
            StartCoroutine(Hide());
            
            var proj = Instantiate(earthquake, transform.position, Quaternion.identity);
            proj.GetComponentInChildren<EffectAttack>().SetInfo(
                _monsterInfo.atk * _monsterInfo.attackCoefficient[1],
                _monsterInfo.attackStunTime[1],
                _monsterInfo.attackKnockback[1],
                Vector3.one, true);
        }
    }

    public void CloneProjectile0()
    {
        for(int i=0;i<projCount;i++){
            var proj = Instantiate(projectile, transform.position, Quaternion.identity);
            Vector2 dir = new Vector2(Random.Range(-1.0f,1.0f), Random.Range(0.0f,1.0f));
            proj.GetComponent<MonsterProjectile>().SetInfo(
                true, 
                dir * projSpeed,
                _monsterInfo.attackKnockback[0],
                _monsterInfo.atk * _monsterInfo.attackCoefficient[0],
                _monsterInfo.attackStunTime[0]);
        }
    }

    public void Attack1()
    {
        if (CanAttackLogic() && !hide)
        {
            _capsuleCollider.sharedMaterial = little;
            
            _speed = 0;
            _animator.SetTrigger("hide");
            hide = true;
            hideAttack = true;
            random = 3.0f;
            shakeElapsed = random;

            
            _monsterAttack.SetAttackBox(attackBoundaryBoxes[1], attackBoundaryOffsets[1]);
            _monsterAttack.SetAttackInfo(
                _monsterInfo.attackKnockback[1], 
                _monsterInfo.atk * _monsterInfo.attackCoefficient[1],
                _monsterInfo.attackStunTime[1]);
            
            StartCoroutine(Earthquake());
        }
    }

    IEnumerator Earthquake()
    {
        yield return new WaitForSeconds(1f);
        gameObject.layer = 10;
        Vector2 dir = (_target.transform.position - transform.position).normalized;
        dir.Set(dir.x, 0);
        dir = dir.normalized;
        while (shakeElapsed > 0.0f && hide)
        {
            yield return new WaitForFixedUpdate();
            dashSpeed = dir.x  * dashPower[1];
            if (earthquakeElpased < 0)
            {
                earthquakeElpased = earthquakeInterval;
                var proj = Instantiate(earthquake, transform.position, Quaternion.identity);
                proj.GetComponentInChildren<EffectAttack>().SetInfo(
                    _monsterInfo.atk * _monsterInfo.attackCoefficient[1],
                    _monsterInfo.attackStunTime[1],
                    _monsterInfo.attackKnockback[1],
                    Vector3.one, true);
            }
        }

        if(shakeElapsed <= 0.0f && hide){
            gameObject.layer = 8;
            dashSpeed = 0;
            _animator.SetTrigger("out");
            hideAttack = false;
            hide = false;
            _speed = 0;
            shakeElapsed = 5.0f;
        }
    }
}
