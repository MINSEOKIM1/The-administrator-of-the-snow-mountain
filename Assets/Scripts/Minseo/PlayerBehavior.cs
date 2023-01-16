using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private PhysicsMaterial2D little, zero;
    [SerializeField] private Transform playerGraphicTransform;
    
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;
    [SerializeField] private float friction;
    [SerializeField] private float jumpPower;

    [SerializeField] private Transform footPos;

    [SerializeField] private LayerMask ground;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private PlayerInputHandler _playerInputHandler;
    private CapsuleCollider2D _capsuleCollider;
    private SpriteRenderer _sprite;
    private PlayerAttack _playerAttack;

    // public will be private ... (for debuging, it is public now)
    public float _speed;
    public bool _isGround;
    private bool _canJump;
    public bool _inSlope;
    private bool _isAttack;
    private bool _canAttack;
    public bool _isKnockback;

    // tmp (will be removed maybe...)
    public float down;
    public float footRadius;
    public float go;
    public Vector2 knockback;
    
    // tmp variable
    private Vector3 _graphicLocalScale;
    private Vector2 _velocity;
    private Vector2 _groundNormalPerp;
    
    public float playerDashSpeed;
    public float externalSpeed;

    // input detect variable
    private bool _normalAttackDetect;

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
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _playerAttack = GetComponentInChildren<PlayerAttack>();
    }

    private void Update()
    {
        AttackCheck();
    }

    // Physics logic...
    private void FixedUpdate()
    {
        ApplyAnimation();
        CheckGround();                      
        UpdateVelocity();
        AttackInputDetect();
        UpdateExternalVelocity();
        Move(_groundNormalPerp, _speed);
    }

    private void AttackCheck()
    {
        _isAttack = _animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");
    }

    private void ApplyAnimation()
    {
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

        if (_playerInputHandler.movement.x != 0 && !_isAttack)
        {
            _speed += accel * _playerInputHandler.movement.x;
            _capsuleCollider.sharedMaterial = zero;
            _graphicLocalScale.Set( _speed >= 0 ? -1 : 1, 1, 1);
            playerGraphicTransform.localScale = _graphicLocalScale;
        }
        else
        {
            if (_isGround && Mathf.Abs(_speed) < 0.1) _capsuleCollider.sharedMaterial = little;
            _speed = Mathf.Lerp(_speed, 0, friction * Time.fixedDeltaTime);
            if (Mathf.Abs(_speed) < 0.1) _speed = 0;
        }

        _speed = Mathf.Clamp(_speed, -maxSpeed, maxSpeed);
    }
    
    private void AttackInputDetect()
    {
        // Normal Attack
        if (_normalAttackDetect)
        {
            _capsuleCollider.sharedMaterial = little;
            _normalAttackDetect = false;
            
            // player stop moving (_speed = 0), and dash while attacking (in _playerAttack.NormalAttack);
            _speed = 0;
            _playerAttack.NormalAttack();
        }
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

    public void Jump()
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

    public void Attack()
    {
        _normalAttackDetect = true;
    }

    public void KnockBack()
    {
        _speed = 0;
        _isKnockback = true;
        _isGround = false;
        _canJump = false;
        _rigidbody.AddForce(knockback.y * Vector2.up, ForceMode2D.Impulse);
        externalSpeed = knockback.x * _playerAttack.transform.localScale.x;
    }

    
    // OnCollision Methods..
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Ground") )
        {
            if (col.contacts[0].normal.y > 0.7) _isGround = true;
            else externalSpeed = 0;
        }
    }
    
    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Ground"))
        {
            if (col.contacts[0].normal.y > 0.7) _isGround = true;
            else externalSpeed = 0;
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
