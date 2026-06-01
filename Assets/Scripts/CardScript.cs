using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{
    public Image Below;
    public Image Cover;

    public void Awake()
    {
        Below.gameObject.SetActive(true);
        Cover.gameObject.SetActive(true);
    }

    public void SetBelowColor(Color newColor)
    {
        Below.color = newColor;
    }

    public void SetBelowImage(Sprite newImage)
    {
        Below.color = Color.white;
        Below.sprite = newImage;
    }

    public void DisableCover()
    {
        Cover.gameObject.SetActive(false);
    }

    public void EnableCover()
    {
        Cover.gameObject.SetActive(true);
    }
}