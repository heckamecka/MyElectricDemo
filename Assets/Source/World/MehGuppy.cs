using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MehGuppy : MehNpc {
    [SerializeField] private MehDialogueMachine dialogue;

    // Face Variables
    [SerializeField] private Image guppyFace;
    [SerializeField] private CanvasGroup faceGroup;

    // Aura Variables
    [SerializeField] private Image auraImage;
    [SerializeField] private CanvasGroup auraGroup;

    // Move these colors into resources
    public Color jemisonColor;
    public Color cooperColor;
    public Color armstrongColor;
    
    [SerializeField] private GameObject danceGuppyIdle;
    [SerializeField] private GameObject danceGuppyBasic;
    [SerializeField] private GameObject danceGuppyAdvanced;

    private string currentGuppy;
    private string currentFace;

    // SETTING GRAPHICS FOR GUPPY, AURA, AND FACE
    #region Setting Graphics

    public void SetGraphics(string lookup)
    {
        if(lookup == "none") {
            this.characterGroup.alpha = 0;
        }
        else {
            dialogue.gottaDefrost = false; 
            this.characterGroup.alpha = 1;
            this.characterImage.sprite = resources.guppies[lookup];
            SetAura(lookup);
        }

        if (lookup != "neutral")
        {
            currentGuppy = lookup;
            //SetAura("none");
        }
    }
    // Sets the color aura around the guppy based on whether it's one of the slimes
    public void SetAura(string lookup)
    {
        if (lookup == "none")
        {
            this.auraGroup.alpha = 0;
        }
        else
        {
            this.auraGroup.alpha = 1;
            this.auraImage.sprite = resources.guppies[lookup];
            if (lookup == "jemison")
            {
                auraImage.color = jemisonColor;
            }
            else if (lookup == "cooper")
            {
                auraImage.color = cooperColor;
            }
            else if (lookup == "armstrong")
            {
                auraImage.color = armstrongColor;
            }
        }
    }
    
    // Sets the current face of the guppy
    public void SetFace(string lookup)
    {
        if (lookup == "none")
        {
            this.faceGroup.alpha = 0;
        }
        else
        {
            if (resources.faces.ContainsKey(lookup))
            {
                this.faceGroup.alpha = 1;
                this.guppyFace.sprite = resources.faces[lookup];
                currentFace = lookup;
            }
            else
            {
                Debug.LogError("Guppy face [" + lookup + "] not found!");
            }
        }
    }

    public void FreezeGuppy()
    {
        SetGraphics(lookup: "neutral");
        SetFace(lookup: "none");
    }

    public void UnfreezeGuppy()
    {
        SetGraphics(currentGuppy);
        SetFace(currentFace);
    }

    #endregion

    public void SetDance(string lookup)
    {
        danceGuppyAdvanced.SetActive(false);
        danceGuppyBasic.SetActive(false);
        danceGuppyIdle.SetActive(false);

        if (lookup == "idle")
        {
            danceGuppyIdle.SetActive(true);
        }
        else if (lookup == "basic")
        {
            danceGuppyBasic.SetActive(true);
        }
        else if (lookup == "advanced")
        {
            danceGuppyAdvanced.SetActive(true);
        }
    }


}
