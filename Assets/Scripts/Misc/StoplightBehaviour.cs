using UnityEngine;

[RequireComponent(typeof(Animator))]
public class StoplightBehaviour : MonoBehaviour
{
    private Animator _animator;
    
    private bool _state = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        ChangeState();
    }

    private void ChangeState()
    {
        if (_state)
        {
            _animator.SetTrigger("onStop");
            
            Invoke("ChangeState", 15.0f);
        }
        else
        {
            _animator.SetTrigger("onGo");
            
            Invoke("ChangeState", 10.0f);
        }
        
        _state = !_state;
    }
}
