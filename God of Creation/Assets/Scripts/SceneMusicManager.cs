using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMusicManager : MonoBehaviour
{
    public static SceneMusicManager Instance;
    [Header("Music Clips")]
    [SerializeField] private AudioClip[] sceneMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip music = GetMusicClipForScene(sceneName);
        if (music)
            AudioManager.Instance.PlayMusic(music);
    }

    private AudioClip GetMusicClipForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "DevScene":
                return sceneMusic[0];
            case "BattleScene":
                return sceneMusic[1];
                case "ShopScene":
                return sceneMusic[2];
            default:
                return null;
        }
    }
}
