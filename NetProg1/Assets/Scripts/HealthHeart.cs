using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class HealthHeart : MonoBehaviour
{
    public Sprite fullHeart, emptyHeart;
    private Image heartImage;

    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    public void SetHeartImage(HeartStatus status)
    {
        if (status == 0)
        {
            heartImage.sprite = emptyHeart;
        }
        else
        {
            
            heartImage.sprite = fullHeart;
        }
    }
}

public enum HeartStatus
{
    Empty = 0,
    Full = 1,
}