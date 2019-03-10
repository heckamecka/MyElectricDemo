using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Label for when player hovers over/selects the location
public class LocationFile : MonoBehaviour {

    [SerializeField] Animator anim;
    [SerializeField] TextMeshProUGUI _caseNumber;
    [SerializeField] TextMeshProUGUI _sceneDescription;
    [SerializeField] Image _startSceneButton;
    
    [HideInInspector] public int _index;
    public LocationFileManager _fileManager { private get; set; }

    private void Start()
    {
        // randomly generate a case number
        string caseNum = "Case #00";
        for(int i = 0; i < 2; ++i)
        {
            caseNum += Random.Range(0, 9).ToString();
        }
        _caseNumber.SetText(caseNum);
    }

    public void SetFileShown(bool show)
    {
        anim.SetBool("show", show);
        _startSceneButton.raycastTarget = show;
    }

    public void SetText(string sceneName, string sceneDescription)
    {
        _sceneDescription.SetText(sceneName +"\n\n"+ sceneDescription);
    }

    // Propagate the start scene command to mapNode
    public void StartScene()
    {
        _fileManager.StartScene(_index);
    }

}
