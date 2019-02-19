using UnityEngine;

public class MehRoomManager : MonoBehaviour {

    enum Side { NULL, LEFT, RIGHT}

    [SerializeField] private MehDialogueMachine _dialogue;
    [SerializeField] public MehResourceLookups _resource;
    [SerializeField] private MehGuppy _guppy;
    [SerializeField] private MehRoomSide _rightSide;
    [SerializeField] private MehRoomSide _leftSide;
    
    [Header("Movement Variables")]
    [Tooltip("How long it takes for a character to move off/on screen")] 
    public float standardMoveDuration = 1.0f;
    public AnimationCurve moveCurve;
    public float saturationShiftDuration = 0.3f;
    public Color saturatedColor;

    // convert into something more robust if the need rises - Michel
    [Header("NPC Variables")]
    [SerializeField] private Material _defaultMat;
    [SerializeField] private Material _holoMat;

    string _rightSlot { get { return _rightSide._slot; } }
    string _leftSlot { get { return _leftSide._slot; } set { _leftSide._slot = value; } }

    private void Start()
    {
        _rightSide.rm = this;
        _leftSide.rm = this;

        _leftSlot = "guppy";
    }

    public void SetNPC(string lookup, string[] parameters)
    {
        // if look up is none, clear entire board of NPCs - Michel
        if (lookup == "none")
        {
            if (_rightSlot != "") _rightSide.PopActive();
            if (_leftSlot != "" && !isGuppy(_leftSlot)) _leftSide.PopActive();

            if (_rightSide.speakerActive) _rightSide.DeactivateNameTag();
            if (_leftSide.speakerActive) _leftSide.DeactivateNameTag();
            
            return;
        }

        // check if the character exists first 
        lookup = CheckCharacter(lookup);

        Side side = GetSideFromParameters(parameters);

        //If the npc already exists on the right side - Michel
        if (lookup == _rightSlot)
        {
            // parameter spcifies left side
            // remove the character from the right side
            // and put them on the left side - Michel
            if (side == Side.LEFT)
            {
                _rightSide.PopActive();
                _leftSide.SetNPC(lookup);
                // reset the npc, since we're moving a new one onto the screen - Michel
                ResetMehNPC(side);
            }
            else 
            {
                // parameter is either null, or set to right, don't do anything special - Michel
                
                // explicitly set side for later processing - Michel
                side = Side.RIGHT;
            }
        }
        // the npc is on the left side
        else if (lookup == _leftSlot)
        {
            // parameter spcifies right side
            // pop on the left and put them on the right - Michel
            if (side == Side.RIGHT)
            {
                _leftSide.PopActive();
                _rightSide.SetNPC(lookup);
                // reset the npc, since we're moving a new one onto the screen - Michel
                ResetMehNPC(side);
            }
            else
            {
                // parameter is either null, or set to left, don't do anything special - Michel
                // explicitly set side for later processing - Michel
                side = Side.LEFT;
            }
        }
        // The npc isn't already there, move the character onto the screen
        else
        {
            // if we explicitly say the left side, put them on the left side - Michel
            if (side == Side.LEFT)
            {
                // determing if we need to pop the guppy - Michel
                if (isGuppy(_leftSlot)) PopGuppy();
                // Set the npc
                _leftSide.SetNPC(lookup);
            }
            // else, just put them on the right side - Michel
            else
            {
                _rightSide.SetNPC(lookup);
                // explicitly set side for later processing - Michel
                side = Side.RIGHT;
            }
            // reset the npc, since we're moving a new one onto the screen - Michel
            ResetMehNPC(side);
        }

        // execute any other parameters
        NPCParameterFunctions(side, parameters);

    }

    // NPC REWORK TODO: given a set of parameters, determine who needs 
    public void SetNPCFace(string[] parameters)
    {
        Side side = GetSideFromParameters(parameters);
        if (side == Side.NULL) side = Side.RIGHT;

        NPCParameterFunctions(side, parameters);

    }

