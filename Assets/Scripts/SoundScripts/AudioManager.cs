using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private bool isMuted = false;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("SFX")]
    public SFXManager sfxManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (sfxManager == null)
            sfxManager = GetComponentInChildren<SFXManager>();

        DontDestroyOnLoad(gameObject);
    }

    public void SetMusicVolume(float volume)
    {
        SetVolume("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetVolume("SFXVolume", volume);
    }

    public void SetEnemySFXVolume(float volume)
    {
        SetVolume("EnemySFXVolume", volume);
    }

    private void SetVolume(string parameterName, float volume)
    {
        float volumeDB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;

        audioMixer.SetFloat(parameterName, volumeDB);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            audioMixer.SetFloat("MasterVolume", -80f);
        }
        else
        {
            audioMixer.SetFloat("MasterVolume", 0f);
        }
    }
}