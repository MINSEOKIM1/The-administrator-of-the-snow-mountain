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
                    _playerBehavior.atk * atkCoefficient
                    , k, 
                    atkStunTime);
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

            boxSize = _playerBehavior.EntityInfo.attackBoundary[_normalAttackNumber];
            boxOffset = _playerBehavior.EntityInfo.attackOffset[_normalAttackNumber];
            atkCoefficient = _playerBehavior.EntityInfo.attackCoefficient[_normalAttackNumber];
            atkKnockback = _playerBehavior.EntityInfo.attackKnockback[_normalAttackNumber];
            atkStunTime = _playerBehavior.EntityInfo.attackStunTime[_normalAttackNumber];

            // tmp : go to forward while attack!!
            if (!_playerBehavior._hitAir) _playerBehavior.dashSpeed = -go * transform.localScale.x;
            canAttack = false;
            _animator.SetTrigger("attack");
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
}
