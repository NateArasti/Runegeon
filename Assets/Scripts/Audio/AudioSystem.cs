using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour
{
    private const string k_MusicVolumeKey = "MusicVolume";
    private const string k_SFXVolumeKey = "SFXVolume";

    private static AudioSystem s_Instance;

    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private AudioSource m_MusicSource;
    [SerializeField] private AudioSource m_SFXAudioSource;

    public static bool MusicEnabled { get; private set; } = true;
    public static bool SoundsEnabled { get; private set; } = true;

    private void Awake()
    {
        if(s_Instance != null)
        {
            Destroy(this);
            return;
        }
        s_Instance = this;
    }

    private void Start()
    {
        LoadPlayerPrefs();
        SetMusicState(MusicEnabled);
        SetSFXState(SoundsEnabled);
    }

    private void LoadPlayerPrefs()
    {
        MusicEnabled = PlayerPrefs.GetInt(k_MusicVolumeKey, 1) == 1;
        SoundsEnabled = PlayerPrefs.GetInt(k_SFXVolumeKey, 1) == 1;
    }

    public static void SwitchBackgroundMusic(AudioClip newMusicClip, float newVolume)
    {
        s_Instance.StartCoroutine(SourceClipSwitch(s_Instance.m_MusicSource, newMusicClip, newVolume));
    }

    private static IEnumerator SourceClipSwitch(AudioSource source, AudioClip newClip, float newVolume)
    {
        if (source.isPlaying)
            yield return s_Instance.StartCoroutine(FadeSourceVolume(source, 0));
        else
            source.volume = 0;
        source.clip = newClip;
        if (!source.isPlaying) source.Play();
        yield return s_Instance.StartCoroutine(FadeSourceVolume(source, newVolume));
    }

    private static IEnumerator FadeSourceVolume(AudioSource source, float endValue, float duration = 1)
    {
        var t = 0f;
        var startValue = source.volume;
        while(t < duration)
        {
            yield return null;
            t += Time.deltaTime;

            source.volume = startValue + (endValue - startValue) * t / duration;
        }
        source.volume = endValue;
    }
    
    public static void SetMusicState(bool enabled)
    {
        MusicEnabled = enabled;
        PlayerPrefs.SetInt(k_MusicVolumeKey, MusicEnabled ? 1 : 0);
        PlayerPrefs.Save();
        SetAudioGroupVolume(k_MusicVolumeKey, MusicEnabled ? 0 : -80);
    }

    public static void SetSFXState(bool enabled)
    {
        SoundsEnabled = enabled;
        PlayerPrefs.SetInt(k_MusicVolumeKey, SoundsEnabled ? 1 : 0);
        PlayerPrefs.Save();
        SetAudioGroupVolume(k_SFXVolumeKey, SoundsEnabled ? 0 : -80);
    }

    private static void SetAudioGroupVolume(string key, float volume)
    {
        s_Instance.m_AudioMixer.SetFloat(key, volume);
    }

    public static void PlaySFXSound(AudioClip audioClip, float pitch = 1, float volume = 1)
    {
        var source = Instantiate(s_Instance.m_SFXAudioSource, s_Instance.transform);
        source.pitch = pitch;
        source.volume = volume;
        source.PlayOneShot(audioClip);
        Destroy(source.gameObject, audioClip.length + 0.1f);
    }
}
