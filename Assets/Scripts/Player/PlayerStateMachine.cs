using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    CharacterController _characterController;
    Animator _animator;
    PlayerInput _playerInput;
    PlayerInputActions _playerInputActions;
    Camera _camera;
    [SerializeField] CinemachineVirtualCamera _cinCam;
    [SerializeField] InputActionReference _lookReference;
    [SerializeField] GameObject _interactHint;
    
    [SerializeField] UltimateRadialMenu _inventory;
    Vector2 _inventoryInput;
    float _inventoryDistance;

    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;

    float _rotationSpeed = 15.0f;
    float _walkSpeed = 5f;
    float _runSpeed = 8f;

    float _gravity = -9.8f;
    float _groundedGravity = -10f;

    bool _isJumpPressed = false;
    float _initialJumpVelocity = 5f;
    float _maxJumpHeight = 3.0f;
    float _maxJumpTime = .75f;
    bool _isJumping = false;
    bool _requireNewJumpPress = false;

    bool _menuEnabled = false;

    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public Animator Animator { get { return _animator; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public bool IsJumping { set { _isJumping = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
    public float GroundedGravity { get { return _groundedGravity; } }
    public float JumpGravity { get { return _gravity; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    public float WalkSpeed { get { return _walkSpeed; } }
    public float RunSpeed { get { return _runSpeed; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInputActions = new PlayerInputActions();
       _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _camera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _playerInputActions.PlayerControls.Movement.started += OnMovement;
        _playerInputActions.PlayerControls.Movement.canceled += OnMovement;
        _playerInputActions.PlayerControls.Movement.performed += OnMovement;
        _playerInputActions.PlayerControls.Run.started += OnRun;
        _playerInputActions.PlayerControls.Run.canceled += OnRun;
        _playerInputActions.PlayerControls.Jump.started += OnJump;
        _playerInputActions.PlayerControls.Jump.canceled += OnJump;
        _playerInputActions.PlayerControls.Inventory.started += OnInventoryInput;
        _playerInputActions.PlayerControls.Inventory.canceled += OnInventoryInput;
    }

    // Start is called before the first frame update
    void Start()
    {
        _inventory.RemoveAllRadialButtons();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        _currentState.UpdateStates();

        Vector3 moveDirection = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * _appliedMovement;
        _characterController.Move(moveDirection * Time.deltaTime);
        _animator.SetFloat("X", _appliedMovement.x);
        _animator.SetFloat("Y", _appliedMovement.z);

        //Raycast for finding Interactables
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 2f))
        {
            if (raycastHit.collider.tag == "Interactable")
            {
                _interactHint.SetActive(true);
                if (_playerInputActions.PlayerControls.Interact.triggered)
                {
                    raycastHit.collider.GetComponent<Interactable>().interact.Invoke();
                }
            }
            else
            {
                _interactHint.SetActive(false);
            }
        } else
        {
            _interactHint.SetActive(false);
        }
    }

    void HandleRotation()
    {
        transform.rotation = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0);
    }

    void OnMovement(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }
    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _menuEnabled = !_menuEnabled;
            _playerInput.currentActionMap = _menuEnabled ? _playerInput.actions.FindActionMap("InventoryControls") : _playerInput.actions.FindActionMap("PlayerControls");
            Cursor.lockState = _menuEnabled ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = _menuEnabled;
            if (_menuEnabled)
            {
                _inventory.EnableRadialMenu();
                _cinCam.GetComponent<CinemachineInputProvider>().XYAxis = null;
            }
            else
            {
                _inventory.DisableRadialMenu();
                _cinCam.GetComponent<CinemachineInputProvider>().XYAxis = _lookReference;
            }
        }
    }

    void OnEnable()
    {
        _playerInputActions.PlayerControls.Enable();
    }

    void OnDisable()
    {
        _playerInputActions.PlayerControls.Disable();
    }
}
