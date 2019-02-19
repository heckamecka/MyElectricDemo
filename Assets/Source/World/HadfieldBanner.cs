using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HadfieldBanner : MonoBehaviour {

    [SerializeField] MehResourceLookups resource;
    [SerializeField] float _textScrollSpeed = 1.0f;
    [SerializeField] Animator anim; 
    
    [Header("Text Components")] 
    [SerializeField] TextMeshProUGUI _interviewee;
    [SerializeField] TextMeshProUGUI _intervieweeTitle;
    [SerializeField] TextMeshProUGUI _topic;
    [SerializeField] TextMeshProUGUI _scrollingText;


    private bool activated;

    // for scrolling - Michel
    float _scrollWidth;
    Vector3 startPos;
    TextMeshProUGUI _scrollingClone;

    // temporary data storage - Michel
    string _nameStore;
    string _titleStore;
    string _topicStore;

    private void Start()
    {
        activated = false;

        startPos = _scrollingText.rectTransform.anchoredPosition;

        _scrollWidth = _scrollingText.preferredWidth + 3; // adds some space at the end - Michel

        // create scrolling clone - Michel
        _scrollingClone = Instantiate(_scrollingText) as TextMeshProUGUI;
        RectTransform cloneTransform = _scrollingClone.GetComponent<RectTransform>() ;
        cloneTransform.SetParent(_scrollingText.transform);
        cloneTransform.anchoredPosition = new Vector3(startPos.x + _scrollWidth, startPos.y, startPos.z);
        cloneTransform.localScale = Vector3.one;

        // TODO: add checking for if text is too short, clone more copies based on screen width.
    }
    
    void SetData(HadfieldNewsData data)
    {
        // set data - Michel
        _interviewee.text = data._interviewee;
        _intervieweeTitle.text = data._intervieweeTitle;
        _topic.text = data._topic;
        _scrollingText.text = data._scrollingText;
        _scrollingClone.text = data._scrollingText;

        // set positions of the scrolling texts - Michel
        _scrollWidth = _scrollingText.preferredWidth + 3;
        _scrollingText.rectTransform.anchoredPosition = new Vector3(startPos.x, startPos.y, startPos.z);
        _scrollingClone.rectTransform.anchoredPosition = new Vector3(startPos.x + _scrollWidth, startPos.y, startPos.z);
    }

    public void ActivateBanner(string dataKey)
    {
        if (activated) return; // if it's already activated, stop
        activated = true;
        StopCoroutine(ScrollText());
        SetData(resource.hadfield[dataKey]);
        anim.SetBool("activated", true);
        StartCoroutine(ScrollText());
    }

    public void DeactivateBanner()
    {
        anim.SetBool("activated", false);
        activated = false;
    }

    IEnumerator ScrollText()
    {
        float scrollPos = 0f;
        while (true)
        {
            _scrollingText.rectTransform.anchoredPosition = new Vector3(-scrollPos % _scrollWidth + startPos.x, startPos.y, startPos.z);
            scrollPos += _textScrollSpeed * Time.deltaTime;
            yield return null;
        }
    }
    
    // Swapping text in the banner
    #region text swapping in banner
    public void ChangeName(string newName)
    {
        if (!activated) return;
        _nameStore = newName;
        anim.SetTrigger("nameSwap");
    }
    public void ChangeTitle(string newTitle)
    {
        if (!activated) return;
        _titleStore = newTitle;
        anim.SetTrigger("titleSwap");
    }
    public void ChangeTopic(string newTopic)
    {
        if (!activated) return;
        _topicStore = newTopic;
        anim.SetTrigger("topicSwap");
    }

    void SwapName()
    {
        _interviewee.text = _nameStore;
    }
    void SwapTitle()
    {
        _intervieweeTitle.text = _titleStore;
    }
    void SwapTopic()
    {
        _topic.text = _topicStore;
    }
    #endregion

}
