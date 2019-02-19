using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FavorVials : MonoBehaviour {

    [SerializeField] public MehVariableStorage storage;

    public GameObject J_vial;
    public GameObject C_vial;
    public GameObject A_vial;

    private float gooSupplement = 0.25f;
	
	void Update () {

        J_vial.GetComponent<Image>().fillAmount = storage.J_percent + gooSupplement;
        C_vial.GetComponent<Image>().fillAmount = storage.C_percent + gooSupplement;
        A_vial.GetComponent<Image>().fillAmount = storage.A_percent + gooSupplement;
    }
}
