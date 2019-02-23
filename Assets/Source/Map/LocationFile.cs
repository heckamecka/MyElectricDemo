using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Label for when player hovers over/selects the location
// TODO: polish, talk w/ art/design on how to pretty it up/ make it more usable
public class LocationFile : MonoBehaviour {

    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI _sceneDescription;

    [HideInInspector] public MapNode _node;

    public void ShowLabel(bool show)
    {
        // play animation for file to move up
        anim.SetBool("show", show);
    }

    public void SetText(string sceneName, string sceneDescription)
    {
        _sceneDescription.SetText(sceneName +"\n\n"+ sceneDescription);
    }

}
