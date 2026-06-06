using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonSound : MonoBehaviour
{
    public AudioClip clickSound;
    public string sceneToLoad;

    AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (sceneToLoad != "")
        {
            GetComponent<Button>().onClick.AddListener(PlaySoundAndLoad);
        }
        else
        {
            GetComponent<Button>().onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    void PlaySoundAndLoad()
    {
        if (clickSound != null)
        {
            StartCoroutine(WaitAndLoad());
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    IEnumerator WaitAndLoad()
    {
        audioSource.PlayOneShot(clickSound);
        DontDestroyOnLoad(gameObject);
        yield return new WaitForSeconds(clickSound.length);
        SceneManager.LoadScene(sceneToLoad);
        Destroy(gameObject);
    }
}