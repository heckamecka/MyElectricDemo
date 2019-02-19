using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShantyManager : MonoBehaviour
{
    [Tooltip ("For testing purposes only, remember to clear after you're done!")]
    [SerializeField] public SceneData _sceneData;
    [Tooltip("Time for each character to display")]
    [SerializeField] private float _charDelay;
    [SerializeField] private TextMeshProUGUI _displayUI;
    [SerializeField] private float _speedUpScale;

    private float _timeScale = 1.0f;

    private Coroutine shantyRunner;

    private void Start()
    {
        _displayUI.text = "";
    }

    public void RunShanty(SceneData data)
    {
        if (shantyRunner != null)
        {
            StopCoroutine(shantyRunner);
        }
        shantyRunner = StartCoroutine(CO_RunShanty(data));

        _charDelay = Mathf.Max(_charDelay, 0.0001f);
    }

    public void SpeedUp()
    {
        Debug.Log("speed up called");
        _timeScale = _speedUpScale;
    }

    public void SpeedDown()
    {
        Debug.Log("speed down called");
        _timeScale = 1.0f;
    }

    public IEnumerator CO_RunShanty(SceneData data)
    {
        // Parse Scene Data - Michel
        // TODO: My gut tells me that this process should be done in the SceneData scriptable
        // object but whateves i'll deal with this later - Michel
        string line = data.GetShantyText();
        Dictionary<int, List<Command>> commandsByIndex;
        string displayText = ""; //The text the player will actually see
        MehLineParser.ParseInlineCommands(line, out displayText, out commandsByIndex);

        // Run Shanty Text
        float t = 0;
        int stringProgress = 0;


        // Change text display - Michel
        _displayUI.text = displayText;
        _displayUI.maxVisibleCharacters = (0);

        while (stringProgress < displayText.Length)
        {
            // Run timer until we hit time limit - Michel
            while (t < _charDelay)
            {
                t += Time.deltaTime * _timeScale;
                yield return null;
            }

            // Update text - Michel
            while (t > _charDelay)
            {
                t -= _charDelay;
                //Run our shanty commands - Michel
                if (commandsByIndex.ContainsKey(stringProgress-1)) // this is -1 because ParseCommands doesn't work well with /n characters - Michel
                {
                    foreach (var command in commandsByIndex[stringProgress-1])
                    {
                        yield return this.StartCoroutine(RunShantyCommand(command));
                    }
                }
                stringProgress++;

                _displayUI.maxVisibleCharacters = stringProgress;

            }
        }

        MehGameManager.instance.LoadYarnScene(data);
        
    }

#region Commands and Parsing
    // NOTES------------------------------------------------------------------

    // I'm not sure how I'm copypasta-ing so much of the MehCommandLookups script and there's this 
    // feeling in my gut that tells me I should somehow integrate some of this functionality into that script.
    // I guess I'll find out if this bites me in the butt further down the line - Michel 6/16/2018

    // Somewhat duplicate of RunCommand in MehCommandLookups
    IEnumerator RunShantyCommand(Command command)
    {
        string[] w = command.text.Split(null);
        if (w.Length == 0)
        {
            Debug.LogError("Skipping command; no words in command");
        }

        if (w[0] == "speed") _charDelay = Mathf.Max(StringToFloat(w[1]), 0.0001f);
        else if (w[0] == "wait") yield return pauseForSeconds(duration: StringToFloat(w[1]));

        yield return null;

    }

    /// <summary>
    /// Pause for a specified duration
    /// </summary>
    IEnumerator pauseForSeconds(float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime * _timeScale;
            yield return null;
        }
    }
    
    /// <summary>
    /// Converts string to float
    /// </summary>
    float StringToFloat(string parameter)
    {
        float output;
        if (!float.TryParse(parameter, out output)) {
            Debug.LogError("Failed to convert shanty command parameter " + parameter + " into float");
        }
        return output;
    }
    #endregion


}


