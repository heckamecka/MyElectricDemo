using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Text Speed", menuName = "MEH/Text Speed Data", order = 0)]
public class TextSpeedMode : ScriptableObject {
    [Header("Default:")]
    [SerializeField] private float _letterDelta        = 0.05f;
    [Header("Exceptions:")]
    [SerializeField] private float _whitespaceDelta    = 0.025f;
    [SerializeField] private float _commaDelta         = 0.15f;
    [SerializeField] private float _colonDelta         = 0.2f;
    [SerializeField] private float _sentenceEndDelta   = 0.3f;

    public void initialize(
        float letterDelta,
        float sentenceEndDelta, 
        float commaDelta
    ){
        this._letterDelta = letterDelta;
        this._sentenceEndDelta = sentenceEndDelta;
        this._commaDelta = commaDelta;
    }
    
    public float getCharDelta(char c) {
        if (System.Char.IsWhiteSpace(c))
            return this._whitespaceDelta;
        else if (",".Contains(c.ToString()))
            return this._commaDelta;
        else if (":;".Contains(c.ToString()))
            return this._colonDelta;
        else if (".!?".Contains(c.ToString()))
            return this._sentenceEndDelta;
        else
            return this._letterDelta;
    }
}
