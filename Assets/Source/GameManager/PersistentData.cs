using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData {

    public Dictionary<string, Value> _dataDict { get; private set; }
    public SettingsData _settings;
    
    public float sfxVolume { get { return _settings.sfxEnabled ? 1.0f : 0.0f; } }
    public float musicVolume { get { return _settings.musicEnabled ? 1.0f : 0.0f; } }

    // Constructors
    public PersistentData()
    {
        Initialize(new Dictionary<string, Value>(), new SettingsData());
    }

    public PersistentData(Dictionary<string, Value> data)
    {
        Initialize(data, new SettingsData());
    }

    // Helper constructor function - Michel
    void Initialize(Dictionary<string, Value> data, SettingsData settings)
    {
        _dataDict = data;
        _settings = settings;
    }

    // Getting and Setting Bools, Ints
    // TODO: figure out what the fuck to do with strings - Michel
    #region General Data

    public Value Get(string key)
    {
        // if it contains a key, return the data stored
        if (_dataDict.ContainsKey(key))
        {
            return _dataDict[key];
        }
        else // return a value initialized to 0
        {
            Value output = new Value();
            output.b = false;
            output.f = 0f;
            output.i = 0;
            output.type = VarType.Null;
            return output;
        }
    }

    public void Set(string key, Value value)
    {
        _dataDict[key] = value;
    }

    public void SetBool(string key, bool boolVal)
    {
        Value value = new Value(boolVal);
        _dataDict[key] = value;
    }

    public bool GetBool(string key)
    {
        if (_dataDict.ContainsKey(key))
        {
            if (_dataDict[key].type == VarType.Bool)
            {
                return _dataDict[key].b;
            }
            else Debug.LogError("Cannot convert boolean from key [" + key + "].");
        }
        return false;
    }
    
    public void SetInt(string key, bool intVal) {
        Value value = new Value(intVal);
        _dataDict[key] = value;
    }

    public int GetInt(string key)
    {
        if (_dataDict.ContainsKey(key))
        {
            if (_dataDict[key].type == VarType.Bool)
            {
                return _dataDict[key].i;
            }
            else Debug.LogError("Cannot convert boolean from key [" + key + "].");
        }
        return 0;
    }

    public void PrintData()
    {
        foreach(KeyValuePair<string, Value> kvp in _dataDict)
        {
            string val = "";

            switch(kvp.Value.type)
            {
                case VarType.Bool:
                    val = kvp.Value.b.ToString();
                    break;
                case VarType.Int:
                    val = kvp.Value.i.ToString();
                    break;
                case VarType.Float:
                    val = kvp.Value.f.ToString();
                    break;
                case VarType.Null:
                    val = "null";
                    break;
            }

            Debug.Log(kvp.Key + " | " + kvp.Value.type + " | " + val);

        }
    }

    #endregion

    #region Scene Visited

    // helper function to avoid mistyping "visited_"- Michel
    string SceneToKey(string scene)
    {
        return ("visited_" + scene);
    }

    public void SetSceneVisited(string scene, bool visited)
    {
        SetBool(SceneToKey(scene), visited);
    }

    public bool GetSceneVisited(string scene)
    {
        if (_dataDict.ContainsKey(SceneToKey(scene)))
        {
            return GetBool(SceneToKey(scene));
        }
        else return false;
    }
    
    #endregion

    // Getting whether or not a location shows up on the map at all. If 
    // it's deactivated it means the node shouldn't show up on screen at all - Michel
    #region Location
    // helper function to avoid mistyping "location_"- Michel
    string LocationToKey(string scene)
    {
        return ("location_" + scene);
    }

    public void ActivateLocation(string location)
    {
        SetBool(LocationToKey(location), true);
    }

    public bool GetLocationActive(string location)
    {
        if (_dataDict.ContainsKey(LocationToKey(location)))
        {
            return GetBool(LocationToKey(location));
        }
        else return false;
    }
    #endregion

    #region Misc checks
    public bool GetIntroComplete()
    {
        return GetBool("intro_complete");
    }

    public void SetIntroComplete(bool complete)
    {
        SetBool("intro_complete", complete);
    }

    #endregion

    #region Audio

    // accessor functions to make things cleaner- Michel
    public bool sfxEnabled { get { return _settings.sfxEnabled; } set { _settings.sfxEnabled = value; } }
    public bool musicEnabled { get { return _settings.musicEnabled; } set { _settings.sfxEnabled = value; } }

    #endregion

}

