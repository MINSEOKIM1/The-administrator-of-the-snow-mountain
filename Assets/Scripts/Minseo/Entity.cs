using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    // for apply appropriate friction to player
    [SerializeField] private PhysicsMaterial2D little, zero;
    
    // Player's info (will be replaced by PlayerInfo Class object later)
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accel;
    [SerializeField] private float jumpPower;
    [SerializeField] private float stateChangeInterval;
    
    // Player's children gameObjects (player graphic, spawn location, fool position, hand position, etc.)
    [SerializeField] private Transform footPos;
    [SerializeField] private Transform playerGraphicTransform;
    
    // Player gameObject's components
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private PlayerInputHandler _playerInputHandler;
    private CapsuleCollider2D _capsuleCollider;
    private SpriteRenderer _sprite;
    private PlayerAttack _playerAttack;
    
    // for below variable, public will be private ... (for debugging, it is public now)
    // for record current state
    public float _speed;
    public bool _isGround;
    protected bool _canJump;
    public bool _inSlope;
    protected bool _isAttack;
    protected bool _canAttack;
    public float playerDashSpeed;
    public float externalSpeed;

    // 0 : idle, -1 : to left, 1 : to right - attack is independent...
    public int moveState;
    public float stateChangeElapsed;

    // tmp (will be removed maybe...)
    public float down;
    public float footRadius;
    public Vector2 knockback;
    public float friction;
    public LayerMask ground;
    
    // tmp variable (for avoiding creating a new object to set value like _rigid.velocity, _graphic.localScale)
    private Vector3 _graphicLocalScale;
    private Vector2 _velocity;
    private Vector2 _groundNormalPerp;

    public GameObject _target;
    
    
}
