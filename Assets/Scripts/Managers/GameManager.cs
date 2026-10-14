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

    private FirstPersonModel _firstPersonPlayer;
    public FirstPersonModel FirstPersonPlayer
    {
        get { return _firstPersonPlayer; }
        set { _firstPersonPlayer = value; }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
}
