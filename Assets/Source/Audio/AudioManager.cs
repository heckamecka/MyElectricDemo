using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] GameObject oneshotPrefab;
    [SerializeField] AudioSource playerOne;

    AudioSource currentPlayer;

    float cachedMusicVol = 1.0f;
    
	void Start () {
        currentPlayer = playerOne;
	}
	
	void Update () {
        currentPlayer.volume = cachedMusicVol * MehGameManager.instance.persistent.musicVolume;
    }

    public void PlayOneShot(AudioClip clip)
    {
        // instantiate game object
        AudioOneShot aos = Instantiate(oneshotPrefab).GetComponent<AudioOneShot>();
        aos.PlayClip(clip);
    }

    public void PlayMusic(AudioClip clip, float time)
    {
        StartCoroutine(CO_Transition(clip, time / 2.0f));
    }

    void FadeOutMusic(float time)
    {
        StartCoroutine(CO_StopMusic(time));
    }

    IEnumerator CO_StopMusic(float time)
    {
        for (float elapsed = time; elapsed > 0; elapsed -= Time.deltaTime)
        {
            cachedMusicVol = elapsed / time;
            yield return null;
        }
        cachedMusicVol = 0.0f;
    }

    IEnumerator CO_Transition(AudioClip clip, float halftime)
    {
        for (float vol = halftime; vol > 0; vol -= Time.deltaTime)
        {
            cachedMusicVol = vol / halftime;
            yield return null;
        }

        currentPlayer.clip = clip;
        currentPlayer.Play();

        for (float vol = 0; vol < halftime; vol += Time.deltaTime)
        {
            cachedMusicVol = vol / halftime;
            yield return null;
        }
    }

}
