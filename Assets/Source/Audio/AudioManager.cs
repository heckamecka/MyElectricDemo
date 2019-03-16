using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField] GameObject oneshotPrefab;
    [SerializeField] AudioSource playerOne;

    AudioSource currentPlayer;

    Coroutine currentCO;

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
        if (currentCO != null) StopCoroutine(currentCO);
        StartCoroutine(CO_Transition(clip, time));
    }

    public void FadeOutMusic(float time)
    {
        if (currentCO != null) StopCoroutine(currentCO);
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

    IEnumerator CO_Transition(AudioClip clip, float time)
    {
        float halftime = time / 2f;
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
