using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class SwitchBreaker : MonoBehaviour {
    [SerializeField] private MehDialogueMachine _dialogue;
    [SerializeField] private NodeButton _jemisonButton;
    [SerializeField] private NodeButton _cooperButton;
    [SerializeField] private NodeButton _armstrongButton;
    [SerializeField] private Image _underlay;
    [SerializeField] private Color[] _underlayColors; //Colors 0-2 for each of the robos, and color 3 for no robo selected

    [Header("Tweakables")]
    [SerializeField] private float _activationSFXDelay = 0.5f;
    [SerializeField] private AudioClip _activationClick;

    public bool isOpen    { get; private set; } //Whether I have one or more candidate nodes available to be switched to
    public bool isEngaged { get; private set; } //Whether the player is currently engaging a node (false if the player's removed their finger from the screen)

    private Dictionary<Guppy, NodeButton> _nodes;

    public void Initialize()
    {
        this._nodes = new Dictionary<Guppy, NodeButton>();
        this._nodes.Add(Guppy.JEMISON, _jemisonButton);
        this._nodes.Add(Guppy.COOPER, _cooperButton);
        this._nodes.Add(Guppy.ARMSTRONG, _armstrongButton);

        _jemisonButton.identifier = Guppy.JEMISON;
        _cooperButton.identifier = Guppy.COOPER;
        _armstrongButton.identifier = Guppy.ARMSTRONG;

        //Disable all nodes at start:
        foreach (var pair in this._nodes)
            pair.Value.MakeDisabled();
    }

    private void Update()
    {
        // I don't know what the underlay does, so i'm just keeping this here for now. I'll probably gut it out when i change the visuals anyways - Michel
        //this._underlay.color = this._underlayColors[this._dialogue.shouldBeFrozen() || this._dialogue.activeCharacter == -1 ? 3 : this._dialogue.activeCharacter];
    }

    /// <summary>
    ///  Receives a list of up to 3 target passage names, each of which needs to start with a j, c, or a. Inject an on-clicked delegate into each of the corresponding j, c, or a buttons 
    ///   which will cause a jump to the corresponding passage upon their being clicked.
    /// </summary>
    public void openFreeSwitch(string[] targetPassageNames)
    {
        StartCoroutine(CO_OpenFreeSwitch(targetPassageNames));
    }

    // Helper function for open free switch
    IEnumerator CO_OpenFreeSwitch(string[] targetPassageNames)
    {
        foreach (var passageName in targetPassageNames)
        {
            if (!"jca".Contains(passageName[0])) throw new System.Exception("Error! Each parameter of sbopen must begin with a j, c, or a");

            // Get the correct node based on first character of passage name
            var node = this._nodes[CharToCharacter(passageName[0])];

            // Activate node
            node.ActivateNode();

            // Inject delegate
            var cachedPassageName = passageName; //Caching because using foreach variable in lambda is apparently dangerous
            node.MakeCandidate(() => this.onCandidateSelected(node, cachedPassageName));

            this.isOpen = true;

            yield return new WaitForSeconds(_activationSFXDelay);
        }
        MehGameManager.instance.audioMan.PlayOneShot(_activationClick);
    }

    /// <summary>
    ///   Inject an on-clicked delegate into each of the corresponding j, c, or a buttons, using data from the dialogue runner
    ///   which will cause the appropriate block of instructions to be inserted - Michel
    /// </summary>
    public void openGateSwitch()
    {
        StartCoroutine(CO_OpenGateSwitch());
    }

    IEnumerator CO_OpenGateSwitch()
    {
        // Grab the stored gate state from dialogue runner - Michel
        GateInstruction gateInstr = _dialogue._dgRunner.GateState;

        if (gateInstr == null) { Debug.LogError("sbgate (or equivalent) was called, but there was no gate instruction set in dialogue runner"); yield break; }

        // if there is an instruction block in the stored gate instruction, open that node - Michel
        foreach (char c in "jca")
        {
            if (gateInstr.HasBlock(c))
            {
                var node = _nodes[CharToCharacter(c)];

                node.ActivateNode();
                node.MakeCandidate(() => OnGateSelected(node));

                this.isOpen = true;

                yield return new WaitForSeconds(_activationSFXDelay);
            }
        }
        MehGameManager.instance.audioMan.PlayOneShot(_activationClick);
    }
    
    private void onCandidateSelected(NodeButton selected, string targetPassage)
    {   
        var oldCharacter = this._dialogue.activeGuppy;
        var newCharacter = selected.identifier;
        
        this.isOpen = false;
        this.isEngaged = true;

        foreach (var n in this._nodes.Values)
            n.MakeDisabled();
        selected.MakeAgent(() => this.onAgentDeselected(selected));

        if (targetPassage == "") throw new System.Exception("Error on selecting passage. Target Passage is " + targetPassage);
        
        this._dialogue.goToNode(targetPassage, selected.identifier);
    }

    private void OnGateSelected(NodeButton selected)
    {
        // Set new character to that of the selected node - Michel
        var newCharacter = selected.identifier;

        this.isOpen = false;
        this.isEngaged = true;
        foreach (var n in this._nodes.Values)
            n.MakeDisabled();
        selected.MakeAgent(() => this.onAgentDeselected(selected));
        _dialogue.ChooseGate(newCharacter);
    }

    private void onAgentDeselected(NodeButton deselected)
    {
        deselected.MakeDisabled(); //TODO: We disable the button before making it a candidate as a cheap hack to reset some values. Fix this shit, Brendan
        deselected.MakeCandidate(() => this.onAgentReselected(deselected));
        this.isEngaged = false;
    }

    private void onAgentReselected(NodeButton reSelected)
    {
        this.isEngaged = true;
        reSelected.MakeAgent(() => this.onAgentDeselected(reSelected));
    }

    public void closeAllNodes()
    {
        foreach (var kvp in this._nodes) {
            kvp.Value.MakeDisabled();
        }
        this.isEngaged = false;
    }
    
    public void closeAllNonActiveNodes()
    {
        foreach (var kvp in this._nodes) {
            //If there's no active character, or if the node isn't already the active character, disable the node
            if(this._dialogue.activeGuppy == Guppy.NULL || this._dialogue.activeGuppy == kvp.Value.identifier)
                kvp.Value.MakeDisabled();
        }
        this.isOpen = false;
        this.isEngaged = false;
    }

    // 
    public void CloseNode(Guppy chara)
    {
        if (CharacterIsGuppy(chara))
        {
            _nodes[chara].MakeDisabled();
        }
        else Debug.LogError("attempted to close node with unrecognized identifier [" + chara + "]");
    }

    // I should probably move these two functions to somewhere that makes more sense - Michel

    public bool CharacterIsGuppy(Guppy character)
    {
        return (character == Guppy.ARMSTRONG ||
            character == Guppy.JEMISON ||
            character == Guppy.COOPER);
    }

    public Guppy CharToCharacter(char c)
    {
        switch (c)
        {
            case 'j':
                return Guppy.JEMISON;
            case 'c':
                return Guppy.COOPER;
            case 'a':
                return Guppy.ARMSTRONG;
        }

        return Guppy.NULL;

    }

}