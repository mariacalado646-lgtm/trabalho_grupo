using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    public void Pause()
    {
        pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void Menu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MenuColetar");
    }
}