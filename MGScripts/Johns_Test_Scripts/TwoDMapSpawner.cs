using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is used to enable UI Image elements in the scene that 
/// move to the beat just like their 3D counterparts do. 
/// Authors: John Blau
/// </summary>
public class TwoDMapSpawner : MonoBehaviour {

    #region Class Vars
    private int NUM_PRELOADED_IMAGES = 10;
    #endregion

	// Use this for initialization
	void Start () {
		// Loop through and instantiate all of the Images we want to pre-load. Make sure none of them are enabled
        for (int num = 0; num < NUM_PRELOADED_IMAGES; num++)
        {
            
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
