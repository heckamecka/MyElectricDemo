using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "Scene Data", menuName = "MEH/Scene Data", order = 0)]
public class SceneData : ScriptableObject {

    public string _sceneName;
    [TextArea(3, 10)]
    [SerializeField] private string _shantyText;

    [SerializeField] private TextAsset _yarnScene;
    
    public string GetShantyText() { return _shantyText; }

    public TextAsset GetYarnScene() { return _yarnScene; }

}
