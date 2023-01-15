using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public bool isAttack;
    public bool canAttack;

    private Animator _animator;
    
    private int _normalAttackNumber;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _normalAttackNumber = 0;
        canAttack = true;
    }

    public void NormalAttack()
    {
        if (canAttack)
        {
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
