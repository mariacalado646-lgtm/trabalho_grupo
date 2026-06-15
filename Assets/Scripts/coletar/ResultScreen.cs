using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ResultScreen : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text scoreText;
    public TMP_Text targetText;

    public GameObject nextButton;
    public GameObject restartButton;

    public int totalLevels = 3;

    public AudioClip clickSound;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;

    [Range(0f, 1f)]
    public float extraDelay = 0.1f;

    AudioSource audioSource;
    AudioSource musicSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = false;

        int score = PlayerPrefs.GetInt("LastScore", 0);
        int target = PlayerPrefs.GetInt("TargetScore", 0);
        int won = PlayerPrefs.GetInt("Won", 0);
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        scoreText.text = "Pontuação: " + score;
        targetText.text = "Meta: " + target;

        if (won == 1)
        {
            if (currentLevel >= totalLevels)
            {
                resultText.text = "JOGO CONCLUÍDO";
                nextButton.SetActive(false);
                restartButton.SetActive(true);
            }
            else
            {
                resultText.text = "VITÓRIA";
                nextButton.SetActive(true);
                restartButton.SetActive(false);
            }

            PlayMusic(victoryMusic);
        }
        else
        {
            resultText.text = "DERROTA";
            nextButton.SetActive(false);
            restartButton.SetActive(true);

            PlayMusic(defeatMusic);
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void NextLevel()
    {
        StartCoroutine(PlayAndLoad("IntroLevel" + (PlayerPrefs.GetInt("CurrentLevel", 1) + 1)));
    }

    public void Restart()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int won = PlayerPrefs.GetInt("Won", 0);

        if (currentLevel >= totalLevels && won == 1)
        {
            // Ganhou o último level — reinicia tudo do zero
            StartCoroutine(PlayAndLoadRestart(true));
        }
        else
        {
            // Perdeu ou não é o último level — reinicia o level atual
            StartCoroutine(PlayAndLoadRestart(false));
        }
    }

    public void Menu()
    {
        StartCoroutine(PlayAndLoad("MenuColetar"));
    }

    IEnumerator PlayAndLoad(string scene)
    {
        musicSource.Stop();

        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            yield return new WaitForSeconds(clickSound.length + extraDelay);
        }

        SceneManager.LoadScene(scene);
    }

    IEnumerator PlayAndLoadRestart(bool resetAll)
    {
        musicSource.Stop();

        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            yield return new WaitForSeconds(clickSound.length + extraDelay);
        }

        if (resetAll)
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
            PlayerPrefs.SetInt("CurrentLevel", 1);
            SceneManager.LoadScene("IntroLevel1");
        }
        else
        {
            string scene = PlayerPrefs.GetString("CurrentScene", "Level1");
            SceneManager.LoadScene(scene);
        }
    }
}