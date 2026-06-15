using UnityEngine;

public class FoodUI : MonoBehaviour
{
    public int points = 1;
    public float speed = 400f;

    public AudioClip collectSound;

    RectTransform rect;
    public static RectTransform basket;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        basket = GameObject
            .Find("Basket")
            .GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition +=
            Vector2.down *
            speed *
            Time.deltaTime;

        if (rect.anchoredPosition.y < -700)
        {
            Destroy(gameObject);
        }

        float distance = Vector2.Distance(
            rect.anchoredPosition,
            basket.anchoredPosition
        );

        if (distance < 120)
        {
            PlayCollectSound();

            GameManager.Instance.AddPoints(points);

            Destroy(gameObject);
        }
    }

    void PlayCollectSound()
    {
        if (collectSound != null)
        {
            GameObject soundObj = new GameObject("CollectSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            source.PlayOneShot(collectSound);
            Destroy(soundObj, collectSound.length);
        }
    }
}