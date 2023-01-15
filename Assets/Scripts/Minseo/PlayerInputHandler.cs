using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 movement;
    private PlayerBehavior _playerBehavior;

    private void Start()
    {
        _playerBehavior = GetComponent<PlayerBehavior>();
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        movement = value.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            _playerBehavior.Jump();
        }
    }
}
