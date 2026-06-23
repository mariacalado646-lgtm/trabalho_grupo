using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerConnect3 : MonoBehaviour
{
    [SerializeField] public string nextLevelName;
    
    public static GameManagerConnect3 instance; // static reference

    public GameObject backgroundPanel; // gray background
    public GameObject victoryPanel;
    public GameObject losePanel;
    public GameObject pausePanel;
    
    public float startTime; // time to end game
    public float time; // time 'til the game ends
    public int goal; // amount of points to win
    public int points; //current points

    public bool isGameEnded;
    public bool isGamePaused;
    
    public TMP_Text timerTxt;
    public TMP_Text pointsTxt;
    public TMP_Text goalTxt;
    public TMP_Text winTxt;

    public bool isTimerCounting = false;
    
    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip backgroundMusic;
    private bool isMuted = false;
    public AudioSource sfxSource;
    // win sound
    public AudioClip winMusic;
    // lose sound
    public AudioClip loseMusic;
    
    [SerializeField] public int currentLevel;
    
    public Button muteButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    
    public void Awake()
    {
        instance = this;
    }

    public void Initialize(int _goal, float _time)
    {
        goal = _goal;
        time = _time;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (MusicManager.Instance != null)
        {
            Destroy(MusicManager.Instance.gameObject);
            MusicManager.Instance = null;
        }
        
        goalTxt.text = goal.ToString();
        isTimerCounting = true;
        startTime++;
        time = startTime;
        DisplayTime(time);
        pointsTxt.text = "0";
        Debug.Log($"Start: time={time}, startTime={startTime}");
        
        // start music
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        pointsTxt.text = points.ToString();
        
        // if time bigger than 0, game is still running and timer is till counting
        if (time > 0 && !isGameEnded &&  isTimerCounting)
        {
            time -= Time.deltaTime;
        }
        else if (!isGameEnded && isTimerCounting && time <= 0)
        {
            isTimerCounting = false;
            time = 0;
            isGameEnded = true;
            backgroundPanel.SetActive(true);
            losePanel.SetActive(true);
            sfxSource.PlayOneShot(loseMusic);
        }
        DisplayTime(time);
    }

    public void ProcessTurn(int _pointsToGain)
    {
        points += _pointsToGain;
    
        if (points >= goal)
        {
            isGameEnded = true;
            PlayerPrefs.SetInt($"Level_{currentLevel}_Complete", 1);
            PlayerPrefs.Save();
            winTxt.text = $"Parabéns, completaste o nível em apenas {Mathf.FloorToInt(GetElapsedTime())} segundos!"; 
            backgroundPanel.SetActive(true);
            victoryPanel.SetActive(true);
            sfxSource.PlayOneShot(winMusic);
        }
    }

    #region Buttons

    // attach to UI buttons to change scene when winning
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }
    public void LevelsScreen()
    {
        SceneManager.LoadScene("Connect3Levels");
    }

    public void PauseGame()
    {
        isGamePaused = true;
        backgroundPanel.SetActive(true);
        pausePanel.SetActive(true);
        isTimerCounting = false;
        Debug.Log("Game paused");
    }
    
    public void ResumeGame()
    {
        isGamePaused = false;
        backgroundPanel.SetActive(false);
        pausePanel.SetActive(false);
        isTimerCounting = true;
        Debug.Log("Game resumed");
    }
    
    public void ToggleMusic()
    {
        isMuted = !isMuted;
        musicSource.mute = isMuted;
        muteButton.GetComponent<Image>().sprite = isMuted ? musicOffSprite : musicOnSprite;
    }
    
    #endregion
    
    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
    
        timerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public float GetElapsedTime()
    {
        return startTime - time;
    }
}
