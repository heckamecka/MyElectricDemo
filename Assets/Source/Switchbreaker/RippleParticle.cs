using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RippleParticle : MonoBehaviour {

    Material mat;
    float _time = 0;
    bool activated = false;

	// Use this for initialization
	void Start () {
        mat = GetComponent<Image>().material;
        mat.SetFloat("_CTime", _time);
    }
	
	// Update is called once per frame
	void Update () {
        if (activated)
        {
            _time += Time.deltaTime;
            mat.SetFloat("_CTime", _time);
            if (_time > 20f) activated = false;
        }
	}

    public void ActivateRipple()
    {
        _time = 0f;
        activated = true;
    }

}
