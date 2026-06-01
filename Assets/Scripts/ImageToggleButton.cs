using UnityEngine;
using UnityEngine.UI;

public class ImageToggleButton : MonoBehaviour
{
    public Image targetImage;

    public Sprite image1;
    public Sprite image2;

    [Header("Som")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private bool isImage1 = true;

    public void ToggleImage()
    {
        isImage1 = !isImage1;

        targetImage.sprite = isImage1
            ? image1
            : image2;

        // Só toca quando mostrar a imagem 2
        if (!isImage1)
        {
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound);
            }
        }
    }
}