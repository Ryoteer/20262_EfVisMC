using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullscreenManager : MonoBehaviour
{
    #region Instance

    public static  FullscreenManager Instance;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    #endregion

    [Header("<color=white>Rendering</color>")]
    [SerializeField] private Material _vignetteMaterial;

    private float t = 0.0f;

    private ThirdPersonModel _model;

    private void Start()
    {
        _model = GameManager.Instance.ThirdPersonPlayer;
        
        _vignetteMaterial.SetFloat("_Amount", 0.0f);
    }

    private void Update()
    {
        if (_model && _vignetteMaterial)
        {
            if (_model.IsSprinting)
            {
                t += Time.deltaTime * 2.5f;
            }
            else
            {
                t -= Time.deltaTime * 1.5f;
            }

            t = Mathf.Clamp01(t);

            if (_model.RawInput.sqrMagnitude != 0.0f)
            {
                _vignetteMaterial.SetFloat("_Amount", t);
            }
            else
            {
                _vignetteMaterial.SetFloat("_Amount", 0.0f);
            }
        }
    }
}
