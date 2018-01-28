using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Input Manager Class
/// Coded by Matthew Barry
/// Handles all user input in the game
/// </summary>
public class InputManager : MonoBehaviour
{
    #region Variables
    // public bool variables
    public bool singleInput = false;
    public bool doubleInput = false;
    public bool leftSideTouched = false;
    public bool rightSideTouched = false;

    // private object variables
    private GameManager gm; // game manager
    #endregion

    /// <summary>
    /// Sets reference objects for components
    /// </summary>
    void Start()
    {
        // set component to attributes script
        gm = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
    }

    /// <summary>
    /// Detects all user inputs and sets public booleans accordingly
    /// </summary>
    void Update()
    {
        // if the game is being played on a computer and not mobile
        if (gm.DESKTOPGAME == true)
        {
            singleInput = false;
            doubleInput = false;
            // input for either of the player buttons to attack
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.M))
            {
                // input to see if player hit both keys at the same time
                if (Input.GetKeyDown(KeyCode.Z) && Input.GetKeyDown(KeyCode.M))
                {
                    singleInput = false;
                    doubleInput = true;
                }
                // only one key was pressed, continue on from here
                else
                {
                    singleInput = true;
                    doubleInput = false;
                }
            }
        }
        // if the game is being played on a mobile device and not a computer
        else if (gm.MOBILEGAME == true)
        {
            leftSideTouched = false;
            rightSideTouched = false;
            singleInput = false;
            doubleInput = false;
            // if there are any touches currently
            if (Input.touchCount > 0)
            {
                // loop through all of the touches we have in the input manager right now
                foreach (Touch touch in Input.touches)
                {
                    // get the current touch we are checking
                    Touch currentTouch = touch;

                    // if it began this frame
                    if (currentTouch.phase == TouchPhase.Began)
                    {
                        // check to see if the left side of the screen has been touched
                        if (currentTouch.position.x < Screen.width / 2)
                        {
                            leftSideTouched = true;
                        }
                        // check to see if the right side of the screen has been touched
                        else if (currentTouch.position.x > Screen.width / 2)
                        {
                            rightSideTouched = true;
                        }
                    }
                }
                // loop ends and we see what our results are for either single touch or double touch
                // if left side touched but right side was not touched
                if (leftSideTouched == true && rightSideTouched == false)
                {
                    singleInput = true;
                    doubleInput = false;
                }
                // if right side was touched but left side was not 
                else if (leftSideTouched == false && rightSideTouched == true)
                {
                    singleInput = true;
                    doubleInput = false;
                }
                // both left and right side of the screen were touched
                else if (leftSideTouched == true && rightSideTouched == true)
                {
                    singleInput = false;
                    doubleInput = true;
                }
            }
        }
    }
}