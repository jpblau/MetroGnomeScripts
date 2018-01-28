using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Main Game Manager Class
/// @Author: Matthew Barry
/// @Contributors: John Blau
/// Handles keeping track of the objects within the scene
/// Gets closest enemy for player class
/// Handles GameOver and RestartGame functions
/// </summary>
public class GameManager : MonoBehaviour {

    #region Variables
    // public object variables
    public Camera playerCamera; // the camera for the game
    public GameObject player; // spawned object of the player
    public GameObject playerWaypoint; // spawned object of the player
    public GameObject playerPrefab; // prefab for the player gameobject
    public GameObject spawnpoint; // player respawn point
    public GameObject closestEnemyToPlayer; // closest enemy to the player
    public GameObject boltDestruct; // prefab of the dead bolt
    public GameObject soundManager; // gameobject for the sound manager
    public GameObject scoreSpawnpoint; // spawnpoint for the score text
    public Projector outerLeftProjector; // The outer left projector's projector
    public Projector outerRightProjector; // The outer right projector's projector
    public Projector innerProjector; // The inner projector's projector
    public Material M_INCOMING_ENEMY; // the material for projections when the next enemy is coming in from that side of the screen
    public Material M_NO_ENEMY; // the material for projections when the next enemy is not on that side of the screen
    public Canvas gameCanvas; // prefab for the canvas
    public Text scoreText; // score text
    public Button deathMainMenu; // button for the main menu
    public Gradient projectorColor; // gradient for the inner projector for the player's radius
    public AudioClip mainTheme; // audio for our main menu theme

    // public numeric variables
    public float distanceToPlayer; // distance from the closest enemy to the player
    public float distanceToPlayerRadius; // distance from the closest enemy to the player's hit distance projector
    public float score; // score the player has achieved 
    public float restartTimer; // how long until we load the game back up
    public float scoreAdded = 0; // the new score that will be added to overall score

    // public boolean variables
    public bool DESKTOPGAME = false; // if game is being played on desktop
    public bool MOBILEGAME = false; // if game is being played on a mobile device

    // public array variables
    public List<GameObject> enemies = new List<GameObject>(); // all enemies in the game

    // private object variables
    private Vector3 originPosition; // original position of camera
    private Vector3 BOLT_DESTRUCT_IPOS = new Vector3(-50f, 0f, -50f); // Each bolt destruct's initial position
    private Quaternion originRotation; // original rotatation of camera
    private Player playerScript; // player script reference object
    private EnemySpawner enemySpawner; // enemy spawner reference object
    private BeatMapper beatMapper; // beat mapper reference object 
    private AudioSource audioSource; // audio source reference object


    // private numberic variables
    private float sinceLastFrame = 0; // keeps track of how much time has passed since last frame
    private float shake_decay = 0; // how much to decrease the camera shake by
    private float shake_intensity = 0; // how much to make the camera shake in that frame

    // private boolean variables
    private bool gameOver = false; // if game has ended
    #endregion

    private static GameManager dontDestroy; // Ensures that there is only one instance of this gameObject

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

    /// <summary>
    /// Sets initial variables and components
    /// </summary>
    void Start ()
    {
        // set beat mapper component
        beatMapper = soundManager.GetComponent<BeatMapper>();
        // keeping track of all objects in the game
        enemySpawner = GetComponent<EnemySpawner>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerWaypoint = GameObject.FindGameObjectWithTag("PlayerWaypoint");
        playerScript = player.GetComponent<Player>();
        // set audio source component
        audioSource = soundManager.GetComponent<AudioSource>();
        audioSource.clip = mainTheme;
        audioSource.Play();

        // Make sure our projections are white to start
        outerLeftProjector.material.color = M_NO_ENEMY.color;
        outerRightProjector.material.color = M_NO_ENEMY.color;
        innerProjector.material.color = M_NO_ENEMY.color;

        // Spawn in our initial enemy destruction prefabs
        SpawnInitialEnemyDestructs();

        // detect what platform we are playing on
        #if UNITY_STANDALONE
            DESKTOPGAME = true;
#endif
#if UNITY_IOS
            MOBILEGAME = true;
#endif
#if UNITY_ANDROID
            MOBILEGAME = true;
#endif

        // effect for the menu of the game
        /*
        GameObject[] lights = GameObject.FindGameObjectsWithTag("AvailableLight");
        if (lights.Length > 0)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].GetComponent<LightFlicker>().myLight = lights[i].GetComponent<Light>();
                lights[i].GetComponent<LightFlicker>().flash = true;
                lights[i].GetComponent<LightFlicker>().totalSeconds = 5;
                lights[i].GetComponent<LightFlicker>().intensityStep = lights[i].GetComponent<LightFlicker>().maxIntensity / 60;
                lights[i].GetComponent<LightFlicker>().intensityStep = lights[i].GetComponent<LightFlicker>().intensityStep / (lights[i].GetComponent<LightFlicker>().totalSeconds / 2);
                lights[i].GetComponent<LightFlicker>().myLight.enabled = true;
                int randomColorNum = Random.Range(0, lights[i].GetComponent<LightFlicker>().colors.Count);
                lights[i].GetComponent<LightFlicker>().myLight.color = lights[i].GetComponent<LightFlicker>().colors[randomColorNum];
                lights[i].GetComponent<LightFlicker>().repeat = true;
                float randomDelay = Random.Range(0, 2.5f);
                lights[i].GetComponent<LightFlicker>().beginningDelay = randomDelay;
            }
        }
        */
    }

