using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected Rigidbody2D Rigidbody;
    protected Animator Animator;

    protected virtual void Start()
    {
        Rigidbody = GetComponentInChildren<Rigidbody2D>();
        Animator = GetComponentInChildren<Animator>();
    }

    protected abstract void Move(Vector2 direction, float speed);
    public abstract void Hit(float damage, Vector2 knockback, float stunTime);
    public abstract void Die();
}
