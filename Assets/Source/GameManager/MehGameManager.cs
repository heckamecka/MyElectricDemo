using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MehGameManager : MonoBehaviour {


    static float globalTimeScale = 1.0f;

    [SerializeField] private Canvas _canvas;
    [SerializeField] private ScreenFade fade;
    [SerializeField] private AudioClip _overworldMusic;
    public AudioManager audioMan;

    private SaveLoad saveLoad;
    public PersistentData persistent { get; private set; }

    private static MehGameManager privateInstance;
    /// <summary>
    /// Returns the game manager singleton, loads a new instance if one doesn't exist already - Michel
    /// </summary>
    static public MehGameManager instance {
        get
        {
            // Load new game manager object if one doesn't already exist
            if (privateInstance == null)
            {
                // Using Resources.load, because I may want to store inspector editable 
                // data in the game manager or attach other scripts, and it's cleaner to
                // do it on a prefab imo - Michel
                GameObject go = (Resources.Load("GameManager") as GameObject);
                DontDestroyOnLoad(go);
                privateInstance = Instantiate(go).GetComponent<MehGameManager>();
            }
            return privateInstance;
        }
    }

    private void Awake()
    {
        if (privateInstance != null) { Destroy(gameObject); }
        else
        {
            // Set up if game manager already exists in the scene - Michel
            Initialize();
        }
    }

    /// <summary>
    /// Initialize Game Manager - Michel
    /// </summary>
    void Initialize()
    {
        DontDestroyOnLoad(gameObject);
        privateInstance = this;
        persistent = new PersistentData();
        _canvas.worldCamera = Camera.main;
    }


    // slightly hacky way to start the game for GDC
    private void Start()
    {
        fade.FadeFromBlackActually(3.0f);
        if (SceneManager.GetActiveScene().name == "_overworld") audioMan.PlayMusic(_overworldMusic, 1.0f);
    }

    // Saving & Loading Files
    #region Save/Load
    public void Save()
    {
        SaveLoad.Save(persistent);
    }

    public void Load()
    {
        persistent = SaveLoad.Load();
    }

    #endregion
    // Scene Loading
    #region Scene loading


    /// <summary>
    /// Operations to run when the scene loading process starts, before fading to black
    /// </summary>
    void OnFadeBegin()
    {
        TouchBlocker.BlockInput();
    }

    /// <summary>
    /// Operations to run when the scene is finished loading, before fading from black, need to add this to wherever the scene loads
    /// </summary>
    void OnSceneLoadComplete()
    {
        _canvas.worldCamera = Camera.main;
        TouchBlocker.AllowInput();
    }

    // this dictates how the entire scene loading process should look - Michel
    IEnumerator CO_GenericLoadScene(string sceneName)
    {
        OnFadeBegin();
            
        yield return fade.CO_FadeToBlack(2);

        yield return new WaitForSeconds(2);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone) yield return null;

        OnSceneLoadComplete();

        yield return fade.CO_FadeFromBlack(1);

    }

    public void LoadYarnScene(SceneData yarnScene)
    {
        StartCoroutine(CO_LoadYarnScene(yarnScene));
    }

    IEnumerator CO_LoadYarnScene(SceneData yarnScene)
    {
        yield return CO_GenericLoadScene("_main");

        MehDialogueRunner dr = FindObjectOfType<MehDialogueRunner>();
        dr.StartScript(yarnScene);
    }

    public void LoadShantyScene(SceneData yarnScene)
    {
        StartCoroutine(CO_LoadShantyScene(yarnScene));
    }

    IEnumerator CO_LoadShantyScene(SceneData yarnScene)
    {
        yield return CO_GenericLoadScene("_shanty");
        
        ShantyManager shanty = FindObjectOfType<ShantyManager>();

        shanty.RunShanty(yarnScene);

    }

    public void LoadMainMenu()
    {
        StartCoroutine(CO_GenericLoadScene("_menu"));
    }

    public void LoadOverworld()
    {
        StartCoroutine(CO_GenericLoadScene("_overworld"));

        // TODO: Move this into the overworld manager when I make it
        persistent.SetIntroComplete(true);
    }


    #endregion


}
