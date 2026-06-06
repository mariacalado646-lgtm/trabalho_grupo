using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelIntro : MonoBehaviour
{
    public TMP_Text levelNameText;
    public TMP_Text targetScoreText;
    public TMP_Text timeText;

    public string levelSceneName = "Level1";
    public int levelNumber = 1;
    public int targetScore = 10;
    public float totalTime = 30f;

    void Start()
    {
        levelNameText.text = "Nível " + levelNumber;
        targetScoreText.text = "Meta: " + targetScore;
        timeText.text = "Tempo: " + totalTime + " segundos";
    }

    public void StartLevel()
    {
        SceneManager.LoadScene(levelSceneName);
    }
}