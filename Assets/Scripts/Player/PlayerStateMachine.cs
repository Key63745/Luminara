using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    CharacterController _characterController;
    Animator _animator;
    [SerializeField] PlayerInput _playerInput;

    bool _isMovePressed;
    bool _isRunPressed;

    float _rotationSpeed = 15.0f;
    float _runSpeed = 4.0f;

    float _gravity = -9.8f;
    float _groundedGravity = -.05f;

    bool _isJumpPressed = false;
    float _maxJumpHeight = 4.0f;
    float _maxJumpTime = .75f;
    bool _isJumping = false;
    bool _requireNewJumpPress = false;

    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Animator Animator { get { return _animator; } }
    public bool IsJumping { set { _isJumping = value; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }

    void Awake()
    {
       _characterController = GetComponent<CharacterController>();

        _states = new PlayerStateFactory(this);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _currentState.Update();
        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    void OnMovement(InputAction.CallbackContext context)
    {

    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }
    void OnRun(InputAction.CallbackContext context)
    {

    }

    void OnInventoryInput(InputAction.CallbackContext context)
    {

    }
}
