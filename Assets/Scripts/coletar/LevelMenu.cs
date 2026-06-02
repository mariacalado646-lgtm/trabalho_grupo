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
        int unlocked =
            PlayerPrefs.GetInt(
                "UnlockedLevel",
                1
            );

        level2Button.interactable =
            unlocked >= 2;

        level3Button.interactable =
            unlocked >= 3;

        lock2.SetActive(
            unlocked < 2
        );

        lock3.SetActive(
            unlocked < 3
        );
    }

    public void PlayLevel1()
    {
        SceneManager.LoadScene(
            "Level1"
        );
    }

    public void PlayLevel2()
    {
        SceneManager.LoadScene(
            "Level2"
        );
    }

    public void PlayLevel3()
    {
        SceneManager.LoadScene(
            "Level3"
        );
    }
}