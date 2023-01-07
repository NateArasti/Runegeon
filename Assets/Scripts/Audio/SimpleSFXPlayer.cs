using NaughtyAttributes;
using UnityEngine;

public class SimpleSFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip m_AudioClip;
    [SerializeField] private bool m_OverrideVolume;
    [SerializeField, ShowIf(nameof(m_OverrideVolume)), Range(0, 1)] private float m_Volume = 1;
    [SerializeField] private bool m_RandomPitch;
    [SerializeField, ShowIf(nameof(m_RandomPitch)), MinMaxSlider(-3, 3)] private Vector2 m_PitchRange = new(1, 1);

    public void PlaySFX()
    {
        AudioSystem.PlaySFXSound(
            m_AudioClip, 
            m_RandomPitch ? Random.Range(m_PitchRange.x, m_PitchRange.y) : 1,
            m_OverrideVolume ? m_Volume : 1
            );
    }
}
