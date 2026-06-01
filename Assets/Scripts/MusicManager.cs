using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // Garante apenas um MusicManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void MusicOn()
    {
        audioSource.mute = false;
        PlayerPrefs.SetInt("MusicEnabled", 1);
    }

    public void MusicOff()
    {
        audioSource.mute = true;
        PlayerPrefs.SetInt("MusicEnabled", 0);
    }

    private void Start()
    {
        bool musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        audioSource.mute = !musicEnabled;
    }
}