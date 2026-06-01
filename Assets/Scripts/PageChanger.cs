using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PageChanger : MonoBehaviour
{
    [Header("Som do botão")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    public void ChangePage(string sceneName)
    {
        StartCoroutine(ChangePageWithSound(sceneName));
    }

    private IEnumerator ChangePageWithSound(string sceneName)
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);

            yield return new WaitForSeconds(clickSound.length);
        }

        SceneManager.LoadScene(sceneName);
    }
}