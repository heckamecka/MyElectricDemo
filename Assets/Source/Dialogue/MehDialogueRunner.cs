using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Starting point for running the _main scene
// Main job is to run a list of dialogue lines and commands
// and handling switching to different nodes, returning to main menu screen
// interfaces mainly with MehDialogueMachine - Michel
public class MehDialogueRunner : MonoBehaviour {

    // public variables
    public TextAsset _sourceText;
    public MehDialogueMachine _dialogueMachine;
    public bool isDialogueRunning { get { return (CO_runningDialogue != null); }}

    // Internal variables
    // public MehDialogueMachine
    private MehYarnParser _parser;      // parser used to read text assets
    private Dialogue _dialogue;         // The dialogue class to be run
    private Node _nextNode;
    private SceneData _sceneDataCache;

    // Dialogue runner variables
    private Coroutine CO_runningDialogue;
    private bool _terminateInstruction = false;    // Use something to end instructions better than this
    private bool _terminateNode = false;
    private List<Instruction> _instr;   // instruction block that is currently running
    private int  _instrIndex;

    // return variables
    private string _returnNode;
    private int _returnInstructionIndex;

    // gate variables
    private GateInstruction _gateState;
    public GateInstruction GateState { get { return _gateState; } }


    void Start()
    {
        // Initialize Meh Yarn Parser   
        _parser = new MehYarnParser();
    }

    #region Loading, Starting and Stopping Dialogue
    // Load in a new script, replacing the old one
    public bool LoadScript(TextAsset asset)
    {
        if (asset == null) {
            Debug.LogError("Text asset is null, cannot load script");
            return false;
        }
        _sourceText = asset;
        _dialogue = _parser.Load(asset.text);
        return true;
    }

    /// <summary>
    /// Debug for Editor: clears old dialogue, loads new dialogue, runs it - Michel
    /// </summary>
    public void DebugStartScript()
    {
        StopDialogue();
        if (LoadScript(_sourceText))
        {
           StartDialogue();
        }
    }

    public void StartScript(SceneData sceneData)
    {
        StopDialogue();
        if (LoadScript(sceneData.GetYarnScene()))
        {
            _sceneDataCache = sceneData;
            StartDialogue();
        }
    }

    // Clear out any script inside of the dialogue
    public void Clear()
    {
        if (isDialogueRunning)
        {
            Debug.LogError("Cannot clear script. Dialogue is still running");
            return;
        }
        _dialogue = null;
        _nextNode = null;
    }

    public void StartDialogue(string startNode = "Start")
    {
        // starts dialogue coroutine
        if(_dialogue == null)
        {
            Debug.LogError("Cannot start dialogue, no dialogue loaded in");
            return;
        }

        // stop dialogue if there is one running
        StopDialogue();
        CO_runningDialogue = StartCoroutine(RunDialogue(startNode));
    }

    // Stop 
    public void StopDialogue()
    {
        if (isDialogueRunning)
        {
            StopCoroutine(CO_runningDialogue);
            Clear();
        }
    }

    #endregion

    #region Running Dialogue and Nodes
    /// <summary>
    /// Start the dialogue, from the specified node. - Michel
    /// </summary>
    IEnumerator RunDialogue(string startNode)
    {
        //Debug.Log("Run Dialogue Called");

        // TODO: Add OnDialogueStart

        _nextNode = _dialogue[startNode];
        do
        {
            // Set the next node to run
            Node currentNode = _nextNode;
            _nextNode = null;
            //Debug.Log("Start Node: " + currentNode._title);

            // Some coroutine magic to make sure i can terminate
            // the running node and still be able to proceed - Michel
            _terminateNode = false;
            IEnumerator nodeProcess = RunInstructions(currentNode._execBlock);
            while (!_terminateNode && nodeProcess.MoveNext()) {
                yield return nodeProcess.Current;
            }

            // keep running the nodes while there's still a next one - Michel
        } while (_nextNode != null);

        // TODO: Add OnDialogueEnd delegate

        //Debug.Log("Dialogue Ended");

        // fade out music
        MehGameManager.instance.audioMan.FadeOutMusic(4f);

        // Set current scene as visited
        if (_sceneDataCache != null)
        {
            MehGameManager.instance.persistent.SetSceneVisited(_sceneDataCache._sceneName, true);
        }
        

        // Placeholder return to main menu after dialogue has ended - Michel
        MehGameManager.instance.LoadOverworld();

    }

