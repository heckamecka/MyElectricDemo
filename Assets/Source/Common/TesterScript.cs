using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


// Throwaway script for whaever i'm testing at the moment - Michel
public class TesterScript : MonoBehaviour {

    [SerializeField] AudioClip clip1;
    [SerializeField] AudioClip clip2;

    [SerializeField] AudioSource sauce;

    private void Start()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DoTheTest());
        }
    }

    IEnumerator DoTheTest()
    {
        sauce.PlayOneShot(clip1);
        yield return new WaitForSeconds(1f);
        sauce.PlayOneShot(clip2);
    }

}
