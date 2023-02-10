using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 movement;
    private PlayerBehavior _playerBehavior;
    
    // tmp
    public float dashInputCheck;
    public float dashTime;
    public int dashCheck;
    public bool dashDirection;

    private void Start()
    {
        _playerBehavior = GetComponent<PlayerBehavior>();
        dashCheck = 0;
    }

    private void Update()
    {
        dashInputCheck -= Time.deltaTime;

        if (dashInputCheck < 0) dashCheck = 0;
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        if (_playerBehavior.canControl)
        {
            movement = value.ReadValue<Vector2>();

            if (value.started && movement.x != 0)
            {
                dashInputCheck = dashTime;
                if ((dashDirection && movement.x > 0) || (!dashDirection && movement.x < 0) || dashCheck == 0)
                {
                    dashCheck++;
                }

                dashDirection = movement.x > 0;
                if (dashCheck == 2) _playerBehavior.Dash();
            }
        }
        else
        {
            Debug.Log("ASGDDSASGDS");
            movement = Vector2.zero;
        }
    }
    
    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started && _playerBehavior.canControl)
        {
            _playerBehavior.Jump();
        }
    }
    
    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.started && _playerBehavior.canControl)
        {
            _playerBehavior.Attack();
        }
    }

    public void OnBackstep(InputAction.CallbackContext value)
    {
        if (value.started && _playerBehavior.canControl)
        {
            _playerBehavior.Backstep();
        }
    }

    public void OnInteraction(InputAction.CallbackContext value)
    {
        if (value.started && _playerBehavior.canControl)
        {
            _playerBehavior.Interaction();
        }
    }
}
