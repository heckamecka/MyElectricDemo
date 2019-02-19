using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "Text Effect Data", menuName = "MEH/Text Effect Data", order = 0)]
public class TextEffectData : ScriptableObject
{
    [Header("Wave")]
    public AnimationCurve waveCurve;
    public int waveFrequency;
    public float waveAmplitude;

    [Header("Shake")]
    public float defaultShakeStrength;

    [Header("Trail")]
    public AnimationCurve trailCurve;

}