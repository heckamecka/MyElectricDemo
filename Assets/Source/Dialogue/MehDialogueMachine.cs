using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MehCommandDefinitions;

public enum Guppy { NULL, JEMISON, COOPER, ARMSTRONG}

public class MehDialogueMachine : MonoBehaviour {
    //Helper types:
    public enum DialogueMode { Guppy, Npc }

    //Scene References:
    [SerializeField] public MehDialogueRunner _dgRunner;
    [SerializeField] private MehText _text;
    [SerializeField] private MehGuppy guppy;
    [SerializeField] private MehBackground backgroundGraphic;
    [SerializeField] private MehAudio _audio;
    [SerializeField] private MehResourceLookups resourceLookups;
    [SerializeField] private SwitchBreaker _switchBreaker;
    [SerializeField] private MehRoomManager _room;
    [SerializeField] private HadfieldBanner _hadfield; 

    //Tweakables:
    [SerializeField] private TextSpeedMode defaultSpeed;
    [SerializeField] private float lineEndDelay = 0.5f;
    [Tooltip("Time scale for when players tap to advance dialogue. Minimum 1.0")]
    [SerializeField] private float dgSpeedMultiplier = 10f;

    //Cache:
    private MehCommandLookups _commandDic;                          // Reference to my command dictionary
    private TextSpeedMode _activeTextSpeed;                         // The TextSpeedMode currently in use for printing output
    // Who is speaking, what mode we're in - Michel
    public Guppy activeGuppy { get; private set; }                  // Which guppy is currently active - Michel
    public DialogueMode dialogueMode { get; private set; }          // Determines how and when text is pushed to the text window (require interaction if in player mode, go automatically if in npc mode)
    private string _speakerLookup = "none";                         // Look up storage for who to set speaker to when RunLine is called - Michel
    // coroutine for pausing - Michel
    private Coroutine _pausedDialogueCoroutine;                     // Countdown coroutine that we use for pauses. Cached so that we can stop it as necessary.
    // Running a line - Michel
    private int stringProgress = 0;                                 // current progress of typing out text - Michel
    private string displayText;                                     // text to be displayed on screen - Michel
    public bool gottaDefrost;
    bool shouldBeFrozen { get { return this.dialogueMode == DialogueMode.Guppy && !this._switchBreaker.isEngaged; } }

    // time stuff - Michel
    private float timeScale { get { return _skipScale * debugFastForwardScale * _pauseTimeScale; } }// timeScale that needs to affect all timing based coroutines - Michel
    private bool isWaiting = false;                                 // check if the NPC is waiting for player input to advance dialogue - Michel
    private float _skipScale = 1.0f;                                // Time scale for fast forwarding through dialogue on player input - Michel
    static public float _pauseTimeScale = 1.0f;                     // time scale for pausing, through settings menu or otherwise - Michel


    // debug stuff - Michel
    [HideInInspector] public float debugFastForwardScale = 1.0f;    // time scale for fast forwarding through dialogue on debug fast forward - Michel
    [HideInInspector] public bool debugFastForward = false;

    private void Awake()
    {
        this.activeGuppy = Guppy.NULL;
        this.dialogueMode = DialogueMode.Npc;
        this._commandDic = new MehCommandLookups(this.guppy, this.backgroundGraphic, this._audio, this, this._switchBreaker, this._room, this._hadfield);
        this._text.SetText("");
        //Create a textSpeedMode with default values. This will be overwritten whenever we recieve a text speed change command from Yarn
        this._activeTextSpeed = this.defaultSpeed;
        this._switchBreaker.Initialize();
        dgSpeedMultiplier = Mathf.Max(dgSpeedMultiplier, 1.0f);
        debugFastForward = false;
        debugFastForwardScale = 1.0f;
        _pauseTimeScale = 1.0f;
    }

    private void Start()
    {
        _room.ActivateSpeaker("none");
        StartCoroutine(_text.SetText(""));
    }

