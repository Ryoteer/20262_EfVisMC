using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(ThirdPersonController))]
public class ThirdPersonModel : MonoBehaviour
{
    private ThirdPersonController _controller;
    private ThirdPersonView _view;

    [Header("<color=red>Camera</color>")]
    [SerializeField] private Transform _trackerTransform;
    public Transform TrackerTransform { get { return _trackerTransform; } }

    [Header("<color=red>Inputs</color>")]
    [Range(0.0f, 1.0f)] [SerializeField] private float _smoothInputSpeed = 0.2f;

    [Header("<color=red>Physics</color>")]
    [Range(0.125f, 0.75f)] [SerializeField] private float _groundRayLength = 0.25f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _jumpForce = 8.75f;
    [SerializeField] private float _moveSpeed = 5.0f;
    [Range(1.0f, 5.0f)][SerializeField] private float _sprintModifier = 1.375f;

    private bool _isGrounded = true, _isSprinting = false;
    public bool IsSprinting { get { return _isSprinting; } set { _isSprinting = value; } }
    private float _actualSprintModifier = 1.0f;

    private Ray _groundRay;

    private Vector2 _rawInput = new(), _smoothedInput = new(), _smoothedVelocity = new();
    public Vector2 RawInput { get { return _rawInput; } set { _rawInput = value; } }

    private Vector3 _groundOffsetPos = new(), _moveDir = new(), _dirFix = new(), _cameraForwardFix = new(), _cameraRightFix = new();
    public Vector3 MoveDir { get { return _moveDir; } }
    
    private Rigidbody _rb;
    private SpringArmCamera _springArm;
    private Transform _cameraTransform;

    private void Awake()
    {
        GameManager.Instance.ThirdPersonPlayer = this;
        
        _controller = GetComponent<ThirdPersonController>();
        
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Start()
    {
        _view = GetComponentInChildren<ThirdPersonView>();

        _springArm = Camera.main.GetComponentInParent<SpringArmCamera>();
        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        switch (_isSprinting)
        {
            case true:
                _actualSprintModifier = _sprintModifier;
                _view.SetTimeScale(_actualSprintModifier);
                break;
            case false:
                _actualSprintModifier = 1.0f;
                _view.SetTimeScale(_actualSprintModifier);
                break;
        }

        _smoothedInput = Vector2.SmoothDamp(_smoothedInput, _rawInput, ref _smoothedVelocity, _smoothInputSpeed);
        
        _view.SetMoveValue(_smoothedInput);
        _view.SetGroundBool(_isGrounded);
    }

    private void FixedUpdate()
    {
        _isGrounded = IsGrounded();

        if (_rawInput.x != 0.0f ||  _rawInput.y != 0.0f)
        {
            Movement(_smoothedInput);
        }
    }

    private bool IsGrounded()
    {
        _groundOffsetPos = new Vector3(
            transform.position.x,
            transform.position.y + (_groundRayLength / 4),
            transform.position.z);
        
        _groundRay = new Ray(_groundOffsetPos, -transform.up);

        return Physics.Raycast(_groundRay, _groundRayLength, _groundLayer);
    }

    public void Jump()
    {
        if (!_isGrounded) return;
        
        _view.SetJumpTrigger();
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }

    private void Movement(Vector2 input)
    {
        _cameraForwardFix = _cameraTransform.forward;
        _cameraRightFix = _cameraTransform.right;

        _cameraForwardFix.y = 0.0f;
        _cameraRightFix.y = 0.0f;
        
        Rotate(_cameraForwardFix);
        
        _cameraForwardFix.Normalize();
        _cameraRightFix.Normalize();
        
        _dirFix = _cameraRightFix * input.x + _cameraForwardFix * input.y;
        
        _rb.MovePosition(_rb.position + _dirFix * (_moveSpeed * _actualSprintModifier) * Time.fixedDeltaTime);
    }

    private void Rotate(Vector3 dir)
    {
        transform.forward = dir;
    }
}
