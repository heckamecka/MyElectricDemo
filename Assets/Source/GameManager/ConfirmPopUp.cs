using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConfirmPopUp : MonoBehaviour {
    
    [SerializeField] CanvasGroup _canvasGrp;
    [SerializeField] TextMeshProUGUI promptText;
    Action _onConfirm;

    private void Start()
    {
        HidePopUp();
    }

    public void OnConfirm()
    {
        _onConfirm();
        HidePopUp();
    }

    public void OnCancel()
    {
        HidePopUp();
    }

    public void ShowPopUp(Action confirmInject, string promptString)
    {
        _onConfirm = confirmInject;
        promptText.text = promptString;

        _canvasGrp.interactable = true;
        _canvasGrp.alpha = 1;
        _canvasGrp.blocksRaycasts = true;
    }

    void HidePopUp()
    {
        _canvasGrp.interactable = false;
        _canvasGrp.alpha = 0;
        _canvasGrp.blocksRaycasts = false;
    }

}
