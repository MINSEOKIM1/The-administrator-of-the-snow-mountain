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
    private Rigidbody2D _rigidbody2D;
    
    private int _normalAttackNumber;
    
    //tmp
    public float go;

    private void Start()
    {
        _playerBehavior = GetComponentInParent<PlayerBehavior>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponentInParent<Rigidbody2D>();
        
        _normalAttackNumber = 0;
        canAttack = true;
    }

    public void NormalAttack()
    {
        if (canAttack)
        {
            // tmp : go to forward while attack!!
            _playerBehavior.playerDashSpeed = -go * transform.localScale.x;
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
