using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class typeWriter : MonoBehaviour {

    public float scrollDelay = 0.1f;
    //private float nextOffset = 0f;
    //public float xOffset;
    //public float yOffset; 

    //public Text loneLetterPrefab;
    //public Canvas renderCanvas;

    public bool runText = false;
    public float pauseAfter = 1f; 
    public string fullText;
    private string currentText = "";

    private AudioSource aud; 

    void Start () {
        //StartCoroutine(ShowText());
        aud = GetComponent<AudioSource>();
}

void Update()
    {
        if (runText)
        {
            StartCoroutine(ShowText());
            runText = false;
        }
    }

    public IEnumerator ShowText()
    {
        for(int i = 0; i < fullText.Length+1; i++)
        {
            currentText = fullText.Substring(0, i);
            //aud.Play();
            this.GetComponent<Text>().text = currentText;
            yield return new WaitForSeconds(scrollDelay);
        }

        //wigglyText option
        //foreach (char letter in fullText.ToCharArray())
        //{

        //    nextOffset = nextOffset + 0.05f;
        //    xOffset = xOffset - 25f;
        //    Text tempLetter = Instantiate(loneLetterPrefab, new Vector2(0, 0), transform.rotation) as Text;
        //    tempLetter.transform.SetParent(renderCanvas.transform, false);
        //    tempLetter.transform.localPosition = new Vector2(-xOffset, yOffset);
        //    tempLetter.GetComponent<Text>().text = "" + letter;
        //    tempLetter.GetComponent<waveEffect>().offset = nextOffset;

        //    yield return new WaitForSeconds(scrollDelay);
        //}

    }
}
