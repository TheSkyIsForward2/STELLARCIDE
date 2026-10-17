using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin mcp;
    public float duration = 1;
    public float strength = 1.4f;

    void Awake()
    {
        mcp = GetComponent<CinemachineBasicMultiChannelPerlin>();
        mcp.AmplitudeGain = 0;
    }

    void OnEnable()
    {
        EventBus.Instance.OnPlayerHit += ShakeScreen;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        EventBus.Instance.OnPlayerHit -= ShakeScreen;
    }

    void ShakeScreen()
    {
        StartCoroutine(_ShakeScreen());
    }

    IEnumerator _ShakeScreen()
    {
        mcp.AmplitudeGain = strength;
        yield return new WaitForSeconds(duration);
        mcp.AmplitudeGain = 0;
    }
}
