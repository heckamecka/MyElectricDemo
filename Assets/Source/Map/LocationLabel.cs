using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Label for when player hovers over/selects the location
// TODO: polish, talk w/ art/design on how to pretty it up/ make it more usable
public class LocationLabel : MonoBehaviour {

    [SerializeField] Image _bgImage;
    [SerializeField] TextMeshProUGUI _uiText;

    public void ShowLabel(bool show)
    {
        _bgImage.enabled = show;
        _uiText.enabled = show;
    }

    public void SetText(string content)
    {
        _uiText.SetText(content);
    }

}
