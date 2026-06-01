using UnityEngine;
using UnityEngine.UI;

public class MusicToggleButton : MonoBehaviour
{
    public Image buttonImage;

    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    private bool musicEnabled;

    private void Start()
    {
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;

        UpdateButtonVisual();
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;

        if (MusicManager.Instance != null)
        {
            if (musicEnabled)
                MusicManager.Instance.MusicOn();
            else
                MusicManager.Instance.MusicOff();
        }

        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);

        UpdateButtonVisual();
    }

    private void UpdateButtonVisual()
    {
        buttonImage.sprite = musicEnabled
            ? musicOnSprite
            : musicOffSprite;
    }
}