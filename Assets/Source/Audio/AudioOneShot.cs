using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOneShot : MonoBehaviour {

    [SerializeField] AudioSource audSource;
    float cachedVolume = 1.0f;

    private void Update()
    {
        if (!audSource.isPlaying) // destroy self once the music stops playing - Michel
        {
            Destroy(gameObject); 
        }

        // modulate volume based on settings volume - Michel
        audSource.volume = cachedVolume * MehGameManager.instance.persistent.sfxVolume;
        

    }

    public void PlayClip(AudioClip clip)
    {
        audSource.clip = clip;
        audSource.Play();
    }

}
