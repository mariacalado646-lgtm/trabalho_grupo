using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelMenu : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;

    public GameObject lock2;
    public GameObject lock3;

    public AudioClip clickSound;

    [Range(0f, 1f)]
    public float extraDelay = 0.1f;

    AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (!PlayerPrefs.HasKey("UnlockedLevel"))
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
        }

        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        level2Button.interactable = unlocked >= 2;
        level3Button.interactable = unlocked >= 3;

        lock2.SetActive(unlocked < 2);
        lock3.SetActive(unlocked < 3);

        level1Button.onClick.AddListener(() => StartCoroutine(PlayAndLoad("IntroLevel1")));
        level2Button.onClick.AddListener(() => StartCoroutine(PlayAndLoad("IntroLevel2")));
        level3Button.onClick.AddListener(() => StartCoroutine(PlayAndLoad("IntroLevel3")));
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