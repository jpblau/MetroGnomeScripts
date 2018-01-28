using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SettingManager : MonoBehaviour
{
    #region Attributes
    //UI elements
    public Slider musicSlider;
    public Toggle beatIndicationToggle;
    public Image beatIndicationImage;
    public Button leftButton;
    public Button rightButton;
    public Button applyButton;
  
    //Game Elements
    public GameSettings gameSettings;
    public AudioSource musicSource;
    #endregion

    private static SettingManager dontDestroy; // Ensures that there is only one instance of this gameObject

    #region Don't Destroy
    private void Awake()
    {
        DontDestroyBetweenScenes();
    }

    /// <summary>
    /// Ensures that this game object is not destroyed between scenes, and thus does not have to be re-loaded in
    /// </summary>
    private void DontDestroyBetweenScenes()
    {
        // Ensure that this is the only object we are not destroying, and that we create no duplicates
        if (dontDestroy == null)
        {
            DontDestroyOnLoad(this.gameObject);     // We don't want this object to be destroyed any time we change scenes
            dontDestroy = this;
        }
        else if (dontDestroy != this)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion


    void Start ()
    {
        musicSource.volume = gameSettings.musicVol = musicSlider.value;
    }
	

	void OnEnable()
    {
        gameSettings = new GameSettings();

        musicSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChange(); });
        beatIndicationToggle.onValueChanged.AddListener(delegate { OnBeatIndicationToggle(); });
        //leftButton.onClick.AddListener(delegate { OnLeftButtonClick(); });
        //rightButton.onClick.AddListener(delegate { OnRightButtonClick(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });

        LoadSettings();
    }

 
    ///Event handlers
    private void OnBeatIndicationToggle()
    {
        gameSettings.beatIndication = beatIndicationImage.enabled = beatIndicationToggle.isOn;
    }

    private void OnMusicVolumeChange()
    {
        musicSource.volume = gameSettings.musicVol = musicSlider.value;
    }

    private void OnRightButtonClick()
    {
        throw new NotImplementedException();
    }

    private void OnLeftButtonClick()
    {
        throw new NotImplementedException();
    }

    private void OnApplyButtonClick()
    {
        SaveSettings();
    }

    ///Helper methods
    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
    }

    public void LoadSettings()
    {
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
        musicSlider.value = gameSettings.musicVol;
        beatIndicationToggle.isOn = gameSettings.beatIndication;
    }
}
