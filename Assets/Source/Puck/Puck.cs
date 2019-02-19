using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puck : MonoBehaviour {

    [SerializeField] private Camera mCamera;
    [SerializeField] private Canvas mCanvas;
    [SerializeField] private Transform puckRoot;      //The actual location of the puck
    [SerializeField] private Transform neutralAnchor; //Point to which the puck snaps when your finger isn't engaged
    [SerializeField] private Vector2 debugOffset;     //My screen-to-ui conversion math is a little off for some reason. Sorry I'm bad

    private void OnDrawGizmos()
    {
        if(mCamera != null && mCanvas != null) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(this.transform.position, mCamera.UnitAsPercentageOfScreenWidth(0.05f));
        }
    }

	void Update () {
        //Follow the finger on canvas:
        // TODO: make a better input system script
		if(Input.GetKey(KeyCode.Mouse0)) {
            //Beep beep! shitty code alert:

            Vector2 mousePos = Input.mousePosition;
//#if !(UNITY_IOS||UNITY_ANDROID)
//            mousePos = AspectUtility.mousePosition; // use aspect utility mouse pos if on pc, mac or linux
//#endif


            Vector3 location = mCamera.ScreenToWorldPoint(mousePos);
            location.z = 0; // zero out z axis
            this.puckRoot.transform.position = location;
            puckRoot.transform.position += (Vector3)debugOffset;
        }
        else {
            this.puckRoot.transform.position = this.neutralAnchor.transform.position;
        }
	}
}
