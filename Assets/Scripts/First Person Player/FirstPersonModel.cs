using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonModel : MonoBehaviour
{
    [Header("<color=green>Camera</color>")] 
    [SerializeField] private Transform _head;
    public Transform Head { get { return _head; } set { _head = value; } }
    [Range(10.0f, 2500.0f)][SerializeField] private float _mouseSensitivity = 500.0f;
    
    [Header("<color=green>Cursor</color>")]
    [SerializeField] private CursorLockMode _lockMode = CursorLockMode.Locked;
    [SerializeField] private bool _isCursorVisible = false;
    
    [Header("<color=green>Inputs</color>")]
    [Range(0.0f, 1.0f)][SerializeField] private float _smoothInputSpeed = 0.2f;
    
    [Header("<color=green>Physics</color>")]
    [Range(0.01f, 0.75f)][SerializeField] private float _groundRayDistance = 0.375f;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _jumpForce = 7.5f;
    [SerializeField] private float _moveSpeed = 3.5f;
    [Range(1.25f, 5.0f)][SerializeField] private float _sprintModifier = 3.5f;

    private bool _isOnGround = false, _isSprinting = false;
    public bool IsOnGround { get { return _isOnGround; } set { _isOnGround = value; } }
    public bool IsSprinting { get { return _isSprinting; } set { _isSprinting = value; } }
    private float _mouseX = 0.0f, _baseSprintModifier = 1.0f, _actualSprintModifier = 1.0f;
    public float SprintModifier
    {
        get { return _actualSprintModifier; }
        set { _actualSprintModifier = value; }
    }
    
    private FirstPersonCamera _camera;
    private FirstPersonView _view;
    private Rigidbody _rb;

    private Vector2 _rawInput = new(), _mouseInput = new(), _smoothedInput = new(), _smoothedVelocity = new();
    public Vector2 RawInput { get { return _rawInput; } set { _rawInput = value; } }
    public Vector2 MouseInput { get { return _mouseInput; } set { _mouseInput = value; } }

    private Vector3 _groundOffsetPos = new(), _moveDir = new();
    public Vector3 MoveDir { get { return _moveDir; } set { _moveDir = value; } }

    private Ray _groundRay;

    private void Awake()
    {
        GameManager.Instance.FirstPersonPlayer = this;

        Cursor.lockState = _lockMode;
        Cursor.visible = _isCursorVisible;
        
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        _camera = Camera.main.GetComponent<FirstPersonCamera>();
        _view = GetComponentInChildren<FirstPersonView>();
    }

    private void Update()
    {
        if (_isSprinting)
        {
            _actualSprintModifier = _sprintModifier;
            _view.SetTimeScale(_sprintModifier);
        }
        else
        {
            _actualSprintModifier = _baseSprintModifier;
            _view.SetTimeScale(_baseSprintModifier);
        }
        
        if (_mouseInput.x != 0.0f || _mouseInput.y != 0.0f)
        {
            Rotate(_mouseInput);
        }

        _smoothedInput = Vector2.SmoothDamp(_smoothedInput, _rawInput, ref _smoothedVelocity, _smoothInputSpeed);
        
        _view.SetMovementValue(_smoothedInput);
        _view.SetGroundBool(_isOnGround);
    }

    private void FixedUpdate()
    {
        _isOnGround = IsGrounded();

        if (_smoothedInput.x != 0.0f || _smoothedInput.y != 0.0f)
        {
            Movement(_smoothedInput);
        }
    }

    private void LateUpdate()
    {
        if (transform.position.y <= -50.0f)
        {
            transform.position = Vector3.zero;
        }
    }

    private bool IsGrounded()
    {
        _groundOffsetPos = new Vector3(transform.position.x, transform.position.y + _groundRayDistance / 4, transform.position.z);
        
        _groundRay = new Ray(_groundOffsetPos, -transform.up);
        
        return Physics.Raycast(_groundRay, _groundRayDistance, _groundLayerMask);
    }

    public void Jump()
    {
        if(!_isOnGround) return;
        
        _view.CallJumpTrigger();
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void Movement(Vector2 input)
    {
        _moveDir = transform.right * input.x + transform.forward * input.y;
        
        _rb.MovePosition(transform.position + _moveDir * _moveSpeed * _actualSprintModifier * Time.deltaTime);
    }

    private void Rotate(Vector2 input)
    {
        _mouseX += input.x * _mouseSensitivity * Time.deltaTime;

        if (_mouseX >= 360.0f || _mouseX <= -360.0f)
        {
            _mouseX -= 360.0f * Mathf.Sign(_mouseX);
        }
        
        input.y *= _mouseSensitivity * Time.deltaTime;
        
        transform.rotation = Quaternion.Euler(0.0f, _mouseX, 0.0f);
        
        _camera.Rotate(_mouseX, input.y);
    }
}
