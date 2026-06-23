using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMap : MonoBehaviour
{
    public Button[] levelButtons;
    public string[] levelScenes;

    public Sprite lockedSprite;
    public Sprite unlockedSprite;

    public GameObject[] levelCrowns; // one per level button
    
    public GameObject instructionsPanel;
    
    void Start()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i;
            bool completed = PlayerPrefs.GetInt($"Level_{i}_Complete", 0) == 1;

            // show crown if completed
            levelCrowns[i].SetActive(completed);

            if (i == 0 || PlayerPrefs.GetInt($"Level_{i - 1}_Complete", 0) == 1)
            {
                levelButtons[i].interactable = true;
                levelButtons[i].GetComponent<Image>().sprite = unlockedSprite;
                levelButtons[i].onClick.AddListener(() => LoadLevel(index));
            }
            else
            {
                levelButtons[i].interactable = false;
                levelButtons[i].GetComponent<Image>().sprite = lockedSprite;
            }
        }
    }

    void LoadLevel(int index)
    {
        SceneManager.LoadScene(levelScenes[index]);
    }
    
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void InstructionsPanelOn()
    {
        instructionsPanel.SetActive(true);
    }
    public void InstructionsPanelOff()
    {
        instructionsPanel.SetActive(false);
    }
}