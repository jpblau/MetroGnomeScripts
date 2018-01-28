using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// State Manager Class
/// @Author: Matthew Barry
/// @Contributors: John Blau
/// Handles switching between game states
/// </summary>
public class StateManager : MonoBehaviour {

    #region Variables
    // public canvas objects
    /*
     * List of what order the statesUI needs to be in
     * 0 - Main Menu Canvas
     * 1 - Game Canvas
     * 2 - Beat Mapper Canvas
     * 3 - Options
     */
    public List<Canvas> statesUI = new List<Canvas>();
    public enum GameState {MainMenu, Game, BeatMapper, Options }; // states of the game that correspond to the list of canvas above
                                                                 // i.e. MainMenu is first in states, therefore it is statesUI[0]
    public GameState currentGameState; // our current game state
    public float bufferTime; // buffer time between the players death and main menu trigger
    private CameraManager camManager;
    #endregion

    /// <summary>
    /// Sets initial state
    /// </summary>
    void Start () {
        // change state to menu to start game
        ChangeState(GameState.MainMenu);
        camManager = GetComponent<CameraManager>();
	}

    /// <summary>
    /// Handles user input
    /// <summary
    private void Update()
    {
        //Check if the player clicked escape in the Game
        if (Input.GetKeyDown(KeyCode.Escape) && currentGameState == GameState.Game)
        {
            SceneManager.LoadScene("Game");
        }
    }

    /// <summary>
    /// Resets from main menu to game
    /// </summary>
    public void ResetToMenu()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Change state to main menu
    /// </summary>
    public void ChangeToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    /// <summary>
    /// Change state to game
    /// </summary>
    public void ChangeToGame()
    {
        ChangeState(GameState.Game);
        camManager.ChangeCameraView(currentGameState);
    }

    /// <summary>
    /// Change state to beat mapper
    /// </summary>
    public void ChangeToBeatMapper()
    {
        ChangeState(GameState.BeatMapper);
    }

    /// <summary>
    /// Change state to beat mapper
    /// </summary>
    public void ChangeToOptions()
    {
        ChangeState(GameState.Options);
    }

    /// <summary>
    /// Change state to the passed in state and set every other state false
    /// </summary>
    public void ChangeState(GameState state)
    {
        // loop through all canvas and set them all to false
        for (int i = 0; i < statesUI.Count; i++)
        {
            statesUI[i].enabled = false;
        }
        // set canvas we want to true
        statesUI[(int)state].enabled = true;
        // set our current game state to the one passed in
        currentGameState = state;
        // check to see if new game state is game because that needs more attention
        if(currentGameState == GameState.Game)
        {
            GameObject.FindWithTag("Player").GetComponent<Player>().enabled = true;
            GameObject.FindWithTag("Manager").GetComponent<EnemySpawner>().enabled = true;
            GameObject.FindWithTag("SoundManager").GetComponent<BeatMapper>().currentlyMapping = false;
            GameObject.FindWithTag("Manager").GetComponent<GameManager>().PlayGame();
        }
        // check to see if new game state is main menu
        if(currentGameState == GameState.MainMenu)
        {
            GameObject.FindWithTag("SoundManager").GetComponent<BeatMapper>().currentlyMapping = false;
        }
    }

    /// <summary>
    /// Exits the game immediately
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}