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

    private void Awake()
    {
        if(s_Instance != null)
        {
            Destroy(this);
            return;
        }
        s_Instance = this;
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
    
    public static void SetMusicVolume(float volume)
    {
        SetAudioGroupVolume(k_MusicVolumeKey, volume);
    }

    public static void SetSFXVolume(float volume)
    {
        SetAudioGroupVolume(k_SFXVolumeKey, volume);
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
