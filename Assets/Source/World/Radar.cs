using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour {

    public float radarSpeed;
    SpriteRenderer rend;

    bool fading = false; 

    void Start()
    {
        rend = gameObject.GetComponent<SpriteRenderer>(); 
        if (gameObject.name.Contains("Clone") == true)
        {
            //StartCoroutine(Fade());
        }
    }
	
	void Update () {

        if (gameObject.name.Contains("Clone") == false)
        {
            //GameObject ghost = (GameObject)Instantiate(this.gameObject, transform.position, transform.rotation);
            //Destroy(ghost.GetComponent<BoxCollider2D>());
        }

        if (!fading)
        {
            transform.Rotate(0, 0, Time.deltaTime * radarSpeed);
        }
    }

    IEnumerator Fade()
    {
        fading = true; 
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            yield return new WaitForSeconds(0.0005f);
        }
        Destroy(gameObject);
    }
}
