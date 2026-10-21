using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArmCamera : MonoBehaviour
{
    [Header("<color=red>Camera</color>")]
    [Range(10.0f, 5000.0f)] [SerializeField] private float _mouseSensitivity = 100.0f;
    [Range(0.125f, 1.0f)] [SerializeField] private float _minDistance = 0.25f;
    [Range(1.0f, 8.0f)] [SerializeField] private float _maxDistance = 4.0f;
    [Range(-90.0f, 0.0f)] [SerializeField] private float _minRotation = -85.0f;
    [Range(0.0f, 90.0f)] [SerializeField] private float _maxRotation = 85.0f;
    
    [Header("<color=red>Cursor</color>")]
    [SerializeField] private CursorLockMode _lockMode = CursorLockMode.Locked;
    [SerializeField] private bool _isVisible = false;

    [Header("<color=red>Inputs</color>")]
    [Range(0.0f, 1.0f)] [SerializeField] private float _smoothInputSpeed = 0.2f;

    [Header("<color=red>Physics</color>")]
    [Range(0.05f, 0.125f)][SerializeField] private float _castRadius = 0.075f;
    [Range(0.1f, 0.5f)][SerializeField] private float _hitOffset = 0.25f;

    private bool _isBlocked = false;
    private float _mouseX = 0.0f, _mouseY = 0.0f;

    private Ray _cameraRay;
    private RaycastHit _cameraRayHit;

    private Vector2 _mouseInput = new(), _smoothedInput = new(), _smoothedVelocity = new();
    public Vector2 MouseInput { get { return _mouseInput; }  set { _mouseInput = value; } }

    private Vector3 _dir = new(), _dirTest = new(), _cameraPos = new();

    private Camera _camera;
    private Transform _target;

    private void Start()
    {
        _target = GameManager.Instance.ThirdPersonPlayer.TrackerTransform;
        _camera = Camera.main;

        Cursor.lockState = _lockMode;
        Cursor.visible = _isVisible;

        _mouseX = transform.eulerAngles.y;
        _mouseY = transform.eulerAngles.x;
    }

    private void Update()
    {
        _smoothedInput = Vector2.SmoothDamp(_smoothedInput, _mouseInput, ref _smoothedVelocity, _smoothInputSpeed);

    }

    private void FixedUpdate()
    {
        _cameraRay = new Ray(transform.position, _dir);
        
        _isBlocked = Physics.SphereCast(_cameraRay, _castRadius, out _cameraRayHit, _maxDistance);
    }

    private void LateUpdate()
    {
        UpdateRotation(_smoothedInput);
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _dir = -transform.forward;

        if (_isBlocked)
        {
            _dirTest = (_cameraRayHit.point - transform.position) + (_cameraRayHit.normal * _hitOffset);

            if (_dirTest.sqrMagnitude <= _minDistance * _minDistance)
            {
                _cameraPos = transform.position + _dir * _minDistance;
            }
            else
            {
                _cameraPos = transform.position + _dirTest;
            }
        }
        else
        {
            _cameraPos = transform.position + _dir * _maxDistance;
        }
        
        _camera.transform.position = _cameraPos;
        _camera.transform.LookAt(transform.position);
    }

    private void UpdateRotation(Vector2 input)
    {
        transform.position = _target.position;
        
        if(input.x == 0.0 && input.y == 0.0) return;

        if (input.x != 0.0f)
        {
            _mouseX += input.x * _mouseSensitivity * Time.deltaTime;

            if (_mouseX > 360.0f || _mouseX < -360.0f)
            {
                _mouseX -= 360.0f * Mathf.Sign(_mouseX);
            }
        }

        if (input.y != 0.0f)
        {
            _mouseY += input.y * _mouseSensitivity * Time.deltaTime;

            _mouseY = Mathf.Clamp(_mouseY, _minRotation, _maxRotation);
        }
        
        transform.rotation = Quaternion.Euler(-_mouseY, _mouseX, 0.0f);
    }
}
