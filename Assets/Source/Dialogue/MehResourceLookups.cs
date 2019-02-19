using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MehResourceLookups : MonoBehaviour
{
    [SerializeField] public NPCLookupDictionary npcs;
    [SerializeField] public ImageLookupDictionary       guppies;
    [SerializeField] public ImageLookupDictionary       faces; 
    [SerializeField] public ImageLookupDictionary       backgrounds;
    [SerializeField] public TextSpeedLookupDictionary   textSpeeds;
    [SerializeField] public AudioLookupDictionary       stingers;
    [SerializeField] public AudioLookupDictionary       music;
    [SerializeField] public HadfieldLookupDictionary    hadfield;
}