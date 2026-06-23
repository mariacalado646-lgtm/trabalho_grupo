using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        [TextArea(2, 5)]
        public string questionText;
        public string[] options;
        public int correctIndex;
    }

    [Header("Questions")]
    public Question[] questions;

    [Header("UI References")]
    public TMP_Text questionText;
    public TMP_Text questionCounterText;
    public TMP_Text scoreText;
    // public TMP_Text scoreText;
    public TMP_Text feedbackText;
    public Button[] optionButtons;
    public TMP_Text[] optionTexts;

    [Header("Navigation")]
    public string nextScene;

    [Header("Settings")]
    public float delayBetweenQuestions = 1.5f;

    [Header("Feedback Colors")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    [Header("Continue Button")]
    public Button continueButton;
    
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioClip loseSound;
    
    private int currentQuestion = 0;
    private int score = 0;
    private bool answered = false;

    void Start()
    {
        continueButton.gameObject.SetActive(false);
        
        Debug.Log($"Total questions: {questions.Length}");
        ShowQuestion(0);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => SelectAnswer(index));
        }
    }

    void ShowQuestion(int index)
    {
        answered = false;
        feedbackText.text = "";

        Question q = questions[index];
        questionText.text = q.questionText;
        questionCounterText.text = $"{index + 1}/{questions.Length}";
        // scoreText.text = $"Pontos: {score}/{questions.Length}";

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < q.options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);
                optionTexts[i].text = q.options[i];
                optionButtons[i].GetComponent<Image>().color = defaultColor;
                optionButtons[i].interactable = true;
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SelectAnswer(int selected)
    {
        if (answered) return;
        answered = true;

        Question q = questions[currentQuestion];

        for (int i = 0; i < optionButtons.Length; i++)
            optionButtons[i].interactable = false;

        optionButtons[q.correctIndex].GetComponent<Image>().color = correctColor;

        if (selected == q.correctIndex)
        {
            score++;
            feedbackText.text = "Correto!";
            feedbackText.color = correctColor;
        }
        else
        {
            optionButtons[selected].GetComponent<Image>().color = wrongColor;
            feedbackText.text = $"Errado! A resposta certa era: {q.options[q.correctIndex]}";
            feedbackText.color = wrongColor;
        }

        scoreText.text = $"Pontos: {score}/{questions.Length}";
        StartCoroutine(AutoAdvance());
    }

    IEnumerator AutoAdvance()
    {
        yield return new WaitForSeconds(delayBetweenQuestions);

        currentQuestion++;
        if (currentQuestion < questions.Length)
            ShowQuestion(currentQuestion);
        else
            ShowResults();
    }

    void ShowResults()
    {
        foreach (Button b in optionButtons)
            b.gameObject.SetActive(false);
        questionCounterText.gameObject.SetActive(false);
        feedbackText.text = "";
        questionText.text = $"Terminaste o quiz!\nAcertaste {score} de {questions.Length} perguntas!";
        scoreText.text = $"Pontos finais: {score}/{questions.Length}";
        continueButton.gameObject.SetActive(true);
        if (score > (questions.Length/2)){audioSource.PlayOneShot(winSound);}
        else{audioSource.PlayOneShot(loseSound);}
    }

    public void GoToNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}