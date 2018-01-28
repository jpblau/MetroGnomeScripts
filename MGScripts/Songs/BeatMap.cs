using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class creates a new BeatMap and all the associated metadata
/// <Author> John Blau </Author>
/// </summary>
public class BeatMap {

    public List<float> beatMap; // This beatMap's actual list of beats

    public bool left; // True if this beatMap is for the left side of the screen, false if for the right side
    public int mapPos; // The map position we are currently at
    public float untilNextSpawn; // The time at which we are supposed to spawn in the next beat
    public float sinceLastSpawn; // The time that has passed since we last spawned a beat

    public BeatMap(List<float> beatMapList, bool isLeft)
    {
        // Make sure our list has something in it
        if (beatMapList.Count < 1)
        {
            return;
        }
        // Add another fake beat to the head of the list so we always spawn in based on the difference between two beats, if there's not already one there
        if (beatMapList[0] != 0f)
        {
            beatMapList.Insert(0, 0f);
        }

        // Create our new beat map and set initial values
        this.beatMap = beatMapList;
        left = isLeft;
        mapPos = 1;
        untilNextSpawn = 0f;
        sinceLastSpawn = 0f;
        mapPos = 0;

    }
}
