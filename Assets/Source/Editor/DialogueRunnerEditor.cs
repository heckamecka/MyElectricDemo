using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Simple button that starts dialogue for testing purposes
[CustomEditor(typeof(MehDialogueRunner))]
public class DialogueRunnerEditor : Editor
{

    public override void OnInspectorGUI()
    {

        MehDialogueRunner dr = (MehDialogueRunner)target;
        if (GUILayout.Button("Run Dialogue") && Application.isPlaying)
        {
            dr.DebugStartScript();
        }

        DrawDefaultInspector();

    }


}
