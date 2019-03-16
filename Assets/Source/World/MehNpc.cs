using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MehNpc : MonoBehaviour
{
    [Header("Highlight")]
    [SerializeField]
    private float hightlightTransitionDuration = 0.3f;
    [SerializeField] private Color desaturatedColor;
    [SerializeField] private float unhighlightedScale = 0.7f;
    [SerializeField] private float unhighlightedTranslation = 70f;

    [Header("Components")]
    [SerializeField] protected MehResourceLookups resources;
    [SerializeField] protected Image characterImage;
    [SerializeField] protected CanvasGroup characterGroup;
    [SerializeField] Image faceSprite;

    private string currentNPCLookup;
    private NPCData currentNPC { get { return resources.npcs[currentNPCLookup]; } }
    // TODO: possibly want to make it so that you can change the defauly scaling size in yarn
    // maybe we want the guppy to look puny or massive next to some special sized npc - Michel
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Vector3 unhighlightedPosition;


    public void Start()
    {
        // setting the original transform
        originalScale = transform.localScale;
        originalPosition = characterImage.transform.localPosition;
        unhighlightedPosition = originalPosition+ new Vector3(unhighlightedTranslation * transform.localScale.x, 0f);

        // setting initial to unhighlighted
        transform.localScale = originalScale * unhighlightedScale;
        characterImage.color = desaturatedColor;
        characterImage.transform.localPosition = unhighlightedPosition;

    }

    public void SetCharacter(string lookup)
    {
        if (lookup == "none" || resources.npcs[lookup] == null)
        {
            this.characterGroup.alpha = 0;
        }
        else
        {
            // set the new character look up
            currentNPCLookup = lookup;

            if (currentNPC.graphic != null)
            {
                this.characterGroup.alpha = 1;
                this.characterImage.sprite = currentNPC.graphic;
            }
            else this.characterGroup.alpha = 0;
            
            if (faceSprite != null)
            {
                faceSprite.sprite = currentNPC.defaultFace;

                // hide or show face, depending on whether or not the sprite is null
                if (faceSprite.sprite == null) faceSprite.color = new Color(0f, 0f, 0f, 0f);
                else faceSprite.color = new Color(1f, 1f, 1f, 1f);

                faceSprite.rectTransform.anchoredPosition = currentNPC.faceOffset;
            }
        }
    }

    public void SetFace(string lookup)
    {
        if (faceSprite != null)
        {
            faceSprite.sprite = currentNPC.getFace(lookup);

            // hide or show face, depending on whether or not the sprite is null
            if(faceSprite.sprite == null) faceSprite.color = new Color(0f, 0f, 0f, 0f);
            else faceSprite.color = new Color(1f, 1f, 1f, 1f);
        }
    }   

    public void SetMaterial(Material mat)
    {
        characterImage.material = mat;
    }

    public virtual void HighlightCharacter()
    {
        StartCoroutine(CO_ShiftSaturation(Color.white, hightlightTransitionDuration));
        StartCoroutine(CO_ShiftScale(1.0f, hightlightTransitionDuration));
        StartCoroutine(CO_ShiftPosition(originalPosition, hightlightTransitionDuration));
    }

    public virtual void UnHighlightCharacter()
    {
        StartCoroutine(CO_ShiftSaturation(desaturatedColor,  hightlightTransitionDuration));
        StartCoroutine(CO_ShiftScale(unhighlightedScale,  hightlightTransitionDuration));
        StartCoroutine(CO_ShiftPosition(unhighlightedPosition, hightlightTransitionDuration));
    }

    protected virtual IEnumerator CO_ShiftPosition(Vector3 target, float duration)
    {
        Vector3 start = characterImage.transform.localPosition;
        float timer = 0.0f;
        float timelimit = duration;
        float f = 0.0f;
        while (timer < timelimit)
        {
            //Debug.Log(gameObject.name + " is moving " + obj.name);

            timer += Time.deltaTime;
            f = timer / timelimit;
            characterImage.transform.localPosition = Vector3.Lerp(start, target, f);
            yield return null;
        }
    }

    protected IEnumerator CO_ShiftSaturation(Color target, float duration)
    {
        Color start = characterImage.color;
        float timer = 0.0f;
        float timelimit = duration;
        float f = 0.0f;
        while (timer < timelimit)
        {
            //Debug.Log(gameObject.name + " is moving " + obj.name);

            timer += Time.deltaTime;
            f = timer / timelimit;
            characterImage.color = Color.Lerp(start, target, f);
            yield return null;
        }
    }

    IEnumerator CO_ShiftScale(float target, float duration)
    {
        // use the x scale as a baseline for determining current scale
        float start = transform.localScale.x / originalScale.x;

        float timer = 0.0f;
        float timelimit = duration;
        float f = 0.0f;
        while (timer < timelimit)
        {
            //Debug.Log(gameObject.name + " is moving " + obj.name);

            timer += Time.deltaTime;
            f = timer / timelimit;
            float scale = Mathf.Lerp(start, target, f);
            transform.localScale = originalScale * scale;
            yield return null;
        }
    }

    public void Shake(string magnitude = "10")
    {
        Vector3 origin = this.transform.position;
        float magnitudeFl = float.Parse(magnitude);
        StartCoroutine(CO_Shaking(origin, magnitudeFl));
    }

    IEnumerator CO_Shaking(Vector3 origin, float magnitude)
    {
        for (float f = 1.0f; f >= 0; f -= 0.1f)
        {
            float uiScale = Utility.FindGreatestParent(transform).localScale.x; // Scale it according to the canvas scaler in the greatest parent - Michel
            transform.position = origin + (Vector3)Random.insideUnitCircle * magnitude * uiScale;
            yield return new WaitForSeconds(0.02f);
        }
        transform.position = origin;
    }

    public void Bump(string magnitude = "10")
    {
        Vector3 origin = this.transform.position;
        float magnitudeFl = float.Parse(magnitude);
        StartCoroutine(CO_Bumping(origin, magnitudeFl));
    }

    IEnumerator CO_Bumping(Vector3 origin, float magnitude)
    {
        float uiScale = Utility.FindGreatestParent(transform).localScale.x; // Scale it according to the canvas scaler in the greatest parent - Michel
        transform.position = origin + new Vector3(0, magnitude*uiScale);
        yield return new WaitForSeconds(0.04f);
        transform.position = origin;
    }
}
