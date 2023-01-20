using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerBehavior : Entity
{
    // Player gameObject's components
    [SerializeField] private Transform wallCheckPos;
    private PlayerInputHandler _playerInputHandler;
    private PlayerAttack _playerAttack;
    
    // for below variable, public will be private ... (for debuging, it is public now)
    // for record current state
    public bool isClimb;
    

    // tmp (will be removed maybe...)
    public float dashElapsed;
    public float wallJump;
    public float wallJumpPower;
    
    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    public float leftDis;

    // input detect variable
    public bool _normalAttackDetect;

    // For debugging in editor (not play mode)

    protected override void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _playerInputHandler = GetComponent<PlayerInputHandler>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _playerAttack = GetComponentInChildren<PlayerAttack>();
    }

    protected override void Update()
    {
        AttackCheck();
    }

    // Physics logic...
    protected override void FixedUpdate()
    {
        ApplyAnimation();
        CheckGround();
        CheckWall();
        UpdateVelocity();
        AttackInputDetect();
        UpdateExternalVelocity();
        Move(_groundNormalPerp, _speed);
    }

    private void AttackCheck()
    {
        _isAttack = _animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");
    }

    protected override void ApplyAnimation()
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
    // CheckGround() is inherited... and no override
    
    
    /*
     * Check if...
     * 1. isClimb?
     */
    private void CheckWall()
    {
        var hits = Physics2D.RaycastAll(
            wallCheckPos.position,
            Vector2.left * playerGraphicTransform.localScale.x,
            leftDis
        );
        
        Debug.DrawRay(wallCheckPos.position,
            Vector2.left * playerGraphicTransform.localScale.x * leftDis* playerGraphicTransform.localScale.x,
            Color.black
            );

        foreach (var hit in hits)
        {
            if (hit)
            {
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    if (!isClimb &&
                        (int)_playerInputHandler.movement.x == -(int)playerGraphicTransform.localScale.x &&
                        !_isGround &&
                        !_isAttack &&
                        hit.collider.gameObject.GetComponent<PlatformEffector2D>() == null &&
                        wallJump <= 0 &&
                        _rigidbody.velocity.y < 0)
                    {
                        _canJump = true;
                        isClimb = true;
                        _speed = 0;
                    }
                    else
                    {
                        isClimb = false;
                    }

                    break;
                }
                else
                {
                    isClimb = false;
                }
            }
        }
        
        _animator.SetBool("climb", isClimb);
    }

    /*
     * according to Input...
     * update _speed
     */
    protected override void UpdateVelocity()
    {
        // if condition (!_inSlope) is omitted... player slide down a slope...
        if (!_inSlope || (_inSlope && Mathf.Abs(dashSpeed) > 1))
        {
            if (wallJump<0) _speed = _rigidbody.velocity.x;
        }

        if (_playerInputHandler.movement.x != 0 && !_isAttack && !_hitAir)
        {
            _speed += accel * _playerInputHandler.movement.x;
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
    
    protected override void AttackInputDetect()
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

    protected override void UpdateExternalVelocity()
    {
        dashSpeed = Mathf.Lerp(dashSpeed, 0, friction * Time.fixedDeltaTime);

        dashElapsed -= Time.fixedDeltaTime;
        if ((Mathf.Abs(dashSpeed) < 1 || dashElapsed <= 0) && gameObject.layer == 9) gameObject.layer = 7;
        
        if (_isGround) externalSpeed = Mathf.Lerp(externalSpeed, 0, friction * Time.fixedDeltaTime);
    }
    /*
     * according to _speed, playerDashSpeed, externalSpeed, ground checking condition...
     * update rigid.velocity -> apply change on real player's movement;
     */
    protected override void Move(Vector2 direction, float speed)
    {
        wallJump -= Time.fixedDeltaTime;
        base.Move(direction, speed);
        if (isClimb && wallJump <= 0) _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
    }

    public void Attack()
    {
        _normalAttackDetect = true;
    }

    public override void Jump()
    {
        if (!isClimb)
        {
            base.Jump();
        }
        else
        {
            wallJump = 0.3f;
            _speed = maxSpeed * (_graphicLocalScale.x);
            dashSpeed = wallJumpPower * (_graphicLocalScale.x);
            base.Jump();
        }
    }

    public void KnockBack()
    {
        _speed = 0;
        _isGround = false;
        _canJump = false;
        _rigidbody.AddForce(knockback.y * Vector2.up, ForceMode2D.Impulse);
        externalSpeed = knockback.x * _playerAttack.transform.localScale.x;
    }

    public void Backstep()
    {
        if (_canJump && Mathf.Abs(externalSpeed) < 1)
        {
            if (_isAttack)
            {
                _animator.SetTrigger("attackCancel");
                StartCoroutine(MotionCancel());
            }
            
            _playerAttack.ResetNormalAttack();

            dashElapsed = 1;
            gameObject.layer = 9;
            _speed = 0;
            externalSpeed = 0;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _isGround = false;
            _canJump = false;
            _rigidbody.AddForce(backStepPower.y * Vector2.up, ForceMode2D.Impulse);
            dashSpeed = backStepPower.x * _playerAttack.transform.localScale.x;
        }
    }

    IEnumerator MotionCancel()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.2f);

        Time.timeScale = 1;
    }
    
    protected override void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Ground"))
        {
            if (col.contacts[0].normal.y > 0.7) CheckGround();
            else
            {
                if (Mathf.Abs(col.contacts[0].normal.x) == 1 && wallJump < 0)
                {
                    externalSpeed = 0;
                    dashSpeed = 0;
                }
            }
        }
    }
}
