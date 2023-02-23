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
    public bool isRolling;

    public int dashTrailCount;
    public float dashTrailInterval;

    public float invincibilityTimeElapsed;

    public bool canWall;
    
    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    public float leftDis;

    public new float hp { get => GameManager.Instance.PlayerDataManager.hp; set => GameManager.Instance.PlayerDataManager.hp = value; }
    public new float mp { get => GameManager.Instance.PlayerDataManager.mp; set => GameManager.Instance.PlayerDataManager.mp = value; }

    // input detect variable
    public bool _normalAttackDetect;

    private int tutorialSignal;

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
        GameManager.Instance.UIManager.MapUI.UpdateMapPoint();
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
        if (GameManager.Instance.PlayerDataManager.tutorial % 2 == 0)
        {
            tutorialSignal = 0;
            if (GameManager.Instance.PlayerDataManager.tutorial >= 5 &&
                GameManager.Instance.PlayerDataManager.tutorial <= 13)
            {
                GameManager.Instance.PlayerDataManager.tutorial++;
            } else if (GameManager.Instance.PlayerDataManager.tutorial == 14)
            {
                if (TutorialManager.Instance.tutorialNpc.conversationStart == 15)
                    TutorialManager.Instance.tutorialNpc.conversationStart++;
            }
        }
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
        isRolling = _animator.GetCurrentAnimatorStateInfo(0).IsTag("roll");
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
        if (!_inSlope || (_inSlope && Mathf.Abs(dashSpeed) > 1) || isRolling)
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
        if (PlayerDataManager.attackSpeed == 0 || isHitMotion || isDie || isRolling)
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

            if (_playerInputHandler.dashCheck < 2)
            {
                _playerAttack.NormalAttack();
            }
            else
            {
                if (GameManager.Instance.PlayerDataManager.tutorial == 11)
                {
                    tutorialSignal++;
                    if (_playerAttack.canAttack && CanAttackCondition(3))
                    {
                        if (tutorialSignal == 1)
                        {
                            GameManager.Instance.PlayerDataManager.tutorial++;
                            GameManager.Instance.UIManager.PopMessage("대쉬 어택 "+tutorialSignal + "/1", 3);
                        }
                    }
                }
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
        _animator.SetTrigger("hit");
        KnockBack(GetKnockback(new Vector2(5 * graphicTransform.localScale.x, 5), 0));
        isDie = true;
        PlayerDataManager.isDie = true;
    }

    public void Attack()
    {
        _normalAttackDetect = true;
    }

    public override void Jump()
    {
        if (stunTimeElapsed > 0 || isRolling) return;
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
            if (_isAttack)
            {
                if (CanUtilCondition(3))
                {
                    GameManager.Instance.EffectManager.CreateEffect(0, transform.position, Quaternion.identity);
                    UseUtilSkill(3);
                    _animator.SetTrigger("attackCancel");
                    StartCoroutine(MotionCancel());
                }
                else
                {
                    return;
                }
            }
            if (GameManager.Instance.PlayerDataManager.tutorial == 7)
            {
                tutorialSignal++;
                GameManager.Instance.UIManager.PopMessage("백스텝 "+tutorialSignal + "/2", 3);
                if (tutorialSignal == 2)
                {
                    GameManager.Instance.PlayerDataManager.tutorial++;
                    
                }
            }
            UseUtilSkill(0);
            invincibilityTimeElapsed = EntityInfo.invincibilityTime[0];
            
            GameManager.Instance.AudioManager.PlaySfx(5);
            GameManager.Instance.EffectManager.CreateEffect(2,  (int) -graphicTransform.localScale.x, footPos.position, Quaternion.identity);
            
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
    
    public void Roll()
    {
        if (_isGround && Mathf.Abs(externalSpeed) < 1 && stunTimeElapsed <= 0 && CanUtilCondition(0) && !isRolling)
        {
            if (_isAttack )
            {
                if (CanUtilCondition(3)){
                    GameManager.Instance.EffectManager.CreateEffect(0, transform.position, Quaternion.identity);
                    UseUtilSkill(3);
                    _animator.SetTrigger("attackCancel");
                    StartCoroutine(MotionCancel());
                }
                else
                {
                    return;
                }
            }
            if (GameManager.Instance.PlayerDataManager.tutorial == 5)
            {
                tutorialSignal++;
                GameManager.Instance.UIManager.PopMessage("구르기 "+tutorialSignal + "/2", 3);
                if (tutorialSignal == 2)
                {
                    GameManager.Instance.PlayerDataManager.tutorial++;
                }
            }
            invincibilityTimeElapsed = EntityInfo.invincibilityTime[0];
            
            
            UseUtilSkill(0);
            _playerAttack.ResetNormalAttack();
            GameManager.Instance.EffectManager.CreateEffect(2,  (int) graphicTransform.localScale.x, footPos.position, Quaternion.identity);
            
            GameManager.Instance.AudioManager.PlaySfx(8);
            dashElapsed = 1;
            gameObject.layer = 9;
            _speed = 0;
            externalSpeed = 0;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _isGround = false;
            _canJump = false;
            dashSpeed = entityInfo.rollPower * _playerInputHandler.movement.x * 1.4f;
            if (_playerInputHandler.movement.x < 0)
            {
                graphicTransform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                graphicTransform.localScale = new Vector3(-1, 1, 1);
            }
            _animator.SetTrigger("roll");
        }
    }
    
    public void Dash()
    {
        if (_canJump && Mathf.Abs(externalSpeed) < 1 && stunTimeElapsed <= 0 && CanUtilCondition(2))
        {
            if (_isAttack)
            {
                if (CanUtilCondition(3)) {
                    GameManager.Instance.EffectManager.CreateEffect(0, transform.position, Quaternion.identity);
                    UseUtilSkill(3);
                    _animator.SetTrigger("attackCancel");
                    StartCoroutine(MotionCancel());
                }
                else
                {
                    return;
                }
            }
            if (GameManager.Instance.PlayerDataManager.tutorial == 9)
            {
                tutorialSignal++;
                GameManager.Instance.UIManager.PopMessage("대쉬 "+tutorialSignal + "/2", 3);
                if (tutorialSignal == 2)
                {
                    GameManager.Instance.PlayerDataManager.tutorial++;
                }
            }
            UseUtilSkill(2);
            _playerAttack.ResetNormalAttack();
            
            

            if (_playerInputHandler.movement.x != 0)
            {
                _graphicLocalScale.Set(-_playerInputHandler.movement.x, 1, 1);
                graphicTransform.localScale = _graphicLocalScale;
            }

            StartCoroutine(DashTrail());
            
            GameManager.Instance.AudioManager.PlaySfx(8);
            GameManager.Instance.EffectManager.CreateEffect(2,  (int) graphicTransform.localScale.x, footPos.position, Quaternion.identity);
            dashElapsed = 1;
            gameObject.layer = 9;
            _speed = 0;
            externalSpeed = 0;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _isGround = false;
            _canJump = false;
            dashSpeed = -entityInfo.dashPower * _playerAttack.transform.localScale.x;
        }
    }

    public void Interaction()
    {
        if (!canControl || GameManager.Instance.UIManager.MapUI.gameObject.activeSelf) return;
        var tmp = Physics2D.OverlapBoxAll(transform.position, Vector2.one * 2, 0);
        foreach (var i in tmp)
        {
            if (i.CompareTag("NPC"))
            {
                var npc = i.GetComponent<NPC>();
                GameManager.Instance.UIManager.ConservationUI.SetCurrentConservationArray(
                    npc.conversationClips, 
                    npc.GetConversationStart(),
                    npc);
                try
                {
                    var an = (AgentNPC)npc;
                    GameManager.Instance.UIManager.MapUI.currentAllocatedAgent = an.agentData;
                }
                catch (Exception e)
                {
                    
                }
            }
        }
    }

    IEnumerator MotionCancel()
    {
        if (GameManager.Instance.PlayerDataManager.tutorial == 13)
        {
            tutorialSignal++;
            GameManager.Instance.UIManager.PopMessage("모션캔슬 "+tutorialSignal + "/3", 3);
            if (tutorialSignal == 3)
            {
                GameManager.Instance.PlayerDataManager.tutorial++;
            }
        }
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.01f);

        Time.timeScale = 1;
    }

    IEnumerator DashTrail()
    {
        for (int i = 0; i < dashTrailCount; i++)
        {
            yield return new WaitForSeconds(dashTrailInterval);
            SpawnTrail();
        }
    }
    private void SpawnTrail()
    {
        GameObject trailPart = new GameObject();
        SpriteRenderer tpr = trailPart.AddComponent<SpriteRenderer>();
        if (_sprite.sprite != null)
        {
            tpr.transform.localScale = graphicTransform.localScale;
            tpr.sprite = GetComponentInChildren<SpriteRenderer>().sprite;
        }

        tpr.sortingLayerName = "Player";
        tpr.flipX = _sprite.flipX;
        trailPart.transform.position = transform.position;
        Destroy(trailPart, 0.5f);
        
        StartCoroutine("FadeTrailPart", tpr);
    }
    IEnumerator FadeTrailPart(SpriteRenderer trailPartRenderer)
    {
        Color color = trailPartRenderer.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime*3; 
            trailPartRenderer.color = color;
            yield return new WaitForFixedUpdate();
        }
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
