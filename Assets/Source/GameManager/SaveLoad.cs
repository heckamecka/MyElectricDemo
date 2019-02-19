using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveLoad {
    
    static public void Save(PersistentData persistent, string fileName)
    {
        // don't run the save operation if if it's the editor
#if UNITY_EDITOR
        return;
#endif
        // convert persistent to save
        SaveData saveData = PersistentToSave(persistent);

        // Set formatter
        BinaryFormatter bf = new BinaryFormatter();
        // open file
        FileStream file = File.Open(Application.persistentDataPath + "/"+fileName+".dat", FileMode.OpenOrCreate, FileAccess.Write);
        // Save data
        bf.Serialize(file, saveData);

        Debug.Log("Saving data");
    }
    
    static public void Save(PersistentData persistent)
    {
        Save(persistent, "lychee");
    }

    static public PersistentData Load(string fileName)
    {
        // don't run the load operation if if it's the editor
#if UNITY_EDITOR
        return null;
#endif

        Debug.Log("Attempting to load file");
        

        string path = Application.persistentDataPath + "/"+fileName+".dat";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open, FileAccess.Read);
            SaveData saveData = (SaveData)bf.Deserialize(file);

            // convert from save data to persistent data

            return SaveToPersistent(saveData);

        }
        else return null;
    }

    // default loading w/o file name
    static public PersistentData Load()
    {
        return Load("lychee");
    }

    #region Conversion Functions
    static PersistentData SaveToPersistent(SaveData save)
    {
        PersistentData persistent = new PersistentData(save.stringDict.ToDictionary());

        return persistent;
    }

    static SaveData PersistentToSave(PersistentData persistent)
    {
        SaveData save = new SaveData();

        foreach(KeyValuePair<string,Value> kvp in persistent._dataDict)
        {
            save.stringDict.Add(kvp);
        }
        
        return save;
    }
#endregion

}

[System.Serializable]
class SaveData
{
    public SaveData()
    {
        stringDict = new SerializableDictionary<string, Value>();
    }

    public SerializableDictionary<string, Value> stringDict;
}

// Settings
[System.Serializable]
public class SettingsData
{
    public bool sfxEnabled;
    public bool musicEnabled;

    public SettingsData()
    {
        sfxEnabled = true;
        musicEnabled = true;
    }
}