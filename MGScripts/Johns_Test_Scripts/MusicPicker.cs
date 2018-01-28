using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPicker : MonoBehaviour
{
    #region Class Vars
    public GameObject soundManager; // The soundManager that controls playing songs 
    public AudioClip megalovania;   // These next few should probably be replaced in the future with a list, but this is a refrence to our AudioClip for Megalovania
    public AudioClip spookSong; // A reference to our SpookSong AudioClip
    public AudioClip skrillex;  // A reference to our Skrillex AudioClip
    public AudioClip vulf;  // A reference to our Hero Town AudioClip

    private BeatMapper beatMapper;  // The beatMapper class that controls the player's map. A part of the soundManager
    private AudioSource audioSource;    // The audioSource that plays the songs. A part of the soundManager
    #endregion

    void Start()
    {
        // Initialize our class variables
        audioSource = soundManager.GetComponent<AudioSource>();
        beatMapper = soundManager.GetComponent<BeatMapper>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayMegalovania();
        }
    }

    /// <summary>
    /// Plays Spook Song and sets the approptiate audio levels
    /// </summary>
    public void PlaySpookSong()
    {
        audioSource.clip = spookSong;
        PlayAndStartMapping();
        audioSource.volume = 1.0f;   
    }

    /// <summary>
    /// Plays Megalovania and sets the appropriate audio levels
    /// </summary>
    public void PlayMegalovania()
    {
        audioSource.clip = megalovania;
        PlayAndStartMapping();
        audioSource.volume = .2f;
    }

    public void SetMegalovania()
    {
        audioSource.clip = megalovania;
        audioSource.volume = .2f;
    }

    /// <summary>
    /// Plays Skrillex and sets the appropriate audio levels
    /// </summary>
    public void PlaySkrillex()
    {
        audioSource.clip = skrillex;
        PlayAndStartMapping();
        audioSource.volume = .2f;
    }

    /// <summary>
    /// Plays Hero Town and sets the appropriate audio levels
    /// </summary>
    public void PlayHeroTown()
    {
        audioSource.clip = vulf;
        PlayAndStartMapping();
        audioSource.volume = .6f;
    }

    /// <summary>
    /// Begins playing the set audioClip and starts mapping out beats to the song
    /// Starts the clip at the time selected in the beatMapCanvas
    /// Also sets the current song in BeatMapper
    /// </summary>
    public void PlayAndStartMapping()
    {
        // Set our song's length in BeatMapper
        beatMapper.lengthOfSong = audioSource.clip.length;

        // set the time at which to start the song
        audioSource.time = beatMapper.CalcLeftTimeStamp();

        audioSource.Play();
        audioSource.loop = true;
        // Enable our player to begin mapping out the beats
        beatMapper.StartMapping();

        
    }

}
