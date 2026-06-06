using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score;

    public int targetScore = 10;

    public float timeLeft = 30;

    public int currentLevel = 1;

    public TMP_Text scoreText;
    public TMP_Text timerText;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        scoreText.text = "Pontos: " + score;

        timerText.text = Mathf.Ceil(timeLeft).ToString();

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            EndGame();
        }
    }

    public void AddPoints(int value)
    {
        score += value;
    }

    void EndGame()
    {
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.SetInt("TargetScore", targetScore);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetString("CurrentScene", SceneManager.GetActiveScene().name);

        if (score >= targetScore)
        {
            PlayerPrefs.SetInt("Won", 1);

            int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

            if (currentLevel + 1 > unlocked)
            {
                PlayerPrefs.SetInt("UnlockedLevel", currentLevel + 1);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Won", 0);
        }

        SceneManager.LoadScene("Result");
    }
}