using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Label for when player hovers over/selects the location
public class LocationFile : MonoBehaviour {

    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI _sceneDescription;
    [SerializeField] Image _startSceneButton;

    [HideInInspector] public MapNode _node;

    public void ShowLabel(bool show)
    {
        // play animation for file to move up
        anim.SetBool("show", show);
        _startSceneButton.raycastTarget = show;
    }

    public void SetText(string sceneName, string sceneDescription)
    {
        _sceneDescription.SetText(sceneName +"\n\n"+ sceneDescription);
    }

    // Propagate the start scene command to mapNode
    public void StartScene()
    {
        _node.StartScene();
    }

}
