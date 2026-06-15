using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelIntro : MonoBehaviour
{
    public TMP_Text levelNameText;
    public TMP_Text targetScoreText;
    public TMP_Text timeText;

    public string levelSceneName = "Level1";
    public int levelNumber = 1;
    public int targetScore = 10;
    public float totalTime = 30f;

    public AudioClip clickSound;

    [Range(0f, 1f)]
    public float extraDelay = 0.1f;

    AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        levelNameText.text = "Nível " + levelNumber;
        targetScoreText.text = "Meta: " + targetScore;
        timeText.text = "Tempo: " + totalTime + " segundos";
    }

    public void StartLevel()
    {
        StartCoroutine(PlayAndLoad());
    }

    IEnumerator PlayAndLoad()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            yield return new WaitForSeconds(clickSound.length + extraDelay);
        }

        SceneManager.LoadScene(levelSceneName);
    }
}