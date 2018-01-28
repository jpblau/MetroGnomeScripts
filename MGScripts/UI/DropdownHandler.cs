using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * Drop Down List  class
 * @Author: Barrington Campbell
 * Handles the dropdown list songs and selection
 */
public class DropdownHandler : MonoBehaviour
{
    #region Attributes
    public Dropdown dropdown;       //Drop down list GameObject

    //List of songs to choose from in the Drop Down
    List<string> songs = new List<string>() { "SELECT SONG", "HERO TOWN", "MEGALOVANIA", "SPOOK SONG", "SKRILLEX - MAKE IT BUN DEM" };
    MusicPicker musicPck;
    #endregion

    /// <summary>
    /// Used to initialize the components and populat the list
    /// </summary>
    void Start()
    {
        musicPck = GetComponent<MusicPicker>();
        PopulateDropDown();
    }

    /// <summary>
    /// Populate the Dropdown list
    /// </summary>
    void PopulateDropDown()
    {
        dropdown.AddOptions(songs);
    }

    /// <summary>
    /// Select the song to play using the given index
    /// </summary>
    /// <param name="index"></param>
    public void IndexChange(int index)
    {
        //Choose the correct song to play
        switch (index)
        {
            case 1:
                musicPck.PlayHeroTown();
                break;
            case 2:
                musicPck.PlayMegalovania();
                break;
            case 3:
                musicPck.PlaySpookSong();
                break;
            case 4:
                musicPck.PlaySkrillex();
                break;
        }

        //Arrange the selected object to be played using Music Picker
        //char delimiter = '-';
        //string[] substrings = songs[index].Split(delimiter);
        //string song = substrings[0].ToLower();
        //musicPck.
    }
}