    public IEnumerator RunLine(string line) {

        // Reset timescale - Michel
        _skipScale = 1.0f;

        // decide who's actually talking
        _room.ActivateSpeaker(_speakerLookup);

        //Process our Yarn line for further Meh-specific syntax:
        Dictionary<int, List<Command>> inlineCommandsByIndex;
        displayText = ""; //The text the player will actually see
        MehLineParser.ParseInlineCommands(line, out displayText, out inlineCommandsByIndex);

        // set up console text
        _text.maxVisibleCharacters = 0;
        yield return _text.SetText(displayText);

        // DISPLAYING TEXT
        // ------------------------------------------------------
        //If we have 0 characters but still have commands, we still run those commands:
        if (displayText.Length == 0 && inlineCommandsByIndex.ContainsKey(-1))
        {
            foreach (var command in inlineCommandsByIndex[-1])
            {
                yield return this.StartCoroutine(this.RunCommand(command));
            }
        }
        else {
            //Print out the message one character at a time:
            float t = 0;
            stringProgress = 0;
            while (stringProgress < displayText.Length)
            {

                while (t < 0 && stringProgress < displayText.Length)
                {
                    //Look up the appropriate delta for the character we just displayed:
                    t += this._activeTextSpeed.getCharDelta(displayText[stringProgress]);

                    // Display the text
                    _text.maxVisibleCharacters = stringProgress + 1;

                    //Run our inline commands:
                    if (inlineCommandsByIndex.ContainsKey(stringProgress))
                    {
                        foreach (var command in inlineCommandsByIndex[stringProgress])
                        {
                            yield return this.StartCoroutine(this.RunCommand(command));
                        }
                    }
                    stringProgress++;
                }

                // Tick down the dialogue timer, unless we're frozen (due to it being Guppy Dialogue and the
                // the player isn't engaging the switch breaker):
                if (!this.shouldBeFrozen)
                {
                    t -= Time.deltaTime * timeScale;
                    if (gottaDefrost)
                    {
                        this.guppy.UnfreezeGuppy();
                        gottaDefrost = false;
                    }
                    //If frozen, set to "buffer" guppy
                }
                else if (this.shouldBeFrozen)
                {
                    if (!gottaDefrost)
                    {
                        this.guppy.FreezeGuppy();
                        gottaDefrost = true;
                    }
                }

                yield return null;
            }
        }
        
        // DISPLAY FINISHED, WAITING INPUT
        // ------------------------------------------------------
        // SB is open, chill out until the player makes a selection - Michel
        if (_switchBreaker.isOpen)
        {
            if (_switchBreaker.isEngaged) // if it is engaged, the last line that was just run was a Guppy line, so try to try to close it
            {
                _switchBreaker.EndOfLineCloseNode(activeGuppy);
            }

            while (_switchBreaker.isOpen)
            {
                yield return null;
            }
        }
        // The npc was talking, wait for the player to input before moving on - Michel
        else if (this.dialogueMode == DialogueMode.Npc && !debugFastForward)
        {
            isWaiting = true;
            while (isWaiting)
            {
                yield return null;
            }
        }

        //If the line was at least one non-whitespace character long, then wait for LineEndDelay seconds:
        if (displayText.Trim().Length > 0)
        {
            yield return new WaitForSeconds(this.lineEndDelay * 1 / timeScale);
        }
    }

    // When player taps to advance dialogue, specifically used to affect RunLine() - Michel
    public void AdvanceDialogue()
    {
        // if the guppy is speaking, just ignore advancing the dialogue
        if (this.dialogueMode != DialogueMode.Npc) return;

        if (isWaiting)
        {
            isWaiting = false;
        }
        else
        {
            _skipScale = dgSpeedMultiplier;
        }
    }

    public IEnumerator RunCommand(Command command) {
        // Debug.Log("Running Command: " + command.text + " at time " + (Mathf.Round(Time.time * 100) / 100));
        yield return this._commandDic.RunCommand(command);
    }

    /// <summary>
    /// Stops all coroutine processes. - Michel
    /// TODO: make sure function releases coroutine threads safely to Dialogue Runner
    /// </summary>
    public void StopProcceses()
    {
        StopAllCoroutines();
    }

