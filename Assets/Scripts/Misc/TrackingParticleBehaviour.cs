using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class TrackingParticleBehaviour : MonoBehaviour
{
    [Header("<color=orange>VFX</color>")]
    [SerializeField] private string _playEventName = "OnFogPlay";
    [SerializeField] private string _stopEventName = "OnFogStop";

    private ThirdPersonModel _player;
    private VisualEffect _vfx;

    private void Awake()
    {
        _vfx = GetComponent<VisualEffect>();
    }

    private void Start()
    {
        _player = GameManager.Instance.ThirdPersonPlayer;

        _vfx.SendEvent(_playEventName);
    }

    private void LateUpdate()
    {
        transform.position = _player.transform.position;
    }
}
