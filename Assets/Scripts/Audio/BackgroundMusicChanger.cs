using UnityEngine;

public class BackgroundMusicChanger : MonoBehaviour
{
    [SerializeField] private AudioClip m_AudioClip;
    [SerializeField] private float m_Volume = 1;

    private void Start()
    {
        AudioSystem.SwitchBackgroundMusic(m_AudioClip, m_Volume);
    }
}
