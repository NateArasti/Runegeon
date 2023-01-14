using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShakeEffect : MonoBehaviour
{
    [SerializeField] private float m_Intensity = 2f;
    [SerializeField] private float m_ShakeTime = 1f;

    private CinemachineBasicMultiChannelPerlin m_CameraPerlin;
    private float m_CurrentShakeTime;

    private void Awake()
    {
        m_CameraPerlin = GetComponent<CinemachineVirtualCamera>()
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    [Button]
    public void Play()
    {
        m_CameraPerlin.m_AmplitudeGain = m_Intensity;
        m_CurrentShakeTime = m_ShakeTime;
    }

    private void Update()
    {
        if(m_CurrentShakeTime > 0)
        {
            m_CurrentShakeTime -= Time.deltaTime;
            if(m_CurrentShakeTime <= 0)
            {
                m_CameraPerlin.m_AmplitudeGain = 0;
            }
        }
    }
}
