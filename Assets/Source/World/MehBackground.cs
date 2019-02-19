using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MehBackground : MonoBehaviour {
    [SerializeField] private MehResourceLookups resources;
    [SerializeField] private Image backgroundImage;

    public void SetGraphic(string lookup)
    {
        this.backgroundImage.sprite = resources.backgrounds[lookup];
    }
}