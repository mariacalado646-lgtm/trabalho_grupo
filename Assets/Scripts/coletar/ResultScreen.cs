using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultScreen : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text scoreText;
    public TMP_Text targetText;

    public GameObject nextButton;
    public GameObject restartButton;

    public int totalLevels = 3;

    void Start()
    {
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
        }
        else
        {
            resultText.text = "DERROTA";
            nextButton.SetActive(false);
            restartButton.SetActive(true);
        }
    }

    public void NextLevel()
    {
        int current = PlayerPrefs.GetInt("CurrentLevel", 1);
        SceneManager.LoadScene("IntroLevel" + (current + 1));
    }

    public void Restart()
    {
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);

        if (currentLevel >= totalLevels)
        {
            // Reinicia tudo do zero
            PlayerPrefs.SetInt("UnlockedLevel", 1);
            PlayerPrefs.SetInt("CurrentLevel", 1);
            SceneManager.LoadScene("IntroLevel1");
        }
        else
        {
            // Reinicia o level atual
            string scene = PlayerPrefs.GetString("CurrentScene", "Level1");
            SceneManager.LoadScene(scene);
        }
    }

    public void Menu()
    {
        SceneManager.LoadScene("MenuColetar");
    }
}