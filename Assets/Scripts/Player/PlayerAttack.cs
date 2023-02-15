using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public bool isAttack;
    public bool canAttack;

    private Animator _animator;
    private PlayerBehavior _playerBehavior;
    private PlayerInputHandler _playerInputHandler;
    private Rigidbody2D _rigidbody2D;
    
    public int _normalAttackNumber;
    
    // tmp variable
    private Vector3 _graphicLocalScale;
    private Vector2 _boxOffsetWithLocalscale;
    public Vector2 boxSize;
    public Vector2 boxOffset;
    public Vector2 atkKnockback;
    public float atkCoefficient;
    public float atkStunTime;
    
    // tmp
    public float go;
    
    private void OnDrawGizmos()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((Vector2)transform.parent.position + _boxOffsetWithLocalscale, boxSize);
    }
    
    private void Start()
    {
        _playerBehavior = GetComponentInParent<PlayerBehavior>();
        _playerInputHandler = GetComponentInParent<PlayerInputHandler>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponentInParent<Rigidbody2D>();
        
        _normalAttackNumber = 0;
        canAttack = true;
    }

    public void SetHitbox(int index)
    {
        boxSize = _playerBehavior.EntityInfo.attackBoundary[index];
        boxOffset = _playerBehavior.EntityInfo.attackOffset[index];
        atkCoefficient = _playerBehavior.EntityInfo.attackCoefficient[index];
        atkKnockback = _playerBehavior.EntityInfo.attackKnockback[index];
        atkStunTime = _playerBehavior.EntityInfo.attackStunTime[index];
    }

    public void HitCheck()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + _boxOffsetWithLocalscale, boxSize, 0);

        foreach (var i in collider2Ds)
        {
            if (i.CompareTag("Monster"))
            {
                var k = atkKnockback;
                k.Set(i.transform.position.x < transform.parent.position.x ? -k.x : k.x, k.y);
                i.GetComponent<Monster>().Hit(
                    _playerBehavior.PlayerDataManager.atk * atkCoefficient
                    , k, 
                    atkStunTime,
                    transform.parent.position);
            }
        }
    }

    public void NormalAttack()
    {
        if (canAttack)
        {
            // change attack direction according to arrow key
            if (_playerInputHandler.movement.x != 0)
            {
                _graphicLocalScale.Set(-_playerInputHandler.movement.x, 1, 1);
                transform.localScale = _graphicLocalScale;
            }

            SetHitbox(_normalAttackNumber);

            // tmp : go to forward while attack!!
            if (!_playerBehavior._hitAir) _playerBehavior.dashSpeed = -go * transform.localScale.x;
            canAttack = false;
            _animator.SetTrigger("attack");
        }
    }

    public void DashAttack()
    {
        if (canAttack && PlayerBehavior.CanAttackCondition(3))
        {
            _playerBehavior.invincibilityTimeElapsed = _playerBehavior.EntityInfo.invincibilityTime[1];
            // change attack direction according to arrow key
            if (_playerInputHandler.movement.x != 0)
            {
                _graphicLocalScale.Set(-_playerInputHandler.movement.x, 1, 1);
                transform.localScale = _graphicLocalScale;
            }

            PlayerBehavior.UseAttackSkill(3);
            SetHitbox(3);

            _normalAttackNumber = 3;
            _animator.SetInteger("normalAttack", _normalAttackNumber);

            // tmp : go to forward while attack!!
            _rigidbody2D.MovePosition(transform.position -  (Vector3) new Vector2(transform.localScale.x, 0) * 3);
            if (!_playerBehavior._hitAir) _playerBehavior.dashSpeed = -transform.localScale.x ;
            canAttack = false;
            _animator.SetTrigger("attack");
            GameManager.Instance.EffectManager.CreateEffect(0, transform.parent.position, Quaternion.identity);
        }
    }

    public void CanAttack()
    {
        canAttack = true;
    }

    public void NormalAttackIncrement()
    {
        _normalAttackNumber++;
        _animator.SetInteger("normalAttack", _normalAttackNumber);
    }

    public void ResetNormalAttack()
    {
        CanAttack();
        _normalAttackNumber = 0;
        _animator.ResetTrigger("attack");
        _animator.SetInteger("normalAttack", _normalAttackNumber);
    }

    public void PlaySFX(int index)
    {
        GameManager.Instance.AudioManager.PlaySfx(index);
    }
}
