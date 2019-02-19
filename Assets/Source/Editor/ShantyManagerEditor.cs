using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Simple button that starts shanty manager for testing purposes
[CustomEditor(typeof(ShantyManager))]
public class ShantyManagerEditor : Editor {

    public override void OnInspectorGUI()
    {

        ShantyManager shanty = (ShantyManager)target;
        if(GUILayout.Button("Run Shanty") && Application.isPlaying)
        {
            shanty.RunShanty(shanty._sceneData);
        }

        DrawDefaultInspector();

    }


}
