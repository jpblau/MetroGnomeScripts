using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    #region Class Vars
    public GameObject spawner;  // The object that spawns our enemies into the scene
    private EnemySpawner enemySpawner;  // The script that will spawn enemies into the scene. Part of spawner
    private static string filePath; // The filepath to our Json file
    private string jsonString;  // The string containing the contents of the Json file
    public RootObject rootObj = new RootObject();   // the rootobject is the base of the Class stucture for Beepbox's Json implementation
    #endregion

    /// <summary>
    /// Gets our spawner, finds our path, and reads in a Json file
    /// </summary>
    void Start()
    {
        // Get our enemySpawner
        enemySpawner = spawner.GetComponent<EnemySpawner>();

        // Set the filePath = Application.Assets and concatenate the rest of the path
        filePath = Application.streamingAssetsPath + "/SpookSong1JSON16.json";

        // Finds a file, opens it, reads everything, then closes it
        jsonString = File.ReadAllText(filePath);

        JsonUtility.FromJsonOverwrite(jsonString, rootObj);

    }

}
// Below is the structure for our BeepBox given Json file
#region BeepBox Json Class structure

[System.Serializable]
//MusicManager.Rootobj.getbeats;
public class Instrument
{
    public int volume;
    public string wave;
    public string envelope;
    public string filter;
    public string chorus;
    public string effect;
}

[System.Serializable]
public class Pattern
{
    public int instrument;
    public List<object> notes;
}

[System.Serializable]
public class Channel
{
    public int octaveScrollBar;
    public List<Instrument> instruments;
    public List<Pattern> patterns;
    public List<int> sequence;
}

[System.Serializable]
public class RootObject
{
    public int version;
    public string scale;
    public string key;
    public int introBars;
    public int loopBars;
    public int beatsPerBar;
    public int ticksPerBeat;
    public int beatsPerMinute;
    public int reverb;
    public List<Channel> channels;
}

#endregion

