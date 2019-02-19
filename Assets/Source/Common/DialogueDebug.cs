using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDebug : MonoBehaviour {

    [SerializeField] MehDialogueMachine dm;

    private void Start()
    {
        dm.debugFastForwardScale = 1.0f;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFastForward();
        }
	}

    void ToggleFastForward()
    {
        // Toggle the boolean - Michel
        dm.debugFastForward = !dm.debugFastForward;

        // set the time scale
        if (dm.debugFastForward) dm.debugFastForwardScale = 20f;
        else dm.debugFastForwardScale = 1.0f;
    }

}
