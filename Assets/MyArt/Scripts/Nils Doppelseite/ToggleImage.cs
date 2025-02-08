using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageToggle : MonoBehaviour
{
    public Image firstImage;
    public Image secondImage;

    void Start()
    {
        firstImage.gameObject.SetActive(true);
        secondImage.gameObject.SetActive(false);
    }

    public void ToggleImages()
    {
        bool isFirstActive = firstImage.gameObject.activeSelf;
        firstImage.gameObject.SetActive(!isFirstActive);
        secondImage.gameObject.SetActive(isFirstActive);
    }
}
