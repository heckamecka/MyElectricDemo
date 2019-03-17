using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour {
    
    [SerializeField] CanvasGroup canvas;
    Coroutine _fadeCoroutine;
        
    #region Deprecated Static Instancing
    //static ScreenFade _instance;
    //static public ScreenFade instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            Debug.LogError("No instance of screen fade was found in this scene. If this caused a null reference exception please remember to check if instance is null before using it");
    //            return null;
    //        }
    //        else return _instance;
    //    }
    //}

    //private void Awake()
    //{
    //    // always set the instance of screen fade to the one most recently loaded in - Michel
    //    _instance = this;
    //}

    #endregion

    void Awake () {
        // set up fade image to transparent as default - michel
        canvas.alpha = 0.0f;
        // set up coroutine - michel
    }

    /// <summary>
    ///  Fade in from black - Michel
    /// </summary>
    /// <param name="fadeTime"> the time it takes to go from completely black to completely transparent </param>
    public void FadeFromBlack(float fadeTime)
    {
        //Debug.Log("Fade in called");
        if(_fadeCoroutine!=null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(CO_FadeFromBlackHelper( fadeTime));
    }

    /// <summary>
    /// Fade to black, sorting layer 3(i.e. covers everything except for credits) - Michel
    /// </summary>
    /// <param name="fadeTime"> the time it takes to go from completely transparent to completely black </param>
    public void FadeToBlack(float fadeTime)
    {
        //Debug.Log("Fade out called");
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(CO_FadeToBlackHelper(fadeTime));
    }

    /// <summary>
    /// Fade from black, setting the screen to pitch black before hand
    /// </summary>
    /// <param name="fadeTime"> the time it takes to go from completely black to completely transparent </param>
    public void FadeFromBlackActually(float fadeTime)
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        canvas.alpha = 1.0f;
        _fadeCoroutine = StartCoroutine(CO_FadeFromBlackHelper(fadeTime));
    }

    public IEnumerator CO_FadeToBlack(float fadeTime)
    {
        yield return CO_FadeToBlackHelper(fadeTime);
    }

    public IEnumerator CO_FadeFromBlack(float fadeTime)
    {
        yield return CO_FadeFromBlackHelper(fadeTime);
    }

    IEnumerator CO_FadeFromBlackHelper(float fadeTime)
    {
        // set up - Michel
        //Debug.Log(canvas.alpha);
        float percentage = 1-canvas.alpha;
        float elapsed = percentage * fadeTime;
        // fade loop - Michel
        while (canvas.alpha > 0.01f)
        {
            //Debug.Log("fade loop running");
            elapsed += Time.deltaTime;
            percentage = elapsed / fadeTime;
            canvas.alpha = Mathf.Lerp(1.0f, 0.0f, percentage);
            //Debug.Log(canvas.alpha);
            yield return null;
        }
        canvas.blocksRaycasts = false;
    }

    IEnumerator CO_FadeToBlackHelper(float p_fadeTime)
    {
        canvas.blocksRaycasts = true;
        // set up - Michel
        float percentage = canvas.alpha;
        float elapsed = percentage * p_fadeTime;

        // fade loop - Michel
        while (canvas.alpha < 0.99f)
        {
            //Debug.Log("fade loop running");
            elapsed += Time.deltaTime;
            percentage = elapsed / p_fadeTime;
            canvas.alpha = Mathf.Lerp(0.0f, 1.0f, Mathf.Pow(percentage, 0.5f));
            yield return null;
        }
        canvas.alpha = 1.0f;
    }
}
