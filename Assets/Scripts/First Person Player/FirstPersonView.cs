using UnityEngine;

public class FirstPersonView : MonoBehaviour
{
    private Animator _bodyAnimator, _handAnimator;

    [Header("<color=green>Animator</color>")] 
    [SerializeField] private string _groundBoolName = "isGrounded";
    [SerializeField] private string _jumpTriggerName = "onJump";
    [SerializeField] private string _moveFloatName = "moveMagnitude";
    [SerializeField] private string _xFloatName = "xAxis";
    [SerializeField] private string _yFloatName = "yAxis";

    private void Awake()
    {
        _bodyAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        _handAnimator = Camera.main.GetComponentInChildren<Animator>();
    }

    public void SetGroundBool(bool state)
    {
        _bodyAnimator.SetBool(_groundBoolName, state);
        _handAnimator.SetBool(_groundBoolName, state);
    }

    public void CallJumpTrigger()
    {
        _bodyAnimator.SetTrigger(_jumpTriggerName);
        _handAnimator.SetTrigger(_jumpTriggerName);
    }

    public void SetMovementValue(Vector2 input)
    {
        _bodyAnimator.SetFloat(_xFloatName, input.x);
        _bodyAnimator.SetFloat(_yFloatName, input.y);
        _handAnimator.SetFloat(_moveFloatName, input.magnitude);
    }

    public void SetTimeScale(float timeScale)
    {
        if (timeScale > 1.0f)
        {
            timeScale /= 3.0f;
        }
        
        _bodyAnimator.speed = timeScale;
        _handAnimator.speed = timeScale;
    }
}
