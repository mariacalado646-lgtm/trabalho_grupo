using UnityEngine;
using UnityEngine.InputSystem;

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
        float move = 0f;

        if (Keyboard.current.leftArrowKey.isPressed ||
            Keyboard.current.aKey.isPressed)
        {
            move = -1f;
        }
        else if (Keyboard.current.rightArrowKey.isPressed ||
                 Keyboard.current.dKey.isPressed)
        {
            move = 1f;
        }

        rect.anchoredPosition +=
            Vector2.right *
            move *
            speed *
            Time.deltaTime;
    }
}