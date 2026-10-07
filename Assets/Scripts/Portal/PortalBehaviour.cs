using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PortalBehaviour : MonoBehaviour
{
    #region Input System
    private InputSystem_Actions _inputs;

    private void Awake()
    {
        _inputs = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputs.Enable();

        _inputs.Player.Interact.performed += Interact;
    }

    private void OnDisable()
    {
        _inputs.Disable();

        _inputs.Player.Interact.performed -= Interact;
    }
    #endregion

    [Header("<color=cyan>Gameplay</color>")]
    [Range(0.0f, 10.0f)][SerializeField] private float _stateInterval = 2.0f;

    [Header("<color=cyan>Rendering</color>")]
    [SerializeField] private string _alphaFloatName = "_Alpha";
    [SerializeField] private string _destinationTexName = "_DestinationTex";
    [SerializeField] private Texture[] _destinationTexs;
    [SerializeField] private string _baseColorName = "_BaseColor";
    [SerializeField] private Color[] _baseColors;
    [SerializeField] private string _borderColorName = "_BorderColor";
    [SerializeField] private Color[] _borderColors;
    [SerializeField] private string _tilingVectorName = "_Tiling";
    [SerializeField] private Vector2[] _tilings;
    [SerializeField] private string _strengthFloatName = "_Strength";
    [SerializeField] private float[] _strengths;

    private Material _material;
    private Renderer _renderer;

    private bool _isActive = false;
    private int _index = 0;

    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _material = _renderer.material;

        _destinationTexs[0] = _material.GetTexture(_destinationTexName);
        _baseColors[0] = _material.GetColor(_baseColorName);
        _borderColors[0] = _material.GetColor(_borderColorName);
        _tilings[0] = _material.GetVector(_tilingVectorName);
        _strengths[0] = _material.GetFloat(_strengthFloatName);
    }

    private void Interact(InputAction.CallbackContext value)
    {
        if((_destinationTexs.Length == _baseColors.Length && _destinationTexs.Length == _borderColors.Length) && !_isActive)
        {
            StartCoroutine(ChangeDestination());
        }
    }

    private IEnumerator ChangeDestination()
    {
        _isActive = true;

        float t = 0.0f;

        while(t < 1.0f)
        {
            t += Time.deltaTime / _stateInterval;

            _material.SetFloat(_alphaFloatName, Mathf.Lerp(1.0f, 0.0f, t));

            yield return null;
        }

        if(_index >= _destinationTexs.Length - 1)
        {
            _index = 0;
        }
        else
        {
            _index++;
        }

        _material.SetTexture(_destinationTexName, _destinationTexs[_index]);
        _material.SetColor(_baseColorName, _baseColors[_index]);
        _material.SetColor(_borderColorName, _borderColors[_index]);
        _material.SetVector(_tilingVectorName, _tilings[_index]);
        _material.SetFloat(_strengthFloatName, _strengths[_index]);

        t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime / _stateInterval;

            _material.SetFloat(_alphaFloatName, Mathf.Lerp(0.0f, 1.0f, t));

            yield return null;
        }

        _isActive = false;
    }
}
