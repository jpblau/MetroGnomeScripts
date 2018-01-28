using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @Author: John Blau
/// A part for a playableSong
/// Selecting a part allows you to play it in the song
/// </summary>
public class SongPart {

    #region Class Variables
    public float beginStamp;    // The timestamp of the beginning of this part in the song
    public float endStamp;  // The timestamp of the end of this part in the song

    public string partName; // The name of this part written for the song

    public List<BeatMap> beats; // A list of beatMaps that accompany this part and tell EnemySpawner when to enemies should be spawned
    #endregion

    /// <summary>
    /// Create a new part for this playable song
    /// </summary>
    public SongPart()
    {
        // Set a default name
        partName = "newPart";

        // Set our beginning and ending floats initially
        beginStamp = 0.00f;
        endStamp = 0.01f;

        // Set our list of beats
        beats = new List<BeatMap>();
    }

    /// <summary>
    /// Set the name for this part
    /// Ex: Trumpet, Drums, vocals
    /// </summary>
    public void SetPartName(string name)
    {
        partName = name;
    }

    /// <summary>
    /// Add a beatMap to this part's list of beatMaps
    /// </summary>
    public void AddBeatMap(BeatMap bm)
    {
        beats.Add(bm);
    }

    /// <summary>
    /// Sets time stamps for the beginning of and end of this part
    /// </summary>
    public void SetPartTimeStamps()
    {
        // TODO implement this
    }
}
