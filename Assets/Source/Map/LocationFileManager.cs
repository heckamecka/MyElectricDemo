using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationFileManager : MonoBehaviour {
    
    [SerializeField] LocationFile[] _files;

    // Run time
    MapNode[] _nodes;
    int _currentlyOpenFileIndex = -1;
    int initMarker; // used for map node initialization 

    // Use this for initialization
    void Awake () {
        _nodes = new MapNode[_files.Length];

        for (int i = 0; i < _files.Length; ++i) {
            _files[i]._index = i;
            _files[i]._fileManager = this;
        }
	}
	
    public int ReigsterMapNode(MapNode node)
    {
        // ToDo: logic for randomly generating node number, needs to not repeat last nodes
        int nodeIndex = initMarker; 
        _nodes[initMarker] = node;
        initMarker++;
        if (initMarker > _files.Length) Debug.LogError("Not enough files graphics for the number of map nodes");

        // ToDo: do some logic for setting the specific files visible
        _files[nodeIndex].gameObject.SetActive(true);

        return nodeIndex;
    }

    public void ShowFile(int index)
    {
        // toDo: if current file is not the same as the given index, close the current file
        if (index != _currentlyOpenFileIndex) CloseFile();
        // ToDo: Play animation, set current open file as index, and SetHighlighted for the map node with the corresponding index
        _nodes[index].SetHighlight(true);
        _files[index].SetFileShown(true);
        _currentlyOpenFileIndex = index;
    }

    public void CloseFile()
    {
        Debug.Log("close file called");
        if (_currentlyOpenFileIndex < 0) return;
        // find file and map node with open location file index, play the animation on file, setHighlighted to false for map node, set the current open file to null (-1)
        _nodes[_currentlyOpenFileIndex].SetHighlight(false);
        _files[_currentlyOpenFileIndex].SetFileShown(false);
        _currentlyOpenFileIndex = -1;
    }

    public void StartScene(int index)
    {
        _nodes[index].StartScene();
    }

}