    // Look through the list of parameters to find if there is a specified side - Michel
    Side GetSideFromParameters(string[] parameters)
    {
        foreach (string s in parameters)
        {
            if (s == "left") return Side.LEFT;
            else if (s == "right") return Side.RIGHT;
        }
        return Side.NULL;
    }

    void NPCParameterFunctions(Side side, string[] parameters)
    {
        MehNpc targetNPC = RoomSide(side).GetActiveNPC();
        
        foreach(string s in parameters)
        {
            if (s == "left" || s == "right" || s == "") ; // do nothing if it's a left or right statement - Michel
            else if(s == "holo")
            {
                targetNPC.SetMaterial(_holoMat);
                //Debug.Log("Set Holo on side " + side);
            }
            else // after passing every other test, assume it's a character face lookup - Michel
            {
                Debug.Log(s);
                targetNPC.SetFace(s);
            }
        }
    }
    
    /// <summary>
    /// Either moves the guppy onto the screen, removes it from the screen,
    /// or sets it to one of the character
    /// </summary>
    public void SetGuppy(string lookup = "")
    {
        // assume we want to pop the guppy - Michel
        if (lookup == "none")
        {
            PopGuppy();
            return;
        }

        // if someone is already there - Michel
        if (_leftSlot != "")
        {
            // if they're not the guppy pop them off and put the guppy on - Michel
            if (!isGuppy(_leftSlot))
            {
                _leftSide.PopActive();
                _leftSide.Push(_guppy.transform);
            }
        }
        else // no one is there, push the guppy on - Michel
        {
            _leftSide.Push(_guppy.transform);
        }

        // set the guppy graphic to the appropriate graphic - Michel
        if (lookup != "")
        {
            _guppy.SetGraphics(lookup);
            _leftSlot = lookup;
        }
        else
        {
            _leftSide.DeactivateNameTag();
            _leftSlot = "guppy"; // i assume if you didn't give me a name, they aren't speaking - Michel
        }
    }
    
    public void PopGuppy()
    {
        _leftSide.Pop(_guppy.transform);
    }

    public void BumpNPC(string magnitude = "10", string lookup = "")
    {
        switch (FindNPC(lookup))
        {
            case Side.LEFT:
                _leftSide.GetActiveNPC().Bump(magnitude);
                break;
            case Side.RIGHT:
                _rightSide.GetActiveNPC().Bump(magnitude);
                break;
            case Side.NULL:
                break;
        }
    }

    public void ShakeNPC(string magnitude = "10", string lookup = "")
    {
        switch (FindNPC(lookup))
        {
            case Side.LEFT:
                _leftSide.GetActiveNPC().Shake(magnitude);
                break;
            case Side.RIGHT:
                _rightSide.GetActiveNPC().Shake(magnitude);
                break;
            case Side.NULL:
                break;
        }
    }

    // Deciding who's speaking - Michel
    #region Set Speaker

    /// <summary>
    /// Given a name lookup, determine the text content and which side of the screen to set the name plate.
    /// Should Only be called by MehDialougeMachine.RunLine() - Michel
    /// </summary>
    /// <param name="lookup"></param>
    public void ActivateSpeaker(string lookup)
    {
        if (lookup == "none") //no one is speaking - Michel
        {
            if (_rightSide.speakerActive) _rightSide.DeactivateNameTag();
            if (_leftSide.speakerActive) _leftSide.DeactivateNameTag();
        }
        else if (lookup == "npc") // if there's an npc, that npc is speaking- Michel
        {
            if (_rightSlot != "") ActivateSpeakerHelper(_rightSlot, Side.RIGHT);
            else if (!isGuppy(_leftSlot)) ActivateSpeakerHelper(_leftSlot, Side.LEFT);
        }
        else if (lookup != "player" && lookup != "guppy") // else, figure out from look up who is speakign - Michel
        { 
            // check if the character exists - Michel
            lookup = CheckCharacter(lookup);

            if (lookup == _leftSlot) ActivateSpeakerHelper(lookup, Side.LEFT);
            else if (lookup == _rightSlot) ActivateSpeakerHelper(lookup, Side.RIGHT);
            // TODO: we're probably gonna wanna make it so that you CAN activate a speaker even with literally nothing on screen
            else Debug.LogError("Speaker ["+ name+"] is not currently on screen (look up was ["+lookup +"])");
        }
    }

