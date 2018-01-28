using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the cameras for the game
/// Written by Barrington Campbell
/// </summary>
public class CameraManager : MonoBehaviour {

    #region Attributes
    [SerializeField]
    Vector3 menuCameraPosition, gameCemeraPostion;  //Different Camera positions (Menu - Main Menu | Game - In Game)

    [SerializeField]
    float transitionTime;

    bool transition;    //Tells if there is a camera transition currently going on
    #endregion


    private void Update()
    {
        if(transition == true)
        {
            //Lerp throught the point of view
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 12, 0.02f);

            //Lerp the position of the camera
            float cameraX = Mathf.Lerp(Camera.main.transform.position.x, gameCemeraPostion.x, 0.02f);
            float cameraY = Mathf.Lerp(Camera.main.transform.position.y, gameCemeraPostion.y, 0.02f);
            float cameraZ = Mathf.Lerp(Camera.main.transform.position.z, gameCemeraPostion.z, 0.02f);
            Camera.main.transform.position = new Vector3(cameraX, cameraY, cameraZ);

            //Stop the transition once the camera is positioned correctly
            if (Camera.main.orthographicSize >= 12 || Camera.main.orthographicSize <= 3)
                transition = false;
        }
    }

    /// <summary>
    /// Changes the main cameras field of view when switching in between game states
    /// </summary>
    public void ChangeCameraView(StateManager.GameState state)
    {
        if (state == StateManager.GameState.Game)
        {
            transition = true;
        }

        else if (state == StateManager.GameState.MainMenu)
        {
            transition = true;
        }
    }
}
