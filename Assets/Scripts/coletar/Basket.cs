using UnityEngine;

public class BasketUI : MonoBehaviour
{
    public float speed = 800f;

    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        float move =
            Input.GetAxis("Horizontal");

        rect.anchoredPosition +=
            Vector2.right *
            move *
            speed *
            Time.deltaTime;
    }
}