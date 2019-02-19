using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Needle : MonoBehaviour {

    [SerializeField] private MehDialogueMachine _dialogue;
    [SerializeField] private float jemisonRot = 240f;
    [SerializeField] private float cooperRot = 0f;
    [SerializeField] private float armstrongRot = 120f;

    [SerializeField] public int spinRate = 10;
    [SerializeField] public int idleRate = -1;

    private RectTransform rect;
    
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    
    // shhh it's functional
    void Update()
    {
        switch (_dialogue.activeGuppy)
        {
            case Guppy.JEMISON:
                rect.Rotate(new Vector3(0, 0, (jemisonRot - rect.eulerAngles.z) / spinRate));
                break;
            case Guppy.COOPER:
                rect.Rotate(new Vector3(0, 0, (cooperRot - rect.eulerAngles.z) / spinRate));
                break;
            case Guppy.ARMSTRONG:
                rect.Rotate(new Vector3(0, 0, (armstrongRot - rect.eulerAngles.z) / spinRate));
                break;
            default:
                rect.Rotate(new Vector3(0, 0, idleRate));
                break;
        }
    }
}
