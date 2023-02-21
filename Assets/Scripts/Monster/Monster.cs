using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Monster : Entity
{
    // for apply appropriate friction to player

    // Entity's info (will be replaced by PlayerInfo Class object later)
    protected float stateChangeInterval => ((MonsterInfo)entityInfo).stateChangeInterval;
    public Vector2 boxSize => ((MonsterInfo)entityInfo).boxSize;
    public Vector2 boxOffset => ((MonsterInfo)entityInfo).boxOffset;

    public new float maxSpeed;
    // Monster's children gameObjects (player graphic, spawn location, fool position, hand position, etc.)
    

    // for below variable, public will be private ... (for debuging, it is public now)
    // for record current state
    public GameObject _target;
    public float[] groundCheckLength;
    public float perceivePlayerTimeElapsed;


    // 0 : idle, -1 : to left, 1 : to right - attack is independent...
    public int moveState;
    public float stateChangeElapsed;

    // tmp (will be removed maybe...)
    public Slider hpbar;
    public Slider mpbar;

    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    public Vector2 _boxOffsetWithLocalscale;
    
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        _boxOffsetWithLocalscale.Set(boxOffset.x * graphicTransform.localScale.x, boxOffset.y);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + _boxOffsetWithLocalscale, boxSize);
        Gizmos.DrawLine(
            (Vector2)transform.position + _boxOffsetWithLocalscale*groundCheckLength[0], 
            (Vector2)transform.position + _boxOffsetWithLocalscale*groundCheckLength[0] + Vector2.down*groundCheckLength[1]);
        
    }
    protected override void Start()
    {
        hp = maxHp;
        mp = maxMp;

        maxSpeed = entityInfo.maxSpeed * (0.5f + Random.Range(0f, 1f) * 1.3f);

        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();
        hpbar.value = hp / maxHp;
        mpbar.value = mp / maxMp;
        hp += Time.deltaTime * entityInfo.hpIncRate;
        hp = Mathf.Clamp(hp, 0, entityInfo.maxHp);
        mp += Time.deltaTime * entityInfo.mpIncRate;
        mp = Mathf.Clamp(mp, 0, entityInfo.maxMp);
        if (_rigidbody.velocity.x < Mathf.Epsilon && _rigidbody.velocity.x > -Mathf.Epsilon)
            _capsuleCollider.sharedMaterial = zero;
    }

    // FixedUpdated is inherited from Entity class and no override
    // protected override void FixedUpdate() { ... }
    
    /*
     * Method Clamping hp, mp, etc.
     */
    protected override void ValueClamp()
    {
        hp = Mathf.Clamp(hp, 0, maxHp);
        mp = Mathf.Clamp(mp, 0, maxMp);
    }

    /*
     * Check if...
     * 1. isGround?
     * 2. Slope?
     */
    // Method CheckGround() is inherited from Entity class and no override

    protected override void DetermineNextMove()
    {
        stateChangeElapsed -= Time.fixedDeltaTime;
        perceivePlayerTimeElapsed -= Time.fixedDeltaTime;

        if (perceivePlayerTimeElapsed < 0) _target = null;
        
        // try to detect Player
        _boxOffsetWithLocalscale.Set(boxOffset.x * graphicTransform.localScale.x, boxOffset.y);

        if (_target == null)
        {
            var colliders = Physics2D.OverlapBoxAll(
                transform.position + (Vector3)_boxOffsetWithLocalscale,
                boxSize,
                0);

            foreach (var i in colliders)
            {
                if (i.CompareTag("Player"))
                {
                    _target = i.gameObject;
                    perceivePlayerTimeElapsed = ((MonsterInfo)entityInfo).perceiveTime;
                }
            }
        }
        
        // 안 떨어지게
        bool ok = false;
        var hits = Physics2D.RaycastAll(
            (Vector2)transform.position + _boxOffsetWithLocalscale*groundCheckLength[0], 
            Vector2.down, groundCheckLength[1]);
        
        

        foreach (var VARIABLE in hits)
        {
            if (VARIABLE.collider.CompareTag("Ground")) ok = true;
        }

        if (!ok && !_isAttack)
        {
            if (moveState != 0 && _graphicLocalScale.x * moveState < 0)
            {
                stateChangeElapsed = stateChangeInterval / 2;
                moveState = -moveState;
            }
        }
        

        // there is no _target, choose next move randomly
        if (stateChangeElapsed <= 0)
        {
            if (_target == null)
            {
                stateChangeElapsed = stateChangeInterval;
                moveState = Random.Range(0, 3) - 1;

            }
            else
            {
                if (_target.transform.position.x < transform.position.x)
                {
                    stateChangeElapsed = stateChangeInterval / 2;
                    moveState = -1;
                }
                else
                {
                    stateChangeElapsed = stateChangeInterval / 2;
                    moveState = 1;
                }
            }
        }
    }


    /*
     * according to Input...
     * update _speed
     */
    protected override void UpdateVelocity()
    {
        // if condition (!_inSlope) is omitted... player slide down a slope...
        if (!_inSlope || (_inSlope && _isAttack) || isDie)
        {
            _speed = _rigidbody.velocity.x;
        }

        if (moveState != 0 && !_isAttack && !_hitAir && stunTimeElapsed <= 0)
        {
            _speed += accel * moveState;
            _capsuleCollider.sharedMaterial = zero;
            _graphicLocalScale.Set( _speed >= 0 ? -1 : 1, 1, 1);
            if (Mathf.Abs(externalSpeed) < 0.1f && Mathf.Abs(dashSpeed) < 0.1f) graphicTransform.localScale = _graphicLocalScale;
        }
        else
        {
            if (_isGround && Mathf.Abs(_speed) < 0.1) _capsuleCollider.sharedMaterial = little;
            _speed = Mathf.Lerp(_speed, 0, friction * Time.fixedDeltaTime);
            if (Mathf.Abs(_speed) < 0.1) _speed = 0;
        }

        _speed = Mathf.Clamp(_speed, -maxSpeed, maxSpeed);
    }

    // Method UpdateExternalVelocity() is inherited by Entity class and no override
    
    /*
     * according to _speed, playerDashSpeed, externalSpeed, ground checking condition...
     * update rigid.velocity -> apply change on real player's movement;
     */
    // Method Move() is inherited from Entity class and no override

    public override void Hit(float damage, Vector2 knockback, float stunTime)
    {
        if (isDie) return;
        base.Hit(damage, knockback, stunTime);
        GameManager.Instance.EffectManager.CreateEffect(1, transform.position,
            Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward));
        hpbar.value = hp / maxHp;
    }
    
    public virtual void Hit(float damage, Vector2 knockback, float stunTime, Vector3 opponent)
    {
        if (isDie) return;
        base.Hit(damage, GetKnockback(knockback, entityInfo.stance), stunTime);
        GameManager.Instance.AudioManager.PlaySfx(4);
        GameManager.Instance.EffectManager.CreateEffect(1, transform.position,
            Quaternion.AngleAxis(
                Mathf.Atan2((opponent-transform.position).y, (opponent-transform.position).x) * Mathf.Rad2Deg, 
                Vector3.forward));
        hpbar.value = hp / maxHp;
    }

    public override void Die()
    {
        if (isDie) return;
        GameManager.Instance.PlayerDataManager.exp += ((MonsterInfo)entityInfo).exp;
        GetComponent<AIPlayer>().MobDie();
        _animator.SetTrigger("die");
        _speed = 0;
        dashSpeed = 0;
        gameObject.layer = 10;
        _capsuleCollider.sharedMaterial = little;
        for (int i = 0; i < ((MonsterInfo)entityInfo).dropItems.Length; i++)
        {
            if (Random.Range(0, 1f) < ((MonsterInfo)entityInfo).itemDropProbability[i])
            {
                GameManager.Instance.EffectManager.CreateItem(
                    ((MonsterInfo)entityInfo).dropItems[i],
                    ((MonsterInfo)entityInfo).itemDropCount[i],
                    transform.position,
                    Quaternion.identity);
            }
        }
        isDie = true;
    }

    public IEnumerator SpawnInit()
    {
        if (_sprite == null) _sprite = GetComponentInChildren<SpriteRenderer>();
        _sprite.material.color = new Color(1, 1, 1, 0);
        var c = new Color(1, 1, 1, 0);
        while (_sprite.material.color.a < 1)
        {
            c.a += 0.02f;
            _sprite.material.color = c;
            yield return new WaitForFixedUpdate();
        }
    }
}
