using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Light Flicker Class
/// Coded by Matthew Barry
/// Handles lighting effects to make them flash on and off
/// Works for starter and gameplay lights
/// </summary>
public class LightFlicker : MonoBehaviour {

    #region Variables
    // public object variables
    public Light myLight; // the light component
    // public numeric variables
    public float totalSeconds; // the total of seconds the flash wil last
    public float maxIntensity; // the maximum intensity the flash will reach
    public float intensityStep = 0; // how much the light instensity will increase and decrease
    public float sinceLastFrame = 0; // time since last frame
    public float beginningDelay = 0; // delay for the light flashing at first
    // public boolean variables
    public bool flash = false; // to flash or not to flash, that is the question
    public bool repeat = false; // do we want this light to repeat it's flicker?
    // public array variables
    public List<Color> colors = new List<Color>(); // list of colors the light can be
    // private boolean values
    private bool beginningLight = true;
    #endregion

    /// <summary>
    /// Handles the flickering of the light
    /// </summary>
    public void FixedUpdate()
    {
        // can we begin the flicker of the light?
        if (sinceLastFrame >= beginningDelay)
        {
            if(beginningLight == true)
            {
                beginningDelay = 0;
                sinceLastFrame = 0;
                beginningLight = false;
            }
            // if the light should flicker
            if (flash == true)
            {
                this.gameObject.tag = "FlickeringLight";
                // while timer is still below our restart time limit
                if (sinceLastFrame < totalSeconds / 2)
                {
                    // adds time since last frame to the float
                    sinceLastFrame += Time.deltaTime;
                    myLight.intensity += intensityStep; // increase intensity
                }
                else if (sinceLastFrame >= totalSeconds / 2 && sinceLastFrame <= totalSeconds)
                {
                    // adds time since last frame to the float
                    sinceLastFrame += Time.deltaTime;
                    myLight.intensity -= intensityStep; // decrease intensity
                }
                else
                {
                    // if we want our light to flicker again, pick a new color and do it
                    if (repeat)
                    {
                        sinceLastFrame = 0;
                        int randomColorNum = Random.Range(0, colors.Count);
                        myLight.color = colors[randomColorNum];
                    }
                    // want to set light game ready, stop flicking, reset total seconds varible
                    else
                    {
                        this.gameObject.tag = "AvailableLight";
                        flash = false;
                        myLight.enabled = false;
                        sinceLastFrame = 0;
                        totalSeconds = 2;
                        intensityStep = maxIntensity / 60;
                        intensityStep = intensityStep / (totalSeconds / 2);
                    }
                }
            }
        }
        // haven't passed the delay barrier yet, increase time since last checked
        else
        {
            sinceLastFrame += Time.deltaTime;
        }
    }
}
