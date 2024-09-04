using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCharacter : BasicCharacter
{
    const string MOVEMENT_HORIZONTAL = "MovementHorizontal";
    const string JUMP = "Jump";
    const string PLATFORM = "Platform";
    const string HAZARD = "Hazard";
    private bool _isJumping = false;

    private float _coyoteTime = 0.1f;
    private float _coyoteTimeCounter = 0f;

    private float _jumpBufferTime = 0.2f;
    private float _jumpBufferCounter = 0f;

    private int _damageAmount = 1;

    [SerializeField] private LevelEnd[] _levelEnds;

    private void Start()
    {
        _movementBehavior.rb2D = _rigidBody2D;
        
    }

    private void Update()
    {
        //Check wheter the game has ended
        foreach (LevelEnd item in _levelEnds)
        {
            if (item != null)
            {
                if (item.IsEnded)
                {
                    _movementBehavior.HorizontalInput = 0;
                    _movementBehavior.VerticalInput = 0;
                    return;
                }
            }
        }

        UpdateCoyoteTime();
        UpdateJumpBuffer();
        HandleMovementInput();


       
       
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == PLATFORM)
            _isJumping = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == PLATFORM)
            _isJumping = true;

    }

    void HandleMovementInput()
    {
        _movementBehavior.IsJumping = _isJumping;
        if (_movementBehavior == null)
            return;

        _movementBehavior.HorizontalInput = Input.GetAxisRaw(MOVEMENT_HORIZONTAL);

        if (_jumpBufferCounter >0f && _coyoteTimeCounter > 0f)
        {
            _movementBehavior.VerticalInput = 1f;
            _jumpBufferCounter = 0f;
        }
        if (_rigidBody2D.velocity.y >0f)
        {
            _coyoteTimeCounter = 0;
        }


    }

    private bool IsGrounded()
    {
        return !_isJumping;
    }

    private void UpdateCoyoteTime()
    {
        if (IsGrounded())
        {
            _coyoteTimeCounter = _coyoteTime;
            return;
        }
        _coyoteTimeCounter -= Time.deltaTime;

    }

    private void UpdateJumpBuffer()
    {
        if(Input.GetButtonDown(JUMP))
        {
            _jumpBufferCounter = _jumpBufferTime;
            return;
        }
        _jumpBufferCounter -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == HAZARD)
        {
            _playerHealth.TakeDamage(_damageAmount);
            if (!_playerHealth.IsValid)
            {
                SceneManager.LoadScene(0);
            }

        }
    }
}
