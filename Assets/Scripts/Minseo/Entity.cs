using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Entity : MonoBehaviour
{
    // for apply appropriate friction to player
    public PhysicsMaterial2D little, zero;
    
    // Entity's info (will be replaced by PlayerInfo Class object later)
    [SerializeField] protected EntityInfo entityInfo;
    
    protected float maxSpeed => entityInfo.maxSpeed;
    protected float accel => entityInfo.accel;
    protected float jumpPower => entityInfo.jumpPower;
    protected Vector2 backStepPower => entityInfo.backStepPower;
    protected float maxHp => entityInfo.maxHp;
    protected float maxMp => entityInfo.maxMp;
    public float atk => entityInfo.atk;
    public float def => entityInfo.def;
    
    // Entity's children gameObjects (player graphic, spawn location, fool position, hand position, etc.)
    [SerializeField] protected Transform footPos;
    [SerializeField] protected Transform graphicTransform;
    [SerializeField] protected Slider stunEffect;
    
    // Entity gameObject's components
    protected Rigidbody2D _rigidbody;
    protected Animator _animator;
    protected CapsuleCollider2D _capsuleCollider;
    protected SpriteRenderer _sprite;

    // for below variable, public will be private ... (for debugging, it is public now)
    // for record current state
    public float _speed;
    public bool _isGround;
    public bool _canJump;
    public bool _inSlope;
    public bool _isAttack;
    public float dashSpeed;
    public float externalSpeed;
    public float stunTimeElapsed;
    public float stunTime;
    public bool _hitAir;

    public float hp;
    public float mp;

    public bool isDie;

    public bool touchMonster;

    public float hitAirTime;
    

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

    public bool CanAttackLogic()
    {
        return !_isAttack && !_hitAir; 
    }

    // Damage calculation formula
    public static float CalculateDamage(float damage, float def)
    {
        return damage * 100 * (1 / (100 + def));
    }

    protected virtual void OnDrawGizmos()
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
        ValueClamp();
        if (stunTimeElapsed > 0)
        {
            stunEffect.gameObject.SetActive(true);
            stunEffect.value = stunTimeElapsed / stunTime;
        }
        else stunEffect.gameObject.SetActive(false);
    }
    
    protected virtual void FixedUpdate()
    {
        if (isDie) return;
        if (stunTimeElapsed > 0 && !isDie) stunTimeElapsed -= Time.fixedDeltaTime;
        ApplyAnimation();
        CheckGround();
        if (stunTimeElapsed <= 0 && !isDie) DetermineNextMove();
        UpdateVelocity();
        if (stunTimeElapsed <= 0 && !isDie) AttackInputDetect();
        UpdateExternalVelocity();
        Move(_groundNormalPerp, _speed);
    }

    protected virtual void ValueClamp()
    {
        return;
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
        hitAirTime -= Time.fixedDeltaTime;
        if (hit)
        {
            _groundNormalPerp = Vector2.Perpendicular(hit.normal);
            if (_isGround)
            {
                if (_hitAir && hitAirTime <= 0) _hitAir = false;
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
            }
            else
            {
                if (hit.collider.GetComponent<PlatformEffector2D>() == null || _rigidbody.velocity.y < 0)
                {
                    if (_hitAir && hitAirTime <= 0) _hitAir = false;
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

    /*
     * This method process the speed about knockback, dash, etc... 
     */
    protected virtual void UpdateExternalVelocity()
    {
        dashSpeed = Mathf.Lerp(dashSpeed, 0, friction * Time.fixedDeltaTime);
        if (!_hitAir || touchMonster) externalSpeed = Mathf.Lerp(externalSpeed, 0, friction * Time.fixedDeltaTime);
    }

    /*
     * player : input detect -> attack!
     * monster : if the player is detected -> attack!
     */
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
                if (Mathf.Abs(_rigidbody.velocity.y) <= Mathf.Abs(-direction.y * (speed + dashSpeed + externalSpeed))) {
                    _velocity.Set(
                        -(speed + dashSpeed + externalSpeed) * direction.x,
                        Mathf.Clamp(-direction.y * (speed + dashSpeed + externalSpeed), -10, Single.PositiveInfinity));
                } else
                {
                    _velocity.Set(
                        -(speed + dashSpeed + externalSpeed) * direction.x,
                        Mathf.Clamp(_rigidbody.velocity.y, -10, Single.PositiveInfinity));
                }
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
        dashSpeed = 0;
        _isGround = false;
        _canJump = false;
        _hitAir = true;
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
        _rigidbody.AddForce(knockback.y * Vector2.up, ForceMode2D.Impulse);
        externalSpeed = knockback.x;
        hitAirTime = 0.1f;
    }

    public virtual void Hit(float damage, Vector2 knockback, float stunTime)
    {
        if (isDie) return;
        _capsuleCollider.sharedMaterial = zero;
        hp -= CalculateDamage(damage, def);
        if (stunTime > stunTimeElapsed)
        {
            stunTimeElapsed = stunTime;
            this.stunTime = stunTime;
        }
        _speed = 0;
        if (hp <= 0)
        {
            Die();
            return;
        }
        KnockBack(knockback);
    }

    public virtual void Die()
    {
        isDie = true;
        Destroy(gameObject);
    }
    
    protected void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Ground") )
        {
            if (col.contacts[0].normal.y > 0.7) CheckGround();
            else
            {
                if (Mathf.Abs(col.contacts[0].normal.x) == 1 && col.collider.GetComponent<PlatformEffector2D>() == null)
                {
                    externalSpeed = 0;
                    dashSpeed = 0;
                }
            }
        } else if (col.gameObject.tag.Equals("Monster"))
        {
            touchMonster = true;
        }
    }
    
    protected virtual void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Ground"))
        {
            if (col.contacts[0].normal.y > 0.7) CheckGround();
            else
            {
                if (Mathf.Abs(col.contacts[0].normal.x) == 1 && col.collider.GetComponent<PlatformEffector2D>() == null)
                {
                    externalSpeed = 0;
                    dashSpeed = 0;
                }
            }
        }
    }

    protected void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) _capsuleCollider.sharedMaterial = zero;
        if (col.gameObject.CompareTag("Monster"))
        {
            touchMonster = false;
        }
    }
}