    IEnumerator RunInstructions(List<Instruction> instr)
    {
        // set running instruction block
        _instr = instr;
        _terminateInstruction = false;
        IEnumerator runningInstruction = null;

        // loop through all the instructions - Michel
        for (_instrIndex = 0; _instrIndex<_instr.Count; ++_instrIndex)
        {
            //Debug.Log("Attempt Instruction No.: " + _instrIndex + "  " + _instr[_instrIndex]._type);
            switch (_instr[_instrIndex]._type)
            {
                case InstructionType.LINE:
                    LineInstruction lineInstr = _instr[_instrIndex] as LineInstruction;
                    //Debug.Log("LINE: " + lineInstr._line);
                    runningInstruction = _dialogueMachine.RunLine(lineInstr._line);
                    break;
                case InstructionType.CMD_CUSTOM:
                    CommandInstruction cmdInstr = _instr[_instrIndex] as CommandInstruction;
                    runningInstruction = _dialogueMachine.RunCommand(cmdInstr.cmd);
                    break;
                case InstructionType.GATE:
                    GateInstruction gateInstr = _instr[_instrIndex] as GateInstruction;
                    // Set up data
                    _gateState = gateInstr;

                    // if there is an initial block
                    if (_gateState.HasBlock('i'))
                    {
                        // insert block
                        GateInsert('i');
                    }
                    // else open up switch breaker, and wait for input - Michel
                    else
                    {
                        // Passing this in as a Command so dialouge machine can handle it- Michel
                        Command gateCmd = new Command();
                        gateCmd.text = "sbgate";
                        yield return _dialogueMachine.RunCommand(gateCmd); 
                        runningInstruction = _dialogueMachine.WaitForSB();
                    }
                    break;
                case InstructionType.ASSIGN:
                    AssignInstruction asgnInstr = _instr[_instrIndex] as AssignInstruction;
                    MehGameManager.instance.persistent.Set(asgnInstr.GetKey(), asgnInstr.GetData());
                    break;
                case InstructionType.IFELSE:
                    IfElseInstruction ifelse = _instr[_instrIndex] as IfElseInstruction;
                    for (int i = 0; i < ifelse._conditions.Count; i++)
                    {
                        Value val = ifelse._conditions[i].Evaluate();
                        if (!Utility.IsOfType(val, VarType.Bool)) // check to see if the evaluated value is actually a boolean
                        {
                            Debug.LogError("if else condition doesn't resolve to a boolean: " + ifelse._conditions[i].ToString());
                        }
                        else
                        {
                            if (val.b)
                            {
                                _instr.InsertRange(_instrIndex + 1, ifelse._instrBlock[i]);
                                break;
                            }
                        }
                    }
                    break;
                case InstructionType.JUMP:
                    // if they should be jumping to a new node - Michel
                    JumpInstruction jumpInstr = _instr[_instrIndex] as JumpInstruction;
                    SetNextNode(jumpInstr._destNode);
                    // automatically assume the next node is going to be an NPC speaking
                    _dialogueMachine.SetSpeakerDefault();
                    // End any processes that the dialogue machine is running - Michel
                    _dialogueMachine.StopProcceses();
                    // Exit current run instructions block
                    yield break;
            }

            // Make sure I can terminate the instruction and still proceed - Michel
            _terminateInstruction = false;
            while (!_terminateInstruction && runningInstruction.MoveNext())
            {
                yield return runningInstruction.Current;
            }

        }

    }
    
    #endregion

    #region Modifying Dialogue while it is running
    
    ///<summary>
    ///Prematurely ends current node that is running and goes to a different node - Michel
    ///</summary>
    public void BreakJumpNode(string nextNode)
    {
        //Debug.Log("break jump node called");

        // Stop all DMachine Coroutines - Michel
        _dialogueMachine.StopProcceses();

        // set next node - Michel
        SetNextNode(nextNode);
        // set terminate to true, terminating the current node process - Michel
        _terminateNode = true;
        // terminate instruction process - Michel
        _terminateInstruction = true;

    }

    /// <summary>
    /// Sets the next node to go to after the current one ends - Michel
    /// </summary>
    public void SetNextNode(string nextNode)
    {
        //Debug.Log("setting next ndoe as: " + nextNode);
        _nextNode = _dialogue[nextNode];
    }

    public void SetReturnAndJump(string nextNode)
    {
        // save current node string + instruction number - Michel
        BreakJumpNode(nextNode);
    }

    public void ResumePrevious()
    {
        // TODO: set next node + instruction number
        // this should probably be called automatically in MehDialogueRunner
    }

    /// <summary>
    /// Used by switch breaker in the gate state to insert a set of instructions
    /// into the currently running node for the corresponding character
    /// </summary>
    public void GateInsert(char option)
    { 
        // Error checking
        if(!_gateState.HasBlock(option)) {
            Debug.LogError("No instruction block for " + option + " option. Cannot insert instructions");
            return;
        }

        // Super not thread safe, but if the switch breaker is waiting for 
        // input the node runner should be as well, I'll probably change
        // it once we're not doing any more major changes
        //  - famous last words - Michel

        _instr.InsertRange(_instrIndex + 1, _gateState.GetBlock(option));
        _gateState.RemoveBlock(option);

        // terminate instruction process to skip to the next line - Michel
        _terminateInstruction = true;

    }

    #endregion


}