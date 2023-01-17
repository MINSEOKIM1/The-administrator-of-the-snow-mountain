using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    // for apply appropriate friction to player
    [SerializeField] private PhysicsMaterial2D little, zero;
    
    
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;
    [SerializeField] private float jumpPower;
    [SerializeField] private float stateChangeInterval;
    
    // Player's children gameObjects (player graphic, spawn location, fool position, hand position, etc.)
    [SerializeField] private Transform footPos;
    [SerializeField] private Transform playerGraphicTransform;
    
    // for below variable, public will be private ... (for debuging, it is public now)
    // for record current state
    public float _speed;
    public bool _isGround;
    private bool _canJump;
    public bool _inSlope;
    private bool _isAttack;
    private bool _canAttack;
    public float playerDashSpeed;
    public float externalSpeed;

    // 0 : idle, -1 : to left, 1 : to right - attack is independent...
    public int moveState;
    public float stateChangeElapsed;
    
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private CapsuleCollider2D _capsuleCollider;
    private SpriteRenderer _sprite;

    // tmp (will be removed maybe...)
    public float down;
    public float footRadius;
    public Vector2 knockback;
    public float friction;
    public LayerMask ground;
    
    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    private Vector3 _graphicLocalScale;
    private Vector2 _velocity;
    private Vector2 _groundNormalPerp;

    public GameObject _target;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(footPos.position, Vector3.down * down);
        Gizmos.DrawWireSphere(footPos.position, footRadius);
    }
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        ApplyAnimation();
        CheckGround();
        DetermineNextMove();
        UpdateVelocity();
        UpdateExternalVelocity();
        Move(_groundNormalPerp, _speed);
    }
    
    private void ApplyAnimation()
    {
        return;
        
        _animator.SetFloat("speed", _speed);
        _animator.SetBool("isGround", _isGround);
        _animator.SetBool("isFalling", !_isGround && _rigidbody.velocity.y < 0);
    }

    /*
     * Check if...
     * 1. isGround?
     * 2. Slope?
     */
    private void CheckGround()
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

    private void DetermineNextMove()
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
    private void UpdateVelocity()
    {
        // if condition (!_inSlope) is omitted... player slide down a slope...
        if (!_inSlope || (_inSlope && _isAttack))
        {
            _speed = _rigidbody.velocity.x;
        }

        if (moveState != 0 && !_isAttack)
        {
            _speed += accel * moveState;
            _capsuleCollider.sharedMaterial = zero;
            _graphicLocalScale.Set( _speed >= 0 ? -1 : 1, 1, 1);
            if (Mathf.Abs(externalSpeed) < 0.1f && Mathf.Abs(playerDashSpeed) < 0.1f) playerGraphicTransform.localScale = _graphicLocalScale;
        }
        else
        {
            if (_isGround && Mathf.Abs(_speed) < 0.1) _capsuleCollider.sharedMaterial = little;
            _speed = Mathf.Lerp(_speed, 0, friction * Time.fixedDeltaTime);
            if (Mathf.Abs(_speed) < 0.1) _speed = 0;
        }

        _speed = Mathf.Clamp(_speed, -maxSpeed, maxSpeed);
    }

    private void UpdateExternalVelocity()
    {
        playerDashSpeed = Mathf.Lerp(playerDashSpeed, 0, friction * Time.fixedDeltaTime);
        if (_isGround) externalSpeed = Mathf.Lerp(externalSpeed, 0, friction * Time.fixedDeltaTime);
    }
    /*
     * according to _speed, playerDashSpeed, externalSpeed, ground checking condition...
     * update rigid.velocity -> apply change on real player's movement;
     */
    private void Move(Vector2 direction, float speed)
    {
        if (_isGround)
        {
            
            if (_inSlope)
            {
                _velocity.Set(
                    -(speed + playerDashSpeed + externalSpeed) * direction.x,
                    Mathf.Clamp(-direction.y * (speed + playerDashSpeed + externalSpeed), -10, Single.PositiveInfinity));
            }
            else
            {
                _velocity.Set(
                    speed + playerDashSpeed + externalSpeed,
                    Mathf.Clamp(_rigidbody.velocity.y, -10, Single.PositiveInfinity));
            }
        }
        else
        {
            _velocity.Set(
                speed + playerDashSpeed + externalSpeed, 
                Mathf.Clamp(_rigidbody.velocity.y, -10, Single.PositiveInfinity));
        }
        
        _rigidbody.velocity = _velocity;
    }
    
    public void KnockBack(Vector2 knockback)
    {
        _speed = 0;
        _isGround = false;
        _canJump = false;
        _rigidbody.AddForce(knockback.y * Vector2.up, ForceMode2D.Impulse);
        externalSpeed = knockback.x;
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Ground") )
        {
            if (col.contacts[0].normal.y > 0.7) _isGround = true;
            else
            {
                externalSpeed = 0;
                playerDashSpeed = 0;
            }
        }
    }
    
    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Ground"))
        {
            if (col.contacts[0].normal.y > 0.7) _isGround = true;
            else
            {
                externalSpeed = 0;
                playerDashSpeed = 0;
            }
        }
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag.Equals("Ground") && col.contacts[0].normal.y > 0.7)
        {
            _isGround = false;
        }
    }
}