    /// <summary>
    ///  Simple wait for switch breaker
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaitForSB()
    {
        if (_switchBreaker.isOpen)
        {
            while (_switchBreaker.isOpen)
            {
                yield return null;
            }
        }
    }

    #region Setting Dialogue Mode, Speaker, and Active Guppy
    /// <summary> Sets dialogue more, active character, and speaker look up. 
    /// Called through yarn functions, will close all nodes - Michel </summary>
    public void SetSpeakerNPC(string lookup)
    {
        dialogueMode = DialogueMode.Npc;
        _speakerLookup = lookup;

        this.activeGuppy = Guppy.NULL;
        this._switchBreaker.closeAllNodes();
    }

    /// <summary> Sets speaker to the npc on the right, Called automatically when 
    /// jumping to a new node, will close all nodes - Michel </summary>
    public void SetSpeakerDefault()
    {
        SetSpeakerNPC("npc");
    }

    public void SetSpeakerGuppy(Guppy guppy)
    {

        dialogueMode = DialogueMode.Guppy;
        activeGuppy = guppy;

        // set speaker look up - Michel
        switch (guppy)
        {
            case Guppy.JEMISON:
                _speakerLookup = "jemison";
                break;
            case Guppy.COOPER:
                _speakerLookup = "cooper";
                break;
            case Guppy.ARMSTRONG:
                _speakerLookup = "armstrong";
                break;
            default:
                SetSpeakerDefault();
                Debug.LogError("guppy identifier [" + guppy + "] is set incorrectly");
                break;
        }
  
    }

    #endregion

    #region Yarn Command Definitions

    public Coroutine pauseForSeconds(float duration)
    {
        this._pausedDialogueCoroutine = this.StartCoroutine(this.pauseForSecondsSequence(duration));
        return this._pausedDialogueCoroutine;
    }
    
    private IEnumerator pauseForSecondsSequence(float duration)
    {
        float timer = 0;
        while (timer < duration * 1 / timeScale) {
            timer += Time.deltaTime;
            yield return null; 
        }
    }
    
    public void madlibsText(string stringSequence)
    {
        // split string sequence by | - Michel
        string[] w = stringSequence.Split('|');
        // pick a random string from the array - Michel
        string s = w[UnityEngine.Random.Range(0, w.Length)];
        // insert that string into display text by current stringProgress - Michel
        displayText = displayText.Insert(stringProgress+1, s);
    }

    public void setSpeed(string lookup)
    {
        this._activeTextSpeed = this.resourceLookups.textSpeeds[lookup];
    }

    // Deprecated, but still wanna keep this around just in case - Michel
    public IEnumerator manualJump(string node, bool abrupt = false)
    {
        if(!abrupt)
            yield return new WaitForSeconds(this.lineEndDelay * 1/timeScale);
        _dgRunner.BreakJumpNode(node);
    }

    #endregion

    // Functions used by the Switch Breaker
    #region Switchbreaker Hooks:
    public void goToNode(string passageName, Guppy option)
    {
        if (this._pausedDialogueCoroutine != null)
            this.StopCoroutine(this._pausedDialogueCoroutine);
        
        SetSpeakerGuppy(option);
        // using speaker look up because SetSpeakerGuppy should have set that already
        _room.SetGuppy(_speakerLookup);
        
        // don't wait for line to finish
        _dgRunner.BreakJumpNode(passageName);
    }

    public void ChooseGate(Guppy option)
    {
        if (this._pausedDialogueCoroutine != null)
            this.StopCoroutine(this._pausedDialogueCoroutine);
        
        SetSpeakerGuppy(option);
        // using speaker look up because SetSpeakerGuppy should have set that already
        _room.SetGuppy(_speakerLookup);

        char c = 'n';

        // speaker to char
        switch (option)
        {
            case Guppy.JEMISON:
                c = 'j';
                break;
            case Guppy.COOPER:
                c = 'c';
                break;
            case Guppy.ARMSTRONG:
                c = 'a';
                break;
            default:
                Debug.Log("Error in Choosing Gate, option " + option + " not recognized as a valid guppy character. It's probably Michel that fucked up.");
                return;
        }

        _dgRunner.GateInsert(c);
    }

    #endregion



}