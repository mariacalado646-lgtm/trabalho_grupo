using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public Button level2Button;
    public Button level3Button;

    public GameObject lock2;
    public GameObject lock3;

    void Start()
    {
        if (!PlayerPrefs.HasKey("UnlockedLevel"))
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
        }

        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        level2Button.interactable = unlocked >= 2;
        level3Button.interactable = unlocked >= 3;

        lock2.SetActive(unlocked < 2);
        lock3.SetActive(unlocked < 3);
    }

    public void PlayLevel1()
    {
        SceneManager.LoadScene("IntroLevel1");
    }

    public void PlayLevel2()
    {
        SceneManager.LoadScene("IntroLevel2");
    }

    public void PlayLevel3()
    {
        SceneManager.LoadScene("IntroLevel3");
    }
}