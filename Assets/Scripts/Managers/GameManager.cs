using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Instance

    public static GameManager Instance;

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
    [Range(30, 165)][SerializeField] private int _maxFPS = 60;
    
    private ThirdPersonModel _thirdPersonPlayer;

    public ThirdPersonModel ThirdPersonPlayer
    {
        get { return _thirdPersonPlayer; }
        set { _thirdPersonPlayer = value; }
    }

    private void Start()
    {
        Application.targetFrameRate = _maxFPS;
    }    
}
