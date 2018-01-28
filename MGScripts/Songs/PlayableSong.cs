using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// @Author: John Blau
/// A song that the player may choose to play
/// Created when the player maps out beats for a song that does not have any saved data for it
/// Some PlayableSongs are saved as part of Discovery Mode
/// </summary>
public class PlayableSong {

    #region Class Variables
    public List<SongPart> parts;    // A list of parts in this song
    public string name;     // This song's name
    public float length;    // The length of the song

    private bool unlocked;   // True if the player has unlocked this track to use
    public bool Unlocked    // This getter/setter exists so that we don't accidentily overwrite unlocked anywhere
    {
        get { return unlocked; }
        set { unlocked = value; }
    }
    #endregion

    /// <summary>
    /// Create a PlayableSong, for a song without any parts to it yet
    /// </summary>
    public PlayableSong()
    {
        // Create a new song part for this song
        SongPart songPart = new SongPart();

        // Set our list of parts
        parts = new List<SongPart>();
    }
    

}
