using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {

    [SerializeField] GameObject _backGround;
    [SerializeField] GameObject _settingsMenu;
    [SerializeField] GameObject _quitButton;
    [SerializeField] ConfirmPopUp _confirm;

    [SerializeField] string returnToMapPrompt;
    [SerializeField] string quitPrompt;

    [SerializeField] Toggle sfxToggle;
    [SerializeField] Toggle musicToggle;
    
    // accessor functions for toggle button - Michel
    public bool sfxEnabled { get { return MehGameManager.instance.persistent.sfxEnabled; } set { MehGameManager.instance.persistent.sfxEnabled = value; } }
    public bool musicEnabled { get { return MehGameManager.instance.persistent.musicEnabled; } set { MehGameManager.instance.persistent.sfxEnabled = value; } }

    private bool _menuOpen = false;

    private void Awake()
    {
        _backGround.SetActive(false);
        _settingsMenu.SetActive(false);
        _menuOpen = false;
    }

    private void Start()
    {
        sfxToggle.isOn = sfxEnabled;
        musicToggle.isOn = musicEnabled;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //TODO: Make a more robust system to handle the escape / back button in menu navigation
            if (_menuOpen) CloseMenu();
            else OpenMenu();
        }
    }

    public void OpenMenu()
    {
        _menuOpen = true;
        Debug.Log("open menu called");
        _backGround.SetActive(true);
        _settingsMenu.SetActive(true);

        // pause dialogue machine
        MehDialogueMachine._pauseTimeScale = 0f;
    }

    public void CloseMenu()
    {
        _menuOpen = false;
        _backGround.SetActive(false);
        _settingsMenu.SetActive(false);

        // resume dialogue machine
        MehDialogueMachine._pauseTimeScale = 1f;
    }

    // TODO: Check if the intro has been played already
    public void ReturnToMap()
    {
        _confirm.ShowPopUp(() => MehGameManager.instance.LoadOverworld(), returnToMapPrompt);
    }
    
    // TODO: Make this only show up in PC/mac builds
    public void QuitGame()
    {
        _confirm.ShowPopUp(() => Application.Quit(), quitPrompt);
    }

}
