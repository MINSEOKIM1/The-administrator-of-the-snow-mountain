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
    

    // Monster's children gameObjects (player graphic, spawn location, fool position, hand position, etc.)
    

    // for below variable, public will be private ... (for debuging, it is public now)
    // for record current state
    public GameObject _target;


    // 0 : idle, -1 : to left, 1 : to right - attack is independent...
    public int moveState;
    public float stateChangeElapsed;
    
    // tmp (will be removed maybe...)
    public Slider hpbar;

    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    private Vector2 _boxOffsetWithLocalscale;
    
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        _boxOffsetWithLocalscale.Set(boxOffset.x * graphicTransform.localScale.x, boxOffset.y);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + _boxOffsetWithLocalscale, boxSize);
    }
    protected override void Start()
    {
        hp = maxHp;
        mp = maxMp;
        
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
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
        // try to detect Player
        _boxOffsetWithLocalscale.Set(boxOffset.x * graphicTransform.localScale.x, boxOffset.y);
        var colliders = Physics2D.OverlapBoxAll(
            transform.position + (Vector3)_boxOffsetWithLocalscale,
            boxSize,
            0);

        foreach (var i in colliders)
        {
            if (i.CompareTag("Player")) _target = i.gameObject;
        }

        stateChangeElapsed -= Time.fixedDeltaTime;

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
    
    public void Hit(float damage, Vector2 knockback, float stunTime, Vector3 opponent)
    {
        if (isDie) return;
        base.Hit(damage, knockback, stunTime);
        GameManager.Instance.EffectManager.CreateEffect(1, transform.position,
            Quaternion.AngleAxis(
                Mathf.Atan2((opponent-transform.position).y, (opponent-transform.position).x) * Mathf.Rad2Deg, 
                Vector3.forward));
        hpbar.value = hp / maxHp;
    }

    public override void Die()
    {
        if (isDie) return;
        _animator.SetTrigger("die");
        _speed = 0;
        dashSpeed = 0;
        gameObject.layer = 10;
        _capsuleCollider.sharedMaterial = little;
        isDie = true;
    }
}
