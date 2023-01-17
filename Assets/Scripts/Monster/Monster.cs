using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : Entity
{
    // for apply appropriate friction to player

    // Entity's info (will be replaced by PlayerInfo Class object later)
    [SerializeField] private float stateChangeInterval;
    public float maxHp;
    public float maxMp;
    public float exp;
    
    // Player's children gameObjects (player graphic, spawn location, fool position, hand position, etc.)

    // for below variable, public will be private ... (for debuging, it is public now)
    // for record current state
    public GameObject _target;
    public float hp;
    public float mp;
    

    // 0 : idle, -1 : to left, 1 : to right - attack is independent...
    public int moveState;
    public float stateChangeElapsed;
    
    // tmp (will be removed maybe...)

    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    
    
    protected override void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }


    // FixedUpdated is inherited from Entity class and no override
    
    protected override void ApplyAnimation()
    {
        return;
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

        if (stateChangeElapsed <= 0)
        {
            stateChangeElapsed = stateChangeInterval;
            moveState = Random.Range(0, 3) - 1;
        }
    }

    /*
     * according to Input...
     * update _speed
     */
    protected override void UpdateVelocity()
    {
        // if condition (!_inSlope) is omitted... player slide down a slope...
        if (!_inSlope || (_inSlope && _isAttack))
        {
            _speed = _rigidbody.velocity.x;
        }

        if (moveState != 0 && !_isAttack && Mathf.Abs(externalSpeed) < 1)
        {
            _speed += accel * moveState;
            _capsuleCollider.sharedMaterial = zero;
            _graphicLocalScale.Set( _speed >= 0 ? -1 : 1, 1, 1);
            if (Mathf.Abs(externalSpeed) < 0.1f && Mathf.Abs(dashSpeed) < 0.1f) playerGraphicTransform.localScale = _graphicLocalScale;
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
}
