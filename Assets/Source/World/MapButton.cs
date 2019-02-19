using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapButton : MonoBehaviour {

    SpriteRenderer rend;
    Color colorStart = Color.white;
    Color colorHover = Color.red;
    bool mouseOver = false;

    public GameObject radarRing;
    Animator ringAnim;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.material.color = colorStart;
        ringAnim = radarRing.GetComponent<Animator>();
    }

    void Update()
    {
        if (mouseOver && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("_transition", LoadSceneMode.Single);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Radar")
        {
            //print("radar ping");
            ringAnim.SetTrigger("RadarCollide");
        }
    }

    void OnMouseEnter()
    {
        mouseOver = true;
        rend.material.SetColor("_Color", colorHover);
    }

    void OnMouseExit()
    {
        mouseOver = false;
        rend.material.SetColor("_Color", colorStart);
    }

}
