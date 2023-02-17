using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerBehavior : Entity
{
    // Player gameObject's components
    [SerializeField] private Transform wallCheckPos;
    private PlayerInputHandler _playerInputHandler;
    private PlayerAttack _playerAttack;
    
    // Entity's info (will be replaced by PlayerInfo Class object later)
    public PlayerInfo EntityInfo => (PlayerInfo) entityInfo;
    public PlayerDataManager PlayerDataManager
    {
        get => GameManager.Instance.PlayerDataManager;
        set => PlayerDataManager = value;
    }
    
    // for below variable, public will be private ... (for debuging, it is public now)
    // for record current state
    public bool isClimb;
    
    // tmp (will be removed maybe...)
    public float dashElapsed;
    public float wallJump;
    public float wallJumpPower;
    public float wallTime;
    public float hitTimeElapsed;
    public bool isHitMotion;

    public float invincibilityTimeElapsed;

    public bool canWall;
    
    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    public float leftDis;

    public new float hp { get => GameManager.Instance.PlayerDataManager.hp; set => GameManager.Instance.PlayerDataManager.hp = value; }
    public new float mp { get => GameManager.Instance.PlayerDataManager.mp; set => GameManager.Instance.PlayerDataManager.mp = value; }

    // input detect variable
    public bool _normalAttackDetect;

    public bool canControl
    {
        get => GameManager.Instance.PlayerDataManager.canControl;
        set => GameManager.Instance.PlayerDataManager.canControl = value;
    }

    // For debugging in editor (not play mode)

    public static bool CanAttackCondition(int index)
    {
        return GameManager.Instance.PlayerDataManager.mp >= GameManager.Instance.PlayerDataManager.playerInfo.attackMp[index];
    }
    
    public static bool CanUtilCondition(int index)
    {
        return GameManager.Instance.PlayerDataManager.mp >= GameManager.Instance.PlayerDataManager.playerInfo.utilMp[index];
    }
    
    public static bool CanUtilCondition(int index, float rate)
    {
        return GameManager.Instance.PlayerDataManager.mp >= GameManager.Instance.PlayerDataManager.playerInfo.utilMp[index] * rate;
    }

    public static void UseAttackSkill(int index)
    {
        GameManager.Instance.PlayerDataManager.mp -= GameManager.Instance.PlayerDataManager.playerInfo.attackMp[index];
    }
    
    public static void UseUtilSkill(int index)
    {
        GameManager.Instance.PlayerDataManager.mp -= GameManager.Instance.PlayerDataManager.playerInfo.utilMp[index];
    }
    public static void UseUtilSkill(int index, float rate)
    {
        GameManager.Instance.PlayerDataManager.mp -= GameManager.Instance.PlayerDataManager.playerInfo.utilMp[index] * rate;
    }

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
        base.Update();
        AttackCheck();
        _animator.SetFloat("attackSpeed", PlayerDataManager.attackSpeed);
    }

    // Physics logic...
    protected override void FixedUpdate()
    {
        hitTimeElapsed -= Time.fixedDeltaTime;
        invincibilityTimeElapsed -= Time.fixedDeltaTime;
        if (stunTimeElapsed > 0) stunTimeElapsed -= Time.fixedDeltaTime;
        ApplyAnimation();
        CheckGround();
        CheckWall();
        UpdateVelocity();
        AttackInputDetect();
        UpdateExternalVelocity();
        Move(_groundNormalPerp, _speed);
        if (_isGround && hitTimeElapsed < 0 && isHitMotion) _animator.SetTrigger("notHit");
    }

    private void AttackCheck()
    {
        _isAttack = _animator.GetCurrentAnimatorStateInfo(0).IsTag("attack");
        isHitMotion = _animator.GetCurrentAnimatorStateInfo(0).IsTag("hit");
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
        if (isDie || isHitMotion) return;
        wallTime -= Time.fixedDeltaTime;

        var hits = Physics2D.RaycastAll(
            wallCheckPos.position,
            Vector2.left * graphicTransform.localScale.x,
            leftDis
        );
        
        Debug.DrawRay(wallCheckPos.position,
            Vector2.left * graphicTransform.localScale.x * leftDis* graphicTransform.localScale.x,
            Color.black
            );

        foreach (var hit in hits)
        {
            if (hit)
            {
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    if (stunTimeElapsed <= 0 && !_isGround &&
                        !_isAttack &&
                        hit.collider.gameObject.GetComponent<PlatformEffector2D>() == null &&
                        wallJump <= 0 &&
                        CanUtilCondition(1, Time.deltaTime) && canWall && !_hitAir && Mathf.Abs(externalSpeed) < 0.1f)
                    {
                        UseUtilSkill(1, Time.deltaTime);
                        if ((int)_playerInputHandler.movement.x == -(int)graphicTransform.localScale.x)
                        {
                            _canJump = true;
                            isClimb = true;
                            _speed = 0;
                            wallTime = 0.2f;
                        } else if (wallTime > 0)
                        {
                            _canJump = true;
                            isClimb = true;
                            _speed = 0;
                        }
                    }
                    if (!CanUtilCondition(1, Time.deltaTime)) canWall = false;
                    else if (isClimb && wallTime < 0)
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
        if (touchMonster) _capsuleCollider.sharedMaterial = zero;
        // if condition (!_inSlope) is omitted... player slide down a slope...
        if (!_inSlope || (_inSlope && Mathf.Abs(dashSpeed) > 1))
        {
            if (wallJump<0) _speed = _rigidbody.velocity.x;
        }

        if (_playerInputHandler.movement.x != 0 && !_isAttack && !_hitAir && !isClimb && stunTimeElapsed <= 0 && canControl)
        {
            _speed += accel * _playerInputHandler.movement.x;
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
    
    protected override void AttackInputDetect()
    {
        if (PlayerDataManager.attackSpeed == 0 || isHitMotion || isDie)
        {
            _normalAttackDetect = false;
            return;
        }
        // Normal Attack
        if (_normalAttackDetect && stunTimeElapsed <= 0)
        {
            if (!touchMonster) _capsuleCollider.sharedMaterial = little;
            _normalAttackDetect = false;
            
            // player stop moving (_speed = 0), and dash while attacking (in _playerAttack.NormalAttack);
            _speed = 0;
            if (_playerInputHandler.movement.y > 0)
            {
                _playerAttack._normalAttackNumber = 1;
                _animator.SetInteger("normalAttack", 1);
            }

            if (_playerInputHandler.dashCheck < 2)
            {
                _playerAttack.NormalAttack();
            }
            else
            {
                _playerAttack.DashAttack();
                _playerInputHandler.dashCheck = 0;
            }
        }
        
        _normalAttackDetect = false;
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
    public override void Hit(float damage, Vector2 knockback, float stunTime)
    {
        if (isDie) return;
        invincibilityTimeElapsed = EntityInfo.invincibilityTime[3];
        _capsuleCollider.sharedMaterial = zero;
        GameManager.Instance.EffectManager.CreateEffect(1, transform.position,
            Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.forward));
        hp -= CalculateDamage(damage, PlayerDataManager.def);
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
        hitTimeElapsed = 0.2f;
        _animator.SetTrigger("hit");
        _playerAttack.ResetNormalAttack();
        KnockBack(knockback);
    }

    public void Hit(float damage, Vector2 knockback, float stunTime, Vector3 opponent)
    {
        if (isDie || invincibilityTimeElapsed > 0) return;
        invincibilityTimeElapsed = EntityInfo.invincibilityTime[3];
        _capsuleCollider.sharedMaterial = zero;
        GameManager.Instance.EffectManager.CreateEffect(1, transform.position,
            Quaternion.AngleAxis(
                Mathf.Atan2((opponent-transform.position).y, (opponent-transform.position).x) * Mathf.Rad2Deg, 
                Vector3.forward));
        hp -= CalculateDamage(damage, PlayerDataManager.def);
        if (stunTime * (1 - GameManager.Instance.PlayerDataManager.stance) > stunTimeElapsed)
        {
            stunTimeElapsed = stunTime * (1 - GameManager.Instance.PlayerDataManager.stance);
            this.stunTime = stunTime * (1 - GameManager.Instance.PlayerDataManager.stance);
        }
        _speed = 0;
        if (hp <= 0)
        {
            Die();
            return;
        }
        hitTimeElapsed = 0.2f;
        if (GetKnockback(knockback, entityInfo.stance).magnitude > 3 || stunTime > 0.5f)
        {
            _animator.SetTrigger("hit");
            _playerAttack.ResetNormalAttack();
        }

        if (isClimb) isClimb = false;

        KnockBack(GetKnockback(knockback, entityInfo.stance));
    }

    public override void Die()
    {
        isDie = true;
        PlayerDataManager.isDie = true;
    }

    public void Attack()
    {
        _normalAttackDetect = true;
    }

    public override void Jump()
    {
        if (stunTimeElapsed > 0) return;
        if (!isClimb)
        {
            if (isHitMotion && CanUtilCondition(3) && stunTimeElapsed <= 0)
            {
                invincibilityTimeElapsed = EntityInfo.invincibilityTime[2];
                externalSpeed = 0;
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
                _rigidbody.AddForce(backStepPower.y * Vector2.up, ForceMode2D.Impulse);
                _hitAir = false;
                _animator.SetTrigger("notHit");
                UseUtilSkill(3);
                StartCoroutine(MotionCancel());
                GameManager.Instance.EffectManager.CreateEffect(0, transform.position, Quaternion.identity);
                _playerAttack.ResetNormalAttack();
            }
            else
            {
                base.Jump();
            }
        }
        else if (CanUtilCondition(4))
        {
            UseUtilSkill(4);
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
        if (isHitMotion && CanUtilCondition(3) && stunTimeElapsed <= 0)
        {
            invincibilityTimeElapsed = EntityInfo.invincibilityTime[2];
            externalSpeed = 0;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _rigidbody.AddForce(backStepPower.y * Vector2.up, ForceMode2D.Impulse);
            _hitAir = false;
            _animator.SetTrigger("notHit");
            UseUtilSkill(3);
            StartCoroutine(MotionCancel());
            GameManager.Instance.EffectManager.CreateEffect(0, transform.position, Quaternion.identity);
            _playerAttack.ResetNormalAttack();
        }
        else if (_canJump && Mathf.Abs(externalSpeed) < 1 && stunTimeElapsed <= 0 && CanUtilCondition(0))
        {
            UseUtilSkill(0);
            invincibilityTimeElapsed = EntityInfo.invincibilityTime[0];
            if (_isAttack && CanUtilCondition(3))
            {
                GameManager.Instance.EffectManager.CreateEffect(0, transform.position, Quaternion.identity);
                UseUtilSkill(3);
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
    
    public void Dash()
    {
        if (_canJump && Mathf.Abs(externalSpeed) < 1 && stunTimeElapsed <= 0 && CanUtilCondition(2))
        {
            UseUtilSkill(2);
            _playerAttack.ResetNormalAttack();
            
            if (_isAttack && CanUtilCondition(3))
            {
                GameManager.Instance.EffectManager.CreateEffect(0, transform.position, Quaternion.identity);
                UseUtilSkill(3);
                _animator.SetTrigger("attackCancel");
                StartCoroutine(MotionCancel());
            }

            if (_playerInputHandler.movement.x != 0)
            {
                _graphicLocalScale.Set(-_playerInputHandler.movement.x, 1, 1);
                graphicTransform.localScale = _graphicLocalScale;
            }

            dashElapsed = 1;
            gameObject.layer = 9;
            _speed = 0;
            externalSpeed = 0;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _isGround = false;
            _canJump = false;
            dashSpeed = -backStepPower.x * _playerAttack.transform.localScale.x;
        }
    }

    public void Interaction()
    {
        if (!canControl) return;
        var tmp = Physics2D.OverlapBoxAll(transform.position, Vector2.one * 2, 0);
        foreach (var i in tmp)
        {
            if (i.CompareTag("NPC"))
            {
                var npc = i.GetComponent<NPC>();
                GameManager.Instance.UIManager.ConservationUI.SetCurrentConservationArray(
                    npc.conversationClips, 
                    npc.GetConversationStart());
            }
        }
    }

    IEnumerator MotionCancel()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.01f);

        Time.timeScale = 1;
    }
    
    protected override void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag.Equals("Ground"))
        {
            if (col.contacts[0].normal.y > 0.7)
            {
                canWall = true;
                CheckGround();
            }
            else
            {
                if (Mathf.Abs(col.contacts[0].normal.x) == 1 && col.collider.GetComponent<PlatformEffector2D>() == null && wallJump < 0)
                {
                    externalSpeed = 0;
                    dashSpeed = 0;
                }
            }
        }
    }
}
