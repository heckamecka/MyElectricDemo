using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour {

    [SerializeField] LocationFileManager _fileManager;
    [SerializeField] LocationData _locationData;
    [SerializeField] Image _image;
    [SerializeField] Animator _nodeAnim;

    [Header("Debug")]
    [SerializeField] bool _ignoreFirstActivate;

    // RunTime Data
    int _index;
    bool _highlighted = false;

    // store reference (at least i think its reference) to the scene that should be loaded
    // when the player taps on this node - Michel
    SceneData _scene;

    // little psuedo macro to cut down on typing - Michel
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
        _index = _fileManager.ReigsterMapNode(this);
    }



    // When the node is pressed/clicked on by the player - Michel
    public void OnClick()
    {
        if (_scene != null)
        {
            SetHighlight(!_highlighted);// toggle highlight

            if (_highlighted) _fileManager.ShowFile(_index);
            else _fileManager.CloseFile();

            Debug.Log("map node on click called");
        }
    }

    // Called from LocationFileManager
    public void SetHighlight(bool newHighLight)
    {
        // dont change anythign if new highlight and old highlight are the same
        if (_highlighted != newHighLight)
        {
            _highlighted = newHighLight;

            _nodeAnim.SetBool("highlighted", _highlighted);

        }

    }

    public void StartScene()
    {
        MehGameManager.instance.LoadShantyScene(_scene);
    }

    #region Map Init Stuff

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
        for (int i = 1; i < _locationData.SceneCount; ++i)
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
    /// On Init, check if a particular scene in the location list is a valid scene to play
    /// </summary>
    bool ValidateScene(int i)
    {
        // if it's been visited already, return false
        if (persistent.GetSceneVisited(_locationData[i].name)) return false;

        // add other conditions in here

        // if it passes all the conditions, return true
        return true;
    }


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
