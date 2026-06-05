using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class ResultScreen : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text scoreText;
    public TMP_Text targetText;

    public GameObject nextButton;
    public GameObject restartButton;

    void Start()
    {
        int score =
            PlayerPrefs.GetInt(
                "LastScore",
                0
            );

        int target =
            PlayerPrefs.GetInt(
                "TargetScore",
                0
            );

        int won =
            PlayerPrefs.GetInt(
                "Won",
                0
            );

        scoreText.text = "Pontuação: " + score;


        targetText.text = "Meta: " + target;

        if (won == 1)
        {
            resultText.text =
                "VITÓRIA";

            nextButton.SetActive(true);

            restartButton.SetActive(false);
        }
        else
        {
            resultText.text =
                "DERROTA";

            nextButton.SetActive(false);

            restartButton.SetActive(true);
        }
    }

    public void NextLevel()
    {
        int current =
            PlayerPrefs.GetInt(
                "CurrentLevel"
            );

        SceneManager.LoadScene(
            current + 1
        );
    }

    public void Restart()
    {
        int current =
            PlayerPrefs.GetInt(
                "CurrentLevel"
            );

        SceneManager.LoadScene(
            current
        );
    }

    public void Menu()
    {
        SceneManager.LoadScene(
            "Menu"
        );
    }
}