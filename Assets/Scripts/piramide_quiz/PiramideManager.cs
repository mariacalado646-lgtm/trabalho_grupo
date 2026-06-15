using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PiramideManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text option1Text;
    public TMP_Text option2Text;
    public TMP_Text option3Text;

    public TMP_Text feedbackText;

    [Header("Pergunta")]
    public string question;
    public string option1;
    public string option2;
    public string option3;
    public int correctOption;

    [Header("Navegação")]
    public string nextScene;

    [Header("Som")]
    public AudioClip correctSound;
    public AudioClip wrongSound;

    [Range(0f, 2f)]
    public float delayToNext = 1.5f;

    AudioSource audioSource;
    bool answered = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        questionText.text = question;
        option1Text.text = option1;
        option2Text.text = option2;
        option3Text.text = option3;

        feedbackText.text = "";

        if (!PlayerPrefs.HasKey("PiramideCorrect"))
            PlayerPrefs.SetInt("PiramideCorrect", 0);

        if (!PlayerPrefs.HasKey("PiramideWrong"))
            PlayerPrefs.SetInt("PiramideWrong", 0);
    }

    public void Answer(int option)
    {
        if (answered) return;
        answered = true;

        if (option == correctOption)
        {
            PlayerPrefs.SetInt("PiramideCorrect",
                PlayerPrefs.GetInt("PiramideCorrect", 0) + 1);

            feedbackText.text = "Correto!";
            feedbackText.color = Color.green;

            if (correctSound != null)
                audioSource.PlayOneShot(correctSound);
        }
        else
        {
            PlayerPrefs.SetInt("PiramideWrong",
                PlayerPrefs.GetInt("PiramideWrong", 0) + 1);

            feedbackText.text = "Errado!";
            feedbackText.color = Color.red;

            if (wrongSound != null)
                audioSource.PlayOneShot(wrongSound);
        }

        StartCoroutine(GoToNext());
    }

    IEnumerator GoToNext()
    {
        yield return new WaitForSeconds(delayToNext);
        SceneManager.LoadScene(nextScene);
    }
}