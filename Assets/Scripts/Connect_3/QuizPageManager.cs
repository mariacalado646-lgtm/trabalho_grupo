using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BriefingManager : MonoBehaviour
{
    [System.Serializable]
    public class Page
    {
        [TextArea(3, 10)]
        public string text;
    }

    public Page[] pages;
    public TMP_Text contentText;
    // public TMP_Text pageCounterText;
    public GameObject nextButton;
    public GameObject startButton;
    public string gameScene;

    private int currentPage = 0;

    void Start()
    {
        Debug.Log($"Total pages: {pages.Length}");
    
        // test: wire the button via code instead of Inspector
        // GameObject.Find("nextButton").GetComponent<Button>().onClick.AddListener(NextPage);

        ShowPage(0);
    }

    public void NextPage()
    {
        Debug.Log($"NextPage called, going to page {currentPage + 1}");
        currentPage++;
        if (currentPage < pages.Length)
        {
            ShowPage(currentPage);
        }
    }

    void ShowPage(int index)
    {
        contentText.text = pages[index].text;
        // pageCounterText.text = $"{index + 1}/{pages.Length}";

        // on last page, swap Next for Start
        if (index >= pages.Length - 1)
        {
            nextButton.SetActive(false);
            startButton.SetActive(true);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameScene);
    }
}