using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ThirdPersonModel))]
public class ThirdPersonController : MonoBehaviour
{
    private SpringArmCamera _springArm;
    private ThirdPersonModel _model;
    
    private PlayerInputActions _input;

    private void Awake()
    {
        _springArm = Camera.main.GetComponentInParent<SpringArmCamera>();
        _model = GetComponent<ThirdPersonModel>();

        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.ThirdPersonPlayer.Jump.performed += JumpAction;
        _input.ThirdPersonPlayer.Movement.performed += MovementAction;
        _input.ThirdPersonPlayer.Movement.canceled += MovementCancel;
        _input.ThirdPersonPlayer.Rotation.performed += RotationAction;
        _input.ThirdPersonPlayer.Rotation.canceled += RotationCancel;
        _input.ThirdPersonPlayer.Sprint.performed += SprintAction;
        _input.ThirdPersonPlayer.Sprint.canceled += SprintCancel;
    }

    private void OnDisable()
    {
        _input.Disable();
        
        _input.ThirdPersonPlayer.Jump.performed -= JumpAction;
        _input.ThirdPersonPlayer.Movement.performed -= MovementAction;
        _input.ThirdPersonPlayer.Movement.canceled -= MovementCancel;
        _input.ThirdPersonPlayer.Rotation.performed -= RotationAction;
        _input.ThirdPersonPlayer.Rotation.canceled -= RotationCancel;
        _input.ThirdPersonPlayer.Sprint.performed -= SprintAction;
        _input.ThirdPersonPlayer.Sprint.canceled -= SprintCancel;
    }

    private void JumpAction(InputAction.CallbackContext value)
    {
        _model.Jump();
    }
    
    private void MovementAction(InputAction.CallbackContext value)
    {
        _model.RawInput = value.ReadValue<Vector2>();
    }
    
    private void MovementCancel(InputAction.CallbackContext value)
    {
        _model.RawInput = Vector2.zero;
    }
    
    private void RotationAction(InputAction.CallbackContext value)
    {
        _springArm.MouseInput = value.ReadValue<Vector2>();
    }
    
    private void RotationCancel(InputAction.CallbackContext value)
    {
        _springArm.MouseInput = Vector2.zero;
    }

    private void SprintAction(InputAction.CallbackContext value)
    {
        _model.IsSprinting = true;
    }

    private void SprintCancel(InputAction.CallbackContext value)
    {
        _model.IsSprinting = false;
    }
}
