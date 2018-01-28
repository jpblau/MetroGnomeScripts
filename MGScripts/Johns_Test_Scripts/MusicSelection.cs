using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

/// <summary>
/// @Authors: WoozyBytes
/// @Contributors: John Blau
/// Code based on WoozyByte's StackOverflow response, found at https://answers.unity.com/questions/238727/android-music-selection.html
/// </summary>
public class MusicSelection : MonoBehaviour {

    public Text filepathText;   // The text in the BeatMap UI that displays the current filepath

    private AudioSource AS; // The audio source that actually plays the music
    private MusicPicker MP; // The music picker that starts playing the music

    private AudioClip clipToPlay;

    private string curPath; // The current path of our working directory
    private List<string> curDirectoryFolderPaths;   // the list of directories from our current path
    private List<string> curDirectoryFilePaths;     // the list of files from our current path

	// Use this for initialization
	void Start () {
        curDirectoryFolderPaths = new List<string>();
        curDirectoryFilePaths = new List<string>();

        AS = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<AudioSource>();
        MP = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<MusicPicker>();
	}

    private void Update()
    {
        // Hit "U" to get a path to the music folder
        if (Input.GetKeyDown(KeyCode.U))
        {
            FindMusicOnDeviceAndroid();
        }
    }

    /// <summary>
    /// Finds music folder on an Android device
    /// </summary>
    public void FindMusicOnDeviceAndroid()
    {
        // Start by getting our current directory
        curPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        //curPath = Directory.GetCurrentDirectory();
        Debug.Log(curPath);

        filepathText.text = curPath;

        // Check to make sure we found a directory to hop to
        //if (curDirectoryFolderPaths.Count == 0)
        //{
        //    filepathText.text = "No Directory Found";
        //    return;
        //}

        // Look through the currentPath, and get all the .wav and .mp3 files from it
        GetFilePaths(curPath);

        StartCoroutine(LoadFile(curDirectoryFilePaths[1]));

        AS.clip = clipToPlay;

        MP.PlayAndStartMapping();

        // Make sure we found files in that folder
        if (curDirectoryFilePaths.Count == 0)
        {
            filepathText.text = "No Files Found";
            return;
        }
        foreach(String file in curDirectoryFilePaths)
        {
            Debug.Log(file);
            // If we found a wave file at that location, let's say that we founda a wave file
            if (file.Substring(file.Length - 3, 3).Equals("wav"))
            {
                filepathText.text = "WE GOT ONE!";
                return;
            }
        }
    }

    IEnumerator LoadFile(string path)
    {
        WWW www = new WWW("file://" + path);
        while (www == null)
        {
            yield return www;
        }

        AudioClip clip = www.GetAudioClip(false);
        clipToPlay = clip;

    }

    /// <summary>
    /// Gets a list of all the folder paths branching from the given directory
    /// </summary>
    /// <param name="path">The path to the given directory</param>
    private void GetFolderPaths(String path)
    {
        // Feed our path into Directory.GetDirectories, which will return a list of paths of all the folders
        // Make sure we do this in a try/catch since we are handling some IO
        foreach (string folderPath in Directory.GetDirectories(curPath))
        {
            try
            {
                curDirectoryFolderPaths.Add(folderPath);
            }
            catch (Exception e)
            {
                Debug.Log("Could not add directory to list of folder paths");
            }
        }
        Debug.Log("Found " + curDirectoryFolderPaths.Count.ToString() + " Folders in this Directory");
    }

    /// <summary>
    /// Gets a list of all the file paths branching from the given directory
    /// </summary>
    /// <param name="path">The path to the given directory</param>
    private void GetFilePaths(String path)
    {
        // Feed our path into Directory.GetDirectories, which will return a list of paths of all the files
        // Make sure we do this in a try/catch since we are handling some IO
        foreach (string filePath in Directory.GetFiles(curPath))
        {
            try
            {
                curDirectoryFilePaths.Add(filePath);
            }
            catch (Exception e)
            {
                Debug.Log("Could not add file to list of file paths");
            }
        }
        Debug.Log("Found " + curDirectoryFilePaths.Count.ToString() + " Files in this Directory");
    }


}
