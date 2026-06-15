using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PiramideResult : MonoBehaviour
{
    public TMP_Text correctText;
    public TMP_Text wrongText;
    public TMP_Text totalText;

    public string sceneToLoad = "Piramide1";

    public AudioClip clickSound;

    [Range(0f, 1f)]
    public float extraDelay = 0.1f;

    AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        int correct = PlayerPrefs.GetInt("PiramideCorrect", 0);
        int wrong = PlayerPrefs.GetInt("PiramideWrong", 0);

        correctText.text = "Certas: " + correct;
        wrongText.text = "Erradas: " + wrong;
        totalText.text = correct + " de 8 perguntas certas";

        PlayerPrefs.SetInt("PiramideCorrect", 0);
        PlayerPrefs.SetInt("PiramideWrong", 0);
    }

    public void Restart()
    {
        StartCoroutine(PlayAndLoad(sceneToLoad));
    }

    IEnumerator PlayAndLoad(string scene)
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            yield return new WaitForSeconds(clickSound.length + extraDelay);
        }

        SceneManager.LoadScene(scene);
    }
}