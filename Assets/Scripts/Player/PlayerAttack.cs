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
    
    private int _normalAttackNumber;
    
    // tmp variable
    private Vector3 _graphicLocalScale;
    private Vector2 _boxOffsetWithLocalscale;
    public Vector2 boxSize;
    public Vector2 boxOffset;
    
    // tmp
    public float go;
    
    private void OnDrawGizmos()
    {
        _boxOffsetWithLocalscale.Set(boxOffset.x * transform.localScale.x, boxOffset.y);
        Gizmos.color = Color.blue;
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
                i.GetComponent<Monster>().Hit(10, ((i.transform.position - transform.position)*2 + Vector3.up*4), 1);
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
            // tmp : go to forward while attack!!
            _playerBehavior.dashSpeed = -go * transform.localScale.x;
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
        _animator.SetInteger("normalAttack", _normalAttackNumber);
    }
}
