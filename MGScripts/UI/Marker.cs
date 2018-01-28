using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// @Author: John Blau
/// The Marker Class represents a UI element with the given graphic that is placed on the beatMap slider when 
/// the user inputs a beat.
/// </summary>
public class Marker : MonoBehaviour {

    public Sprite graphic; // The graphic that represents when the user has inputted a beat

    public float time; // The time in the song (in seconds) that this markers represents

    private BeatMapperSlider BMS;   // A reference to the BeatMapperSlider script, that handles the positioning of all the slider's parts

    public void Awake()
    {
        // Find our BeatMapperSlider
        BMS = GameObject.FindGameObjectWithTag("UI_Slider").GetComponent<BeatMapperSlider>();

        // Set the raw image's texture
        this.gameObject.GetComponent<Image>().sprite = graphic;

    }

    /// <summary>
    /// Sets the position and time of the marker.
    /// Should be called right after a Marker is instantiated.
    /// </summary>
    /// <param name="time">The time which this marker represents</param>
    /// <param name="lengthOfSong">The length of the selected song</param>
    public void SetPosition(float time, float lengthOfSong)
    {
        // Set our time
        this.time = time;

        // Set our position
        this.transform.position = TimeToPosition(lengthOfSong);
    }

    /// <summary>
    /// Finds the correct position in the Canvas for the Marker to be placed based on the time and the total song length
    /// </summary>
    /// <param name="lengthOfSong">The length of the selected song</param>
    /// <returns>The proper position of the marker</returns>
    private Vector3 TimeToPosition(float lengthOfSong)
    {
        // We only care about the x position of the element, since it is the only thing that will change
        float posX = 0f;

        // Get a decimal for how far into the song we are
        float location = time / lengthOfSong;

        // Take that decimal and scale our new position to a place that far along the slider
        posX = BMS.LEFT_MAX + (BMS.DISTANCE * location);

        // Return that location in the x direction, and on the slider's y position
        return new Vector3(posX, BMS.leftSliderTrans.position.y, 0);
    }
}
