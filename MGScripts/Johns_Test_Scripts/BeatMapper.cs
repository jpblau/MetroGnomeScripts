using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatMapper : MonoBehaviour {
    #region Class Vars
    public float timeSinceStartOfSong; // The total amount of time since the song began playing

    public List<float> listOfBeats; // The list of beats that the player has inputted. Each element is a timestamp for a point in the song

    public float timeBetweenStartOfSongAndFirstBeat; // The current amount of time that passed between the start of the song and the first player inputted beat

    public bool currentlyMapping; // Is the player currently mapping out a song? Yes or no = T/F

    public Text count;  // A reference to the BeatMapCanvas' Count Text field

    public float lengthOfSong;  // The length of our currently selected song

    // References to the state of the left and right beat map buttons in the beatMapCanvas; true if the player has pressed the button
    private bool leftPressed = false;
    private bool rightPressed = false;

    // These represent how many iterations of FixedUpdate we wait between button presses, 
    // since they often lag over from one frame to the next. Limit the player's input.
    // 10f = 1/6 of a second
    private float waitForInputLeft = 5f;
    private float waitForInputRight = 5f;

    // Our two lists of beats for spawning on the right and on the left
    public List<float> listOfBeatsLeft;
    public List<float> listOfBeatsRight;

    public List<float> listGenericMegalovania;

    private GameManager GM;
    private InputManager IM;
    private BeatMapperSlider BMS; // A reference to the BeatMapperSlider script, that handles the positioning of all the slider's parts
    #endregion


    private void Start () {

        // Initialize class vars
        timeSinceStartOfSong = 0f;
        listOfBeats = new List<float>();
        timeBetweenStartOfSongAndFirstBeat = 0f;
        currentlyMapping = false;

        // Get our Game Manager
        GM = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        // Get our input manager
        IM = GameObject.FindGameObjectWithTag("Manager").GetComponent<InputManager>();


        // Create our new lists
        listOfBeatsLeft = new List<float>();
        listOfBeatsRight = new List<float>();

        // Find our BeatMapperSlider
        BMS = GameObject.FindGameObjectWithTag("UI_Slider").GetComponent<BeatMapperSlider>();

        // Set all our generic list
        listGenericMegalovania = new List<float>();
        listGenericMegalovania.Add(0.73f);
        listGenericMegalovania.Add(1.23f);
        listGenericMegalovania.Add(1.73f);
        listGenericMegalovania.Add(2.23f);
        listGenericMegalovania.Add(2.73f);
        listGenericMegalovania.Add(3.23f);
        listGenericMegalovania.Add(3.73f);
        listGenericMegalovania.Add(4.23f);
        listGenericMegalovania.Add(4.73f);
        listGenericMegalovania.Add(5.23f);
        listGenericMegalovania.Add(5.73f);
        listGenericMegalovania.Add(6.23f);
        listGenericMegalovania.Add(6.73f);
        listGenericMegalovania.Add(7.23f);
        listGenericMegalovania.Add(7.73f);
        listGenericMegalovania.Add(8.23f);
        listGenericMegalovania.Add(8.73f);
        listGenericMegalovania.Add(9.23f);
        listGenericMegalovania.Add(9.73f);
        listGenericMegalovania.Add(10.23f);
        listGenericMegalovania.Add(10.73f);
        listGenericMegalovania.Add(11.23f);
        listGenericMegalovania.Add(11.73f);
        listGenericMegalovania.Add(12.23f);
        listGenericMegalovania.Add(12.73f);
        listGenericMegalovania.Add(13.23f);
        listGenericMegalovania.Add(13.73f);

    }
	
    /// <summary>
    /// Called when the player would like to begin mapping out a beat
    /// Clears the list of beats, and sets currentlyMapping to true
    /// </summary>
    public void StartMapping()
    {
        // Clear our lists in the given selection
        ClearSelection();
        // reset our time since the start of the song
        timeSinceStartOfSong = CalcLeftTimeStamp();
        // begin mapping
        currentlyMapping = true;


            listOfBeats.Clear();

    }
    #region Buttons
    /// <summary>
    /// Called when the left mapping button is pressed down
    /// </summary>
    public void LeftPressed()
    {
        leftPressed = true;
    }

    /// <summary>
    /// Called when the right mapping button is pressed down
    /// </summary>
    public void RightPressed()
    {
        rightPressed = true;
    }

    /// <summary>
    /// Clears all the beats in the current slider selection from the leftmost timestamp to the rightmost time stamp
    /// </summary>
    public void ClearSelection()
    {
        float leftTime = CalcLeftTimeStamp();
        float rightTime = CalcRightTimeStamp();

        List<float> toRemove = new List<float>();

        // loop through both the left and right beat maps
        // left
        foreach (float beat in listOfBeatsLeft)
        {
            if (beat <= rightTime && beat >= leftTime)
            {
                toRemove.Add(beat);
            }
        }
        foreach (float beatRm in toRemove)
        {
            // Remove both the marker and the actual beat
            BMS.RemoveMarker(beatRm);
            listOfBeatsLeft.Remove(beatRm);
        }

        // clear our list of beats to remove before the next list
        toRemove.Clear();

        // then right
        foreach (float beat in listOfBeatsRight)
        {
            if (beat <= rightTime && beat >= leftTime)
            {
                toRemove.Add(beat);
            }
        }
        foreach (float beatRm in toRemove)
        {
            // Remove both the marker and the actual beat
            BMS.RemoveMarker(beatRm);
            listOfBeatsRight.Remove(beatRm);
        }

    }
    #endregion

    /// <summary>
    /// Runs at a fixed interval and increments our timeSinceStartOfSong
    /// </summary>
    private void Update()
    {
        // Update the time represented by the positions of our sliders
        BMS.leftTimeStamp.text = SecondsToTimestamp(CalcLeftTimeStamp());
        BMS.rightTimeStamp.text = SecondsToTimestamp(CalcRightTimeStamp());

        if (currentlyMapping)
        {
            timeSinceStartOfSong += Time.deltaTime;

            // If we're on a touchpad
            if (GM.MOBILEGAME)
            {
                #region Touchpad Input
                if (leftPressed)
                {
                    AddToListOfBeats(true);
                }

                if (rightPressed)
                {
                    AddToListOfBeats(false);
                }
                #endregion
            }

            // If we're on a computer
            if (GM.DESKTOPGAME)
            {
                #region Key Input
                // We're using key down here because it will feel better for the player than key up
                if (Input.GetKeyDown(KeyCode.M) || leftPressed)
                {
                    AddToListOfBeats(false);
                }

                if (Input.GetKeyDown(KeyCode.Z) || rightPressed)
                {
                    AddToListOfBeats(true);
                }
                #endregion
            }

            // Reset our leftPressed and rightPressed for the next Update
            leftPressed = false;
            rightPressed = false;

        }

    }


    /// <summary>
    /// Converts a number of seconds into the format "minutes:seconds". (Ex: 71 seconds = 1:11 seconds)
    /// </summary>
    /// <param name="seconds"></param>
    private string SecondsToTimestamp(float length)
    {
        // Calculate our time in the proper format
        float minutes = ((length / 60f) - (length / 60f) % 1f);
        float seconds = ((length % 60f) - (length % 60f) % 1f);

        // if seconds is only one digit, append a 0 in front of it
        if (seconds / 10 < 1)
        {
            return (minutes + ":0" + seconds);
        }
        return (minutes + ":" + seconds);
    }

    /// <summary>
    /// Adds a beat to our list of beats
    /// </summary>
    /// <param name="spawnLeft">Signifies whether this beat is to spawn on the left (true) or right (false) of the screen</param>
    public void AddToListOfBeats(bool spawnLeft)
    {
        // Check to make sure we're currently mapping
        if (!currentlyMapping)
        {
            return;
        }

        // Set our list we are referencing (spawnLeft = true then left)
        if (spawnLeft)
        {
            //referencedList = listOfBeatsLeft;
            listOfBeatsLeft.Add(timeSinceStartOfSong);
        }
        else if (!spawnLeft)
        {
            //referencedList = listOfBeatsRight;
            listOfBeatsRight.Add(timeSinceStartOfSong);
        }

        // Add a marker to the screen, showing that a beat has been inputted
        BMS.PlaceMarker(timeSinceStartOfSong, lengthOfSong); 

        #region TODO remove this
        // Single-button mapping, that also counts the total number of beats inputted
        if (currentlyMapping)
        {
            listOfBeats.Add(timeSinceStartOfSong);
        }
        #endregion 

        // Update the UI text
        count.text = "Number of beats: " + listOfBeats.Count;

        // if this is the first beat in the list, calculate timeBetweenStartOfSongAndFirstBeat
        if (listOfBeats.Count == 1)
        {
            timeBetweenStartOfSongAndFirstBeat = timeSinceStartOfSong;
        }
    }

    /// <summary>
    /// Calculate the time in the song that the left slider currently indicates, based on its distance from LEFT_MAX
    /// </summary>
    public float CalcLeftTimeStamp()
    {
        // get the distance between the leftmost position and the current position of the left slider
        float distance = BMS.leftSliderTrans.position.x - BMS.LEFT_MAX;

        // calculate what portion of the song that distance represents
        float part = (distance / BMS.DISTANCE);

        // Get a total amount of time in the song that percentage corresponds to
        float time = lengthOfSong * part;

        // Add that time to the start of the song (0 seconds) and return it
        return (time + 0f);
    }

    /// <summary>
    /// Calculate the time in the song that the right slider currently indicates, based on its distance from RIGHT_MAX
    /// </summary>
    private float CalcRightTimeStamp()
    {
        // get the distance between the rightmost position and the current position of the right slider
        float distance = BMS.RIGHT_MAX - BMS.rightSliderTrans.position.x;

        // calculate what portion of the song that distance represents
        float part = (distance / BMS.DISTANCE);

        // Get a total amount of time in the song that percentage corresponds to
        float time = lengthOfSong * part;

        // Subtract that time from the max length of the song and return it
        return (lengthOfSong - time);
    }
}
