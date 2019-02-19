using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MehDance : MonoBehaviour {

    public GameObject Manager;

    Animator anim;

	void Start () {

        anim = GetComponent<Animator>();
        anim.SetBool("jemDance", false);
        anim.SetBool("coopDance", false);
        anim.SetBool("strongDance", false);
       //anim.SetBool("idle", true);

    }

    void Update() {

        if (Manager.GetComponent<MehDialogueMachine>().gottaDefrost == false)
        {
            anim.SetBool("idleDance", false);

            if (Manager.GetComponent<MehDialogueMachine>().activeGuppy == Guppy.JEMISON)
            {
                anim.SetBool("jemDance", true);
                anim.SetBool("strongDance", false);
                anim.SetBool("coopDance", false);
            }
            else if (Manager.GetComponent<MehDialogueMachine>().activeGuppy != Guppy.JEMISON)
            {
                //anim.SetBool("jemDance", false);
            }

            if (Manager.GetComponent<MehDialogueMachine>().activeGuppy == Guppy.COOPER)
            {
                anim.SetBool("coopDance", true);
                anim.SetBool("jemDance", false);
                anim.SetBool("strongDance", false);
            }
            else if (Manager.GetComponent<MehDialogueMachine>().activeGuppy != Guppy.COOPER)
            {
                //anim.SetBool("coopDance", false);
            }

            if (Manager.GetComponent<MehDialogueMachine>().activeGuppy == Guppy.ARMSTRONG)
            {
                anim.SetBool("strongDance", true);
                anim.SetBool("jemDance", false);
                anim.SetBool("coopDance", false);
            }
            else if (Manager.GetComponent<MehDialogueMachine>().activeGuppy != Guppy.ARMSTRONG)
            {
                //anim.SetBool("strongDance", false);
            }
        } else if (Manager.GetComponent<MehDialogueMachine>().gottaDefrost == true)
        {
            anim.SetBool("idleDance", true);
            anim.SetBool("strongDance", false);
            anim.SetBool("jemDance", false);
            anim.SetBool("coopDance", false);
        }
    } 
 }

