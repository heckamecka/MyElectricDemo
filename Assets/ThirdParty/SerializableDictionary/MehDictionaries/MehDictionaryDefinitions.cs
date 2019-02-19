using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCLookupDictionary : SerializableDictionary<string, NPCData> { }

[System.Serializable]
public class ImageLookupDictionary : SerializableDictionary<string, Sprite> { }

[System.Serializable]
public class StringLookupDictionary : SerializableDictionary<string, string> { }

[System.Serializable]
public class TextSpeedLookupDictionary : SerializableDictionary<string, TextSpeedMode> { }

[System.Serializable]
public class AudioLookupDictionary : SerializableDictionary<string, AudioClip> { }

[System.Serializable]
public class HadfieldLookupDictionary : SerializableDictionary<string, HadfieldNewsData> { }
