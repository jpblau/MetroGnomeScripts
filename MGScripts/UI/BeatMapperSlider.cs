using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatMapperSlider : MonoBehaviour
{

    public GameObject marker; // A marker prefabs that is placed on the slider when the user inputs a beat

    public GameObject leftSlider;   // The leftmost button on the slider
    public GameObject rightSlider;  // The rightmost button on the slider
    public Text leftTimeStamp;   // The Text that represents the current timestamp of the left slider
    public Text rightTimeStamp;    // The Text that represents the current timestamp of the right slider
    public Transform leftSliderTrans;  // the transform of our leftSlider
    public Transform rightSliderTrans; // the transform of our rightSlider

    public Canvas beatMapCanvas;    // The canvas this script is attached to

    public float LEFT_MAX; // The leftmost position the left slider will be at, indicating the first second of the song
    public float RIGHT_MAX;    // The rightmost position the right slider will be at, indicating the last second of the song
    public float DISTANCE; // The distance from the left to the right maxes. This is our total distance, and the total "length" of the song

    private List<GameObject> listOfMarkers;

    // Use this for initialization
    void Start()
    {
        // Create our list of Markers
        listOfMarkers = new List<GameObject>();

        // Get our slider transforms
        leftSliderTrans = leftSlider.transform;
        rightSliderTrans = rightSlider.transform;

        // set our left and right maxes, as well as the distance
        LEFT_MAX = leftSliderTrans.position.x;
        RIGHT_MAX = rightSliderTrans.position.x;
        DISTANCE = RIGHT_MAX - LEFT_MAX;
    }

    /// <summary>
    /// Places a marker at the given time, and adds the marker to the listOfMarkers
    /// </summary>
    /// <param name="time"></param>
    public void PlaceMarker(float time, float lengthOfSong)
    {
        // Create our new Marker
        GameObject newMarker = GameObject.Instantiate(marker) as GameObject;
        // Set it as a child of our Canvas
        newMarker.transform.SetParent(beatMapCanvas.transform, false);

        // Set the position of our new Marker
        newMarker.GetComponent<Marker>().SetPosition(time, lengthOfSong);
       
        // Add the new marker to our list
        listOfMarkers.Add(newMarker);
    }

    /// <summary>
    /// Removes a marker from the listOfMarkers that has the given time
    /// </summary>
    public void RemoveMarker(float time)
    {
        // Iterate in reverse so that we don't run into any issues
        for (int i = listOfMarkers.Count - 1; i >= 0; i--)
        {
            if (listOfMarkers[i].GetComponent<Marker>().time == time)
            {
                Destroy(listOfMarkers[i]);
                listOfMarkers.RemoveAt(i);
                break;
            }
        }
    }
}