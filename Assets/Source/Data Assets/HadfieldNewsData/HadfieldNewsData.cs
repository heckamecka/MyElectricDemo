using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hadfiel dNews Data", menuName = "MEH/Hadfield News Data", order = 0)]
public class HadfieldNewsData : ScriptableObject {

    public string _interviewee;
    public string _intervieweeTitle;
    public string _topic;
    [TextArea(3, 10)] public string _scrollingText;
}
