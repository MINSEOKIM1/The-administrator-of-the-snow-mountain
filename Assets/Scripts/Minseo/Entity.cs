using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class Entity : MonoBehaviour
{
    // for apply appropriate friction to player
    public PhysicsMaterial2D little, zero;
    
    // Entity's info (will be replaced by PlayerInfo Class object later)
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float accel;
    [SerializeField] protected float jumpPower;
    [SerializeField] protected Vector2 backStepPower;
    
    // Entity's children gameObjects (player graphic, spawn location, fool position, hand position, etc.)
    [SerializeField] protected Transform footPos;
    [SerializeField] protected Transform playerGraphicTransform;
    
    // Entity gameObject's components
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;
    protected CapsuleCollider2D _capsuleCollider;
    protected SpriteRenderer _sprite;

    // for below variable, public will be private ... (for debugging, it is public now)
    // for record current state
    public float _speed;
    public bool _isGround;
    protected bool _canJump;
    public bool _inSlope;
    protected bool _isAttack;
    protected bool _canAttack;
    public float dashSpeed;
    public float externalSpeed;
    

    // tmp (will be removed maybe...)
    public float down;
    public float footRadius;
    public Vector2 knockback;
    public float friction;
    public LayerMask ground;
    
    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    protected Vector3 _graphicLocalScale;
    protected Vector2 _velocity;
    protected Vector2 _groundNormalPerp;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(footPos.position, Vector3.down * down);
        Gizmos.DrawWireSphere(footPos.position, footRadius);
    }

    protected virtual void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        return;
    }
    
    protected virtual void FixedUpdate()
    {
        ApplyAnimation();
        CheckGround();
        DetermineNextMove();
        UpdateVelocity();
        AttackInputDetect();
        UpdateExternalVelocity();
        Move(_groundNormalPerp, _speed);
    }
    
    protected virtual void ApplyAnimation()
    {
        return;
    }

    /*
     * Check if...
     * 1. isGround?
     * 2. Slope?
     */
    protected virtual void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(footPos.position, Vector2.down, down, ground);
        Debug.DrawRay(hit.point, _groundNormalPerp);
        if (hit)
        {
            _groundNormalPerp = Vector2.Perpendicular(hit.normal);
            if (_isGround)
            {
                // no slope
                if (_groundNormalPerp.y == 0)
                {
                    _inSlope = false;
                    _isGround = true;
                    _canJump = true;
                }
                // available slope
                else if (Vector2.Angle(Vector2.up, hit.normal) < 45)
                {
                    _inSlope = true;
                    _isGround = true;
                    _canJump = true;
                }
                // over maximum slope angle
                else
                {
                    _inSlope = false;
                    _isGround = false;
                    _canJump = false;
                }
            } else if (_rigidbody.velocity.y <= 0)
            {
                // no slope
                if (_groundNormalPerp.y == 0)
                {
                    _inSlope = false;
                    _isGround = true;
                    _canJump = true;
                }
                // available slope
                else if (Vector2.Angle(Vector2.up, hit.normal) < 45)
                {
                    _inSlope = true;
                    _isGround = true;
                    _canJump = true;
                }
            }
        }
        else
        {
            _inSlope = false;
            _isGround = false;
            _canJump = false;
        }
    }

    protected virtual void DetermineNextMove()
    {
        return;
    }

    /*
     * according to Input...
     * update _speed
     */
    protected virtual void UpdateVelocity()
    {
        return;
    }

    protected virtual void UpdateExternalVelocity()
    {
        dashSpeed = Mathf.Lerp(dashSpeed, 0, friction * Time.fixedDeltaTime);
        if (_isGround) externalSpeed = Mathf.Lerp(externalSpeed, 0, friction * Time.fixedDeltaTime);
    }

    protected virtual void AttackInputDetect()
    {
        return;
    }
    
    /*
     * according to _speed, playerDashSpeed, externalSpeed, ground checking condition...
     * update rigid.velocity -> apply change on real player's movement;
     */
    protected virtual void Move(Vector2 direction, float speed)
    {
        if (_isGround)
        {
            
            if (_inSlope)
            {
                _velocity.Set(
                    -(speed + dashSpeed + externalSpeed) * direction.x,
                    Mathf.Clamp(-direction.y * (speed + dashSpeed + externalSpeed), -10, Single.PositiveInfinity));
            }
            else
            {
                _velocity.Set(
                    speed + dashSpeed + externalSpeed,
                    Mathf.Clamp(_rigidbody.velocity.y, -10, Single.PositiveInfinity));
            }
        }
        else
        {
            _velocity.Set(
                speed + dashSpeed + externalSpeed, 
                Mathf.Clamp(_rigidbody.velocity.y, -10, Single.PositiveInfinity));
        }
        
        _rigidbody.velocity = _velocity;
    }
    
    public virtual void Jump()
    {
        if (_canJump && Mathf.Abs(externalSpeed) < 1 && !_isAttack)
        {
            externalSpeed = 0;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _isGround = false;
            _canJump = false;
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    public virtual void KnockBack(Vector2 knockback)
    {
        _speed = 0;
        _isGround = false;
        _canJump = false;
        _rigidbody.AddForce(knockback.y * Vector2.up, ForceMode2D.Impulse);
        externalSpeed = knockback.x;
    }
}
