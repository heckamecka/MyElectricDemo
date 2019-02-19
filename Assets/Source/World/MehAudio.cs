using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MehAudio : MonoBehaviour
{
    [SerializeField] private MehResourceLookups resources;

    public void PlayStinger(string lookup)
    {
        if (lookup != "none")
        {
            MehGameManager.instance.audioMan.PlayOneShot(resources.stingers[lookup]);
        }
    }

    public void PlayMusic(string lookup, float time)
    {
        if (lookup != "none")
        {
            MehGameManager.instance.audioMan.PlayMusic(resources.music[lookup], time);
        }
    }
}
