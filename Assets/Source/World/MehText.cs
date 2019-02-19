using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MehText : MonoBehaviour {
    [SerializeField] private MehDialogueMachine dialogue;
    [SerializeField] private TextMeshProUGUI _tmpText;
    [SerializeField] private TextEffect _textEffect;

    [SerializeField] int debugVisibleCharacters;
    public int maxVisibleCharacters
    {
        get { return _tmpText.maxVisibleCharacters; }
        set { _tmpText.maxVisibleCharacters = value; }
    }

    public Color defaultColor;
    public Color jemisonColor;
    public Color cooperColor;
    public Color armstrongColor;

    private void Update()
    {
        //maxVisibleCharacters = debugVisibleCharacters;
        //Debug.Log(maxVisibleCharacters);
    }

    public IEnumerator SetText(string str)
    {
        Color inputColor = defaultColor;

        switch (dialogue.activeGuppy)
        {
            case Guppy.JEMISON:
                inputColor = jemisonColor;
                break;
            case Guppy.COOPER:
                inputColor = cooperColor;
                break;
            case Guppy.ARMSTRONG:
                inputColor = armstrongColor;
                break;
            case Guppy.NULL:
                inputColor = defaultColor;
                break;
        }

        yield return _textEffect.SetText(str,inputColor);
    }

    public void AdvanceDialogue()
    {
        this.dialogue.AdvanceDialogue();
    }

}
