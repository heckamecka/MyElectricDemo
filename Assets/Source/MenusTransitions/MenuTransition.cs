using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTransition : MonoBehaviour {

    [SerializeField] SceneData _introScene;

    public void MenuStart()
    {
        // if the intro is complete, go to overworld
        if (MehGameManager.instance.persistent.GetIntroComplete())
        {
            MehGameManager.instance.LoadOverworld();
        }
        else // else go to the intro
        {
            MehGameManager.instance.LoadShantyScene(_introScene);
        }
    }

    public void OverworldStart()
    {
        MehGameManager.instance.LoadOverworld();
    }
}
