using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    public AudioClip clickSound;

    [Range(0f, 1f)]
    public float extraDelay = 0.1f;

    AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void PlaySound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void Pause()
    {
        PlaySound();
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        PlaySound();
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        StartCoroutine(RestartCoroutine());
    }

    public void Menu()
    {
        StartCoroutine(MenuCoroutine());
    }

    IEnumerator RestartCoroutine()
    {
        PlaySound();
        yield return new WaitForSecondsRealtime(clickSound != null ? clickSound.length + extraDelay : 0f);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator MenuCoroutine()
    {
        PlaySound();
        yield return new WaitForSecondsRealtime(clickSound != null ? clickSound.length + extraDelay : 0f);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuColetar");
    }
}