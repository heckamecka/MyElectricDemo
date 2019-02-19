using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringLookupDictionary))]
public class StringLookupDictionaryDrawer : SerializableDictionaryDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(ImageLookupDictionary))]
public class ImageLookupDictionaryDrawer : SerializableDictionaryDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(NPCLookupDictionary))]
public class NPCLookupDictionaryDrawer : SerializableDictionaryDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(TextSpeedLookupDictionary))]
public class TextSpeedLookupDictionaryDrawer : SerializableDictionaryDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}

[CustomPropertyDrawer(typeof(AudioLookupDictionary))]
public class AudioLookupDictionaryDrawer : SerializableDictionaryDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}


[CustomPropertyDrawer(typeof(HadfieldLookupDictionary))]
public class HadfieldDictionaryDrawer : SerializableDictionaryDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
    }
}