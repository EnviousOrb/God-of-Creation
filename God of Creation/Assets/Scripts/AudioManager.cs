using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<AudioClip> musicClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PlayMusic(musicClips[0]);
        }
        else
            Destroy(gameObject);
    }

    public void PlayMusic(AudioClip music)
    {
        if (music)
        {
            musicSource.clip = music;
            musicSource.Play();
        }
    }

    public void StopMusic()
    { 
        musicSource.Stop();
    }

    public void PlaySFX(AudioClip sound)
    {
        sfxSource.PlayOneShot(sound);
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }
};
