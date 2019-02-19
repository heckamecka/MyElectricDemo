using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple class for just blocking all input, used when fading out during loading scenes - Michel
public class TouchBlocker : MonoBehaviour {

    [SerializeField] CanvasGroup _canvasGroup;

    static TouchBlocker _instance;
    static TouchBlocker instance { get {
            if (_instance == null)
            {
                Debug.LogError("Error: no touch blocker assigned to this scene. Please set one up; there should be a prefab under Asset/Source/GameManager.");
                return null;
            }
            else return _instance;
        } }

    // for inputs that don't require raycast, like the Puck or key presses - Michel
    bool _canInteract = false;
    static bool canInteract { get { return instance._canInteract; } }
    
    
	void Start () {
        // no matter what scene we're in, assume this is gonna be the Touch Blocker - Michel
        _instance = this;
        _canInteract = true;
        _canvasGroup.blocksRaycasts = false;
	}

    // Making these static means I don't have to look for references everytime i wanna call them, and there should 
    // only be one of these per scene, as there should be only 1 per canvas - Michel

    public static void BlockInput()
    {
        if (instance != null)
        {
            instance._canvasGroup.blocksRaycasts = true;
            instance._canInteract = false;
        }
    }

    public static void AllowInput()
    {
        if (instance != null)
        {
            instance._canInteract = true;
            instance._canvasGroup.blocksRaycasts = false;
        }
    }

    public static void ToggleInput()
    {
        if (instance != null)
        {
            if (instance._canInteract) BlockInput();
            else AllowInput();
        }
    }
	
}