    /// <summary>
    /// Checks to see where closest enemy is as many times a second as possible
    /// </summary>
    public void Update()
    {
        if (gameOver == false)
        {
            // get the closest enemy from our list and give it to the player
            GetClosestEnemy();

            // indicate, using projections, which side the closest enemy is coming in from.
            // If our closest enemy isn't null
            if (playerScript.closestEnemy != null)
            {
                ClosestEnemyIndicator();
            }
            // Check to see if we need to generate an inner circle because an enemy is within the player's radius
            InnerCircle();
        }
    }

    /// <summary>
    /// Checks to see if game has ended or not
    /// </summary>
    void FixedUpdate ()
    {
        // if the game is not over
        if (gameOver == false)
        {
            // call methods
            if(shake_intensity > 0)
            {
                playerCamera.transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
                playerCamera.transform.rotation = new Quaternion(
                originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .2f,
                originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .2f);
                shake_intensity -= shake_decay;
            }
        }
        // game is over, work on restarting it for player
        else
        {
            // while timer is still below our restart time limit
            if (sinceLastFrame <= restartTimer)
            {
                // adds time since last frame to the float
                sinceLastFrame += Time.deltaTime;
                if(sinceLastFrame >= restartTimer)
                {
                    // timer has been reached, restart the game by resetting values
                    Debug.Log(sinceLastFrame);
                    GameObject player = Instantiate(playerPrefab, spawnpoint.transform.position, spawnpoint.transform.rotation);
                    player.GetComponent<Player>().enabled = true;
                    score = 0;
                    sinceLastFrame = 0;
                    gameOver = false;
                    enemySpawner.enabled = true;
                    enemySpawner.gameOver = false;
                    mainTheme = soundManager.GetComponent<AudioSource>().clip;
                    this.Start();
                    enemySpawner.Start();
                    PlayGame();
                }
            }
        }
	}

    /// <summary>
    /// Gets the closest enemy to the player
    /// </summary>
    public void GetClosestEnemy()
    {
        if (enemies.Count > 0)
        {
            float closestDistance = 999; // temporary storage of the closest distance found so far in the list
            GameObject closestEnemy = this.gameObject; // temporary storage of the closest enemy in the game

            // go through every enemy and find closest one to the player
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    float distance = Vector3.Distance(playerWaypoint.transform.position, enemy.GetComponent<Enemy>().enemyWaypoint.transform.position);

                    if (distance < closestDistance)
                    {
                        // reset distance and enemy since this object is closer
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }
            // go tell the player which enemy is the closest
            closestEnemyToPlayer = closestEnemy;
            distanceToPlayer = closestDistance;
            distanceToPlayerRadius = closestDistance - playerScript.hitDistance;
            playerScript.closestEnemy = closestEnemy;
        }
    }

    /// <summary>
    /// Changes the outer projection on the left or right side of the player to a different color if the next enemy 
    /// is coming from that side
    /// </summary>
    private void ClosestEnemyIndicator()
    {
        // get the position of that enemy
        Vector3 enemyPos = closestEnemyToPlayer.transform.position;
        // get the position of the player
        Vector3 playerPos = player.transform.position;
        // get the side the enemy is coming from 
        // if the enemy is coming from the right
        if (enemyPos.z > playerPos.z)
        {
            outerRightProjector.material.color = closestEnemyToPlayer.GetComponent<Enemy>().body.GetComponent<MeshRenderer>().material.color;
            //outerLeftProjector.material.color = M_NO_ENEMY.color;
        }
        // else, if the enemy is coming from the left
        else if (enemyPos.z < playerPos.z)
        {
            outerLeftProjector.material.color = closestEnemyToPlayer.GetComponent<Enemy>().body.GetComponent<MeshRenderer>().material.color;
            //outerRightProjector.material.color = M_NO_ENEMY.color;
        }
    }