    void ActivateSpeakerHelper(string lookup, Side side)
    {
        // set which side we're activating and deactivating - Michel
        MehRoomSide thisSide = _rightSide, otherSide = _leftSide;
        if (side == Side.LEFT) { thisSide = _leftSide; otherSide = _rightSide; }
        
        // Setting the name tag
        if (thisSide.speakerActive) // if the same side is active 
        {
            if (thisSide.GetActiveNameString() != GetNameFromLookup(lookup)) // and the names are different, oops this doesn't work because _slot is already changed before we get here
            {
                // deactivate and reactivate name tag
                thisSide.DeactivateNameTag();
                thisSide.ActivateNameTag(GetNameFromLookup(lookup));
            }
            // names are the same, don't do anything
        }
        else if (otherSide.speakerActive)// other side is active, deactivate other side, activate this side
        {
            otherSide.DeactivateNameTag();
            thisSide.ActivateNameTag(GetNameFromLookup(lookup));
        }
        else // neither side is active
        {
            thisSide.ActivateNameTag(GetNameFromLookup(lookup));
        }

        //Setting the highlight
        // highlight the speaker on this side
        if (isGuppy(thisSide._slot)) _guppy.HighlightCharacter();
        else thisSide.SetSpeakerHighlighted(true);
        //unhighlight the speaker on the other side
        if (isGuppy(otherSide._slot)) _guppy.UnHighlightCharacter();
        else otherSide.SetSpeakerHighlighted(false);

    }

    #endregion

    // Utility functions
    #region utility
    // Get the SideManager given a side var - Michel
    MehRoomSide RoomSide(Side side)
    {
        switch (side)
        {
            case Side.LEFT:
                return _leftSide;
            case Side.RIGHT:
                return _rightSide;
            default:
                return null;
        }
    }
    
    // Given a side, reset all variables the active NPC of that side
    void ResetMehNPC(Side side)
    {
        MehNpc targetNPC = RoomSide(side).GetActiveNPC();
        targetNPC.SetMaterial(_defaultMat);
    }

    // Check if character exists first - Michel
    string CheckCharacter(string lookup)
    {
        if (_resource.npcs.ContainsKey(lookup) || isGuppy(lookup)) return lookup;
        Debug.LogError("Given name [" + lookup + "] does not match any existing character in databse");
        return "none";
    }
    
    // Get the character's name from the lookup
    string GetNameFromLookup(string lookup)
    {
        if (_resource.npcs.ContainsKey(lookup)) return _resource.npcs[lookup].characterName;
        else if (lookup == "jemison") return "JEMISON";
        else if (lookup == "cooper") return "COOPER";
        else if (lookup == "armstrong") return "ARMSTRONG";
        else return "";
    }

    // returns true if the lookup is a guppy lookup
    bool isGuppy(string lookup)
    {
        return (lookup == "jemison" ||
                lookup == "cooper" ||
                lookup == "armstrong" ||
                lookup == "guppy");
    }

    // Find 
    Side FindNPC(string lookup)
    {
        if (lookup == "") //  no lookup found, choose a default npc
        {
            if (_rightSlot != "") return Side.RIGHT;
            else if (_leftSlot != "") return Side.LEFT;
        }
        else // a look up is given
        {
            lookup = CheckCharacter(lookup);

            if (_rightSlot == lookup) return Side.RIGHT;
            else if (_leftSlot == lookup) return Side.LEFT;
            else Debug.LogWarning("Unable to find [" + lookup + "] in the scene");
        }

        // don't match any of the conditions, assume no available npc is on screen
        return Side.NULL;
    }

    #endregion
}
