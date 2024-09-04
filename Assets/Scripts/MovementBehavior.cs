using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehavior : MonoBehaviour
{
    public enum MovementState {Grappling,normalMovement, Ungrappled  };

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _swingSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _acceleration ;
    private float _horizontalInput;
    private float _verticalInput;
    private float _currentSwingForce;
    Vector2 _anchorPoint;

    private Rigidbody2D _rbReference;

    private bool _isGrappling;
    private bool _isJumping;
    private bool _swingMomentumGained = false;

    private MovementState _currentState;

    public MovementState CurrentState
    {
        get { return _currentState; }
    }

    public bool SwingMomentumGained
    {
        get { return _swingMomentumGained; }
    }

    public Vector2 AnchorPoint
    {
        get { return _anchorPoint; }
        set { _anchorPoint = value; }
    }

    public bool IsGrappling
    {
        get { return _isGrappling; }
        set { _isGrappling = value; }
    }

    public bool IsJumping
    {
        get { return _isJumping; }
        set { _isJumping = value; }
    }

    public Rigidbody2D rb2D
    {
        get { return _rbReference; }
        set { _rbReference = value; }
    }
    public float HorizontalInput
    {
        get { return _horizontalInput; }
        set { _horizontalInput = value; }
    }

    public float VerticalInput
    {
        get { return _verticalInput; }
        set { _verticalInput = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        _moveSpeed = 30f;
        _swingSpeed = 500f;
        _jumpForce = 60f;
        _acceleration = 10f;

        _horizontalInput = 0;
        _verticalInput = 0;
        _currentSwingForce = 100f;

        _isJumping = false;
        _isGrappling = false;

        _currentState = MovementState.normalMovement;

    }

    // Update is called once per frame
    void Update()
    {
        if (_rbReference == null)
            return;

     
        UpdateMovementState();

    }
    private void FixedUpdate()
    {
        if (_rbReference == null)
            return;

        switch (_currentState)
        {
            case MovementState.Grappling:
            case MovementState.Ungrappled:
                HandleSwing();
                break;
            case MovementState.normalMovement:
                HandleMovement();
                HandleJump();
                break;
            default:
                HandleMovement();
                break;
        }

    }
    private void HandleMovement()
    {
        float targetSpeed = _horizontalInput * _moveSpeed; 

        float speedDiff = targetSpeed - rb2D.velocity.x;

        //[moveSpeed * acceleration, -moveSpeed * acceleration]
        float movement = speedDiff * _acceleration;
     
        _rbReference.AddForce(movement * Vector2.right);
    }
    private void HandleJump()
    {
        float movement = _verticalInput * _jumpForce;
        if (movement> 0.1f)
        {
            _rbReference.AddForce(movement * Vector2.up, ForceMode2D.Impulse);
        }
        _verticalInput = 0f;
    }
    private void HandleSwing()
    {
        const float forceAmountToAdd = 3.0f;

        float targetSpeed = _horizontalInput * _currentSwingForce;

        if (targetSpeed >= _swingSpeed || targetSpeed <= -_swingSpeed)
        {
            targetSpeed = _horizontalInput * _swingSpeed;
            _swingMomentumGained = true;
        }
        
        if (_currentState == MovementState.Grappling)
        {
            if (_rbReference.position.y >= _anchorPoint.y)
            {          
                return;

            }

            if (!_swingMomentumGained)
            {
                if (_horizontalInput !=0)
                {
                    _currentSwingForce += forceAmountToAdd;
                }
               
            }
  
            rb2D.AddForce(targetSpeed * Vector2.right);
            return;
        }

        if (_currentState == MovementState.Ungrappled)
        {
            _rbReference.AddForce(_rbReference.velocity, ForceMode2D.Impulse);

            _currentState = MovementState.normalMovement;
            _currentSwingForce = 0f;
           
            return;
        }

    }

    private void UpdateMovementState()
    {
        switch (_currentState)
        {
            case MovementState.Grappling:
                if (!_isGrappling)
                {
                    _currentState = MovementState.Ungrappled;
                }
                break;
            case MovementState.normalMovement:
                if (_isGrappling)
                {
                    _swingMomentumGained = false;
                    _currentState = MovementState.Grappling;
                }
                break;  
               
              
        }
    }
 
}