    /// <summary>
    /// Creates an inner circle that decreases in radius based on how close an enemy is to the player. 
    /// </summary>
    private void InnerCircle()
    {
        // check to see if the closest enemy is within the player's hit radius
        if (distanceToPlayerRadius < 0.01f)
        {
            // is is, condense the project to match
            innerProjector.GetComponent<Projector>().orthographicSize = distanceToPlayer;
            // calculate and set the alpha value for the gradient of the projector
            float gradientPrecentage = distanceToPlayer / outerLeftProjector.orthographicSize;
            innerProjector.GetComponent<Projector>().material.color = projectorColor.Evaluate(gradientPrecentage);
        }
        // it's not, keep project at outer project radius
        else
        {
            innerProjector.GetComponent<Projector>().orthographicSize = 0;
        }
    }

    /// <summary>
    /// Method to activate certain characteristics of the enviromnent
    /// </summary>
    public void EnvironmentEffect()
    {
        Text spawnedText = Instantiate(scoreText, scoreSpawnpoint.transform.position, scoreSpawnpoint.transform.rotation);
        spawnedText.text = scoreAdded.ToString();
        score += scoreAdded;
        spawnedText.transform.SetParent(gameCanvas.transform, true);
        // Loop through and turn off all the lights in the scene
        /*
        GameObject[] lights = GameObject.FindGameObjectsWithTag("AvailableLight");
        if (lights.Length > 0)
        {
            // turn the light's flash on, give it a random color and enable it
            int randomLightnum = Random.Range(0, lights.Length);
            lights[randomLightnum].GetComponent<LightFlicker>().flash = true;
            lights[randomLightnum].GetComponent<LightFlicker>().myLight.enabled = true;
            int randomColorNum = Random.Range(0, lights[randomLightnum].GetComponent<LightFlicker>().colors.Count);
            lights[randomLightnum].GetComponent<LightFlicker>().myLight.color = lights[randomLightnum].GetComponent<LightFlicker>().colors[randomColorNum];
        }
        */
    }

    /// <summary>
    /// Sets player's camera original position, rotation, and variables for the future camera shake
    /// </summary>
    public void Shake(float si, float sd)
    {
        originPosition = playerCamera.transform.position;
        originRotation = playerCamera.transform.rotation;
        shake_intensity = si;
        shake_decay = sd;
    }

    /// <summary>
    /// Called when enemy has reached player to end game
    /// </summary>
    public void GameOver()
    {
        // only handle game over if the player is not invulnerable
        if (!player.GetComponent<Player>().invulnerable)
        {
            enemySpawner.gameOver = true;
            gameOver = true;
            playerScript.Die();
            RestartGame();
        }
    }

    /// <summary>
    /// Restarts game
    /// </summary>
    /// <returns></returns>
    public void RestartGame()
    {
        // restart the game
        // first make sure there are enemies in our list
        if (enemies.Count > 0)
        {
            // make a temporary list of enemies to destroy to prepare to make a deep copy
            List<GameObject> tempList = new List<GameObject>();
            // to make a deep copy of the list
            foreach (GameObject enemy in enemies)
            {
                if (enemy != null)
                {
                    // add to temporary list
                    tempList.Add(enemy);
                }
            }
            // now to destroy the original list of enemies
            foreach (GameObject enemy in tempList)
            {
                enemy.GetComponent<Enemy>().gameOver = true;
                enemy.GetComponent<Enemy>().Die();
            }
        }
    }

    /// <summary>
    /// Stops the audio source
    /// </summary>
    public void PlayGame()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// Starts the audio source
    /// </summary>
    public void PlayAudio()
    {
        audioSource.Play();
    }

    /// <summary>
    /// Handles lights turning off
    /// </summary>
    /*
    public void TurnTheLightsOff()
    {
        // Loop through and turn off all the lights in the scene
        GameObject[] lights = GameObject.FindGameObjectsWithTag("FlickeringLight");
        foreach (GameObject go in lights)
        {
            go.GetComponent<LightFlicker>().repeat = false;
        }
    }
    */
    /// <summary>
    /// Spawns in 20 instances of enemyDestruct at the beginning of the game, so that we don't have to instantiate them at run time
    /// </summary>
    public void SpawnInitialEnemyDestructs()
    {

        for (int i = 0; i < 20; i++)
        {
            //GameObject bolt = Instantiate(boltDestruct, BOLT_DESTRUCT_IPOS, transform.rotation);
            /*foreach (BoxCollider child in bolt.GetComponentsInChildren(BoxCollider))
            {

            }*/
        }
    }

    #region OldDelayCode
    /*
    public IEnumerator DelaySongStart()
    {
        yield return new WaitForSecondsRealtime(Mathf.Abs(enemySpawner.delayTimer + beatMapper.timeBetweenStartOfSongAndFirstBeat));
        audioSource.Play();
    }
   */
    #endregion
}