using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I think maybe i should centralize some of the functionality in here into a singleton, because not every scene is gonna use look ups - Michel
public class MehMusicPlayer : MonoBehaviour
{
    [SerializeField] private MehResourceLookups resources;
    [SerializeField] private AudioSource audioPlayer;

    float volumeMod { get { return MehGameManager.instance.persistent.musicVolume; } }

    private float cachedVolume;

    private void Start()
    {
        cachedVolume = audioPlayer.volume;
    }

    private void Update()
    {
        audioPlayer.volume = cachedVolume * volumeMod;
    }

    public void PlayMusic(string lookup, string time)
    {
        if (lookup != "none")
        {
            StartCoroutine(CO_Transition(lookup, float.Parse(time) / 2.0f));
        }
        else StartCoroutine(CO_StopMusic(float.Parse(time) / 2.0f));
    }

    IEnumerator CO_StopMusic(float halftime)
    {
        for (float vol = halftime; vol > 0; vol -= Time.deltaTime)
        {
            cachedVolume = vol / halftime;
            yield return null;
        }
        cachedVolume = 0.0f;
    }

    IEnumerator CO_Transition(string lookup, float halftime)
    {
        for (float vol = halftime; vol > 0; vol -= Time.deltaTime)
        {
            cachedVolume = vol / halftime;
            yield return null;
        }

        this.audioPlayer.clip = resources.music[lookup];
        this.audioPlayer.Play();

        for (float vol = 0; vol < halftime; vol += Time.deltaTime)
        {
            cachedVolume = vol / halftime;
            yield return null;
        }
    }
}
