using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : PlayerStates
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;

    private float _horizontalMovement;
    private float _movement;

    public override void ExecuteState()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (Mathf.Abs(_horizontalMovement) > 0.1f) // Check if we are actually moving the player
        {
            _movement = _horizontalMovement;
        }
        else
        {
            _movement = 0f;
        }

        float moveSpeed = _movement * speed;
        _playerController.SetHorizontalForce(moveSpeed);
    }

    protected override void GetInput()
    {
        _horizontalMovement = _horizontalInput;
    }
}
