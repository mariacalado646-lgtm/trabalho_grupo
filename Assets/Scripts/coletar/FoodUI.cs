using UnityEngine;

public class FoodUI : MonoBehaviour
{
    public int points = 1;

    public float speed = 400f;

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

        float distance =
            Vector2.Distance(
                rect.anchoredPosition,
                basket.anchoredPosition
            );

        if (distance < 120)
        {
            GameManager.Instance
                .AddPoints(points);

            Destroy(gameObject);
        }
    }
}