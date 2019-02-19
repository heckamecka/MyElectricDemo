using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MehNamePlate : MonoBehaviour {

    [SerializeField] private MehResourceLookups _resource;
    [SerializeField] private Text _nameText;
    [SerializeField] private Image _nameBackground;

    public string displayName { get { return _nameText.text;  } set{ _nameText.text = value; } }

    public void SetName(string name)
    {
        _nameText.text = name;
        // change background based on name
    }


}
