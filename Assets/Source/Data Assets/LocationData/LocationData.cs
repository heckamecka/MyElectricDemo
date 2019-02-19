using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location Data", menuName = "MEH/Location Data", order = 0)]
public class LocationData : ScriptableObject
{
    public string _locationName;
    [SerializeField] List<SceneData> _sceneDataList;

    // I don't know why i'm doing operator overloading other than maybe code readability - Michel ? 
    public SceneData this[int index]{
        get { return _sceneDataList[index]; }
    }
    /// <summary>
    /// Return the number of scenes in this location - michel
    /// </summary>
    public int SceneCount { get { return _sceneDataList.Count; } }

}