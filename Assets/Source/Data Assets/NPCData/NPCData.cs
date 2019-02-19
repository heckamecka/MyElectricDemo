using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC Data", menuName = "MEH/NPC Data", order = 0)]
public class NPCData : ScriptableObject {

    public string lookup;
    public string characterName;
    public Sprite graphic;
    public Vector2 faceOffset;
    public Sprite defaultFace;
    [SerializeField] ImageLookupDictionary faces;
    // internal accessor that does some checking - Michel
    public Sprite getFace(string key)
    {
        if (key == "") return defaultFace;
        if (!faces.ContainsKey(key))
        {
            Debug.LogWarning(lookup + " does not contain a face for the key [" + key + "].");
            return defaultFace;
        }
        else return faces[key];
    }

}
