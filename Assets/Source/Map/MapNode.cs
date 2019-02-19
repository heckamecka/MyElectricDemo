﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour {
    
    [SerializeField] LocationData _locationData;
    [SerializeField] Image _image;
    [SerializeField] LocationLabel _label;


    [Header("Debug")]
    [SerializeField] bool _ignoreFirstActivate;

    // store reference (at least i think its reference) to the scene that should be loaded
    // when the player taps on this node - Michel
    SceneData _scene;

    // little helper function to cut down on typing - Michel
    PersistentData persistent { get { return MehGameManager.instance.persistent; } }

    void Start ()
    {
        // Error check on start - Michel
        if (_locationData == null)
        {
            Debug.LogError("No location set for map node");
            return;
        }

        MapNodeInit();
        ShowLabel(false);
    }

    /// <summary>
    /// Should called whenever the map scene is loaded in - Michel
    /// </summary>
    void MapNodeInit()
    {
        //if the node is not activated at all, hide it - Michel
        if (!persistent.GetLocationActive(_locationData._locationName) && !_ignoreFirstActivate)
        {
            HideNode();
            return;
        }

        // if the first scene hasn't been visited, it means this is the first time
        // this node is activated, so do something special! - Michel
        if (!persistent.GetSceneVisited(_locationData[0]._sceneName))
        {
            FirstActivateNode();
            _scene = _locationData[0];
            return;
        }

        // loop through the entire list 
        for(int i = 1; i < _locationData.SceneCount; ++i)
        {
            // there is a valid location, so let's use that 
            if (ValidateScene(i))
            {
                _scene = _locationData[i];
                return;
            }
        }

        // assuming if we get to this part of the code, no valid location has been found
        // grey out the node - Michel
        GreyOutNode();

    }
    
    /// <summary>
    /// Check if a particular scene in the location list is a valid scene to play
    /// </summary>
    bool ValidateScene(int i)
    {
        // if it's been visited already, return false
        if (persistent.GetSceneVisited(_locationData[i].name)) return false;
        
        // add other conditions in here

        // if it passes all the conditions, return true
        return true;
    }

    // When the node is pressed/clicked on by the player - Michel
    public void OnClick()
    {
        if (_scene != null)
        {
            MehGameManager.instance.LoadShantyScene(_scene);
            Debug.Log("map node on click called");
        }
    }

    public void OnPointerEnter()
    {
        if (_scene == null) return;
        ShowLabel(true);

    }
    public void OnPointerExit()
    {
        if (_scene == null) return;
        ShowLabel(false);
    }

    public void ShowLabel(bool show)
    {
        if (_label == null) { Debug.LogError("Lable for map node not set"); return; }
        _label.ShowLabel(show);

        if (_scene == null) return;
        _label.SetText(_locationData._locationName + "\n<#000000>" + _scene._sceneName + "</color>");
    }

    #region Map Init Results
    // node has not been activated at all- Michel
    void HideNode()
    {
        _image.raycastTarget = false;
        _image.enabled = false;
        Debug.Log("node not activated");
    }

    // first time a node has ever been activated - Michel
    void FirstActivateNode()
    {
        Debug.Log("first activated");
    }

    // node has been activated, but no valid scene is available - Michel
    void GreyOutNode()
    {
        Debug.Log("grey out the node");
        _image.color = new Color(0.5f, 0.5f, 0.5f);
        _image.raycastTarget = false;
    }

    void ActivateNode()
    {

    }
    #endregion
}
