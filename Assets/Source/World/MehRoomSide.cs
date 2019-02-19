using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MehRoomSide : MonoBehaviour {

    public MehRoomManager rm;
    [Header("NPC Image")]
    [SerializeField] private MehNpc active;
    [SerializeField] private MehNpc reserve;
    public MehNpc GetActiveNPC() { return active; }

    [SerializeField] private Transform activePos;
    [SerializeField] private Transform reservePos;

    [Header("NPC Name tag")]
    [SerializeField] private MehNamePlate activeName;
    [SerializeField] private MehNamePlate reserveName;

    [SerializeField] private Transform activeNamePos;
    [SerializeField] private Transform reserveNamePos;
    public string GetActiveNameString() { return activeName.displayName; }

    public bool speakerActive = false;

    public string _slot;

    private void Start()
    {
        // Initialize variables - Michel
        active.SetCharacter("none");
        reserve.SetCharacter("none");

        activeName.transform.position = reserveNamePos.position;
        reserveName.transform.position = reserveNamePos.position;

        active.transform.position = reservePos.position;
        reserve.transform.position = reservePos.position;

    }

    public void SetCharacter(string lookup)
    {
        active.SetCharacter(lookup);
    }

    /// <summary>
    /// Given a lookup, determines whether to just switch npc graphics, pop a character off screen, or swap it out with a current NPC
    /// </summary>
    /// <param name="lookup"></param>
    public void SetNPC(string lookup)
    {
        // if there's already another character in the slot, pop it out, set variables to null - Michel
        if (_slot != "")
        {
            //Debug.Log("popping " + _Slot);
            // swap the active and reserve slots - Michel
            MehNpc store = reserve;
            reserve = active;
            active = store;
            // pop old
            Pop(reserve);
        }

        // set the character - Michel
        _slot = lookup;
        //Debug.Log("Set " + _Slot);
        active.SetCharacter(lookup);

        // push the active one out - Michel
        Push(active);
    }

    #region Push Pop Character Image
    // TODO: some checking for popping and pushing characters to make sure coroutines don't clash - Michel
    /// <summary>
    /// Animate the given character exiting the screen
    /// </summary>
    public void Pop(Transform trans)
    {
        _slot = "";
        StartCoroutine(CO_Move(activePos, reservePos, trans));
    }

    /// <summary>
    /// Animate the given character entering
    /// </summary>
    public void Push(Transform trans)
    {
        StartCoroutine(CO_Move(reservePos, activePos, trans));
        trans.SetAsLastSibling();

    }

    /// <summary>
    /// Pop out old npc, set character variables to null
    /// </summary>
    void Pop(MehNpc npc)
    {
        Pop(npc.transform);
    }

    public void Push(MehNpc npc)
    {
        Push(npc.transform);
    }

    /// <summary>
    /// Pop out the current active character 
    /// </summary>
    // public facing way of popping
    public void PopActive()
    {
        Pop(active);
    }

    #endregion

    #region Push Pop Name Tag
    // Checking and determing whether to pop or push is performed in Room Manager - Michel

    public void DeactivateNameTag()
    {
        if (!speakerActive) return; // fail safe for it there are no nametags active - Michel

        // Swap active and reserve name tags - Michel
        MehNamePlate store = activeName;
        activeName = reserveName;
        reserveName = store;

        speakerActive = false;
        StartCoroutine(CO_Move(activeNamePos, reserveNamePos, reserveName.transform));
        active.UnHighlightCharacter();
    }

    public void ActivateNameTag(string name)
    {
        if (speakerActive) return; // fail safe for if a name tag is already active
        
        speakerActive = true;
        activeName.displayName = name;
        StartCoroutine(CO_Move(reserveNamePos, activeNamePos, activeName.transform));
        transform.SetAsLastSibling();
    }
    
    public void SetSpeakerHighlighted(bool highlighted)
    {
        if (_slot == "") return; // don't bother doing anything if there isn't anything on this side of the screen - Michel

        if (highlighted) active.HighlightCharacter();
        else active.UnHighlightCharacter();
    }

    #endregion

    IEnumerator CO_Move(Transform start, Transform end, Transform obj, float magnitude = 1.0f)
    {
        float timer = 0.0f;
        float timelimit = rm.standardMoveDuration / magnitude;
        float f = 0.0f;
        while (timer < timelimit)
        {
            //Debug.Log(gameObject.name + " is moving " + obj.name);

            timer += Time.deltaTime;
            f = timer / timelimit;
            f = rm.moveCurve.Evaluate(f);
            obj.position = Vector3.Lerp(start.position, end.position, f);
            yield return null;
        }
    }


}
