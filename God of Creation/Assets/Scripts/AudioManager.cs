using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    private Slider musicSlider;
    private Slider sfxSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

        }
        else
            Destroy(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        musicSlider = FindObjectByName<Slider>("MusicSlider");
        if (musicSlider != null)
        {
            musicSlider.value = GetMusicVolume();
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        sfxSlider = FindObjectByName<Slider>("SFXSlider");
        if (sfxSlider != null)
        {
            sfxSlider.value = GetSFXVolume();
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }
    }

    private T FindObjectByName<T>(string name) where T : Component
    {
        return Resources.FindObjectsOfTypeAll<T>().FirstOrDefault(obj => obj.name == name);
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
