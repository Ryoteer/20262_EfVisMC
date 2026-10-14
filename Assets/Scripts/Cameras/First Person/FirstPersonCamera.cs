using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FirstPersonCamera : MonoBehaviour
{
    [Header("<color=green>Camera</color>")]
    [Range(30.0f, 120.0f)] [SerializeField] private float _fov = 90.0f;
    [Range(-90.0f, 0.0f)] [SerializeField] private float _minRotation = -85.0f;
    [Range(0.0f, 90.0f)] [SerializeField] private float _maxRotation = 85.0f;
    
    [Header("<color=green>Headbob</color>")]
    [Range(0.0f, 0.2f)] [SerializeField] private float _headbobAmount = 0.075f;
    [Range(1.0f, 50.0f)] [SerializeField] private float _headbobSpeed = 12.5f;
    
    [Header("<color=green>Inputs</color>")]
    [Range(0.0f, 1.0f)][SerializeField] private float _smoothFovSpeed = 0.2f;
    
    [Header("<color=green>Overlay Camera</color>")] 
    [SerializeField] private Camera _overlayCamera;
    
    private float _headbobTimer = 0.0f, _mouseY = 0.0f, _baseFov = 90.0f, _desiredFov = 0.0f, _smoothedDesiredFov = 0.0f, _smoothedVelocity = 0.0f;

    private FirstPersonModel _model;
    private Transform _headTransform;

    private void Start()
    {
        _baseFov = _fov;
        
        _model = GameManager.Instance.FirstPersonPlayer;
        _headTransform = _model.Head;

        transform.position = _headTransform.position;
    }

    private void FixedUpdate()
    {
        _desiredFov = Remap(_model.MoveDir.magnitude * _model.SprintModifier, 1.0f, _model.SprintModifier, 0.0f, 1.0f);
        _desiredFov = Mathf.Clamp(_desiredFov, 0.0f, 1.0f);
        _smoothedDesiredFov = Mathf.SmoothDamp(_smoothedDesiredFov, _desiredFov, ref _smoothedVelocity, _smoothFovSpeed);
        _fov = Mathf.Lerp(_baseFov, _baseFov * 1.25f, _smoothedDesiredFov);
        
        Camera.main.fieldOfView = _fov;
        _overlayCamera.fieldOfView = _fov;
        
        if (_model.MoveDir.sqrMagnitude > 0.0f)
        {
            Headbob();
        }
        else
        {
            transform.position = _headTransform.position;
        }
    }

    private void Headbob()
    {
        if (Mathf.Abs(_model.MoveDir.x) > 0.0f || Mathf.Abs(_model.MoveDir.y) > 0.0f)
        {
            _headbobTimer += _headbobSpeed * Time.deltaTime;

            if (!_model.IsOnGround)
            {
                transform.position = _headTransform.position;
            }
            else
            {
                transform.position = new Vector3
                (_headTransform.position.x, 
                    _headTransform.position.y + (Mathf.Sin(_headbobTimer) *  _headbobAmount * _model.MoveDir.magnitude * _model.SprintModifier), 
                    _headTransform.position.z);
            }
        }
    }
    
    private float Remap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return outMin + (value-inMin)*(outMax-outMin)/(inMax-inMin);
    }

    public void Rotate(float x, float y)
    {
        _mouseY += y;
        _mouseY = Mathf.Clamp(_mouseY, _minRotation, _maxRotation);
        
        transform.rotation = Quaternion.Euler(-_mouseY, x, 0.0f);
    }
}
