using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemySpawner class
/// @Author: John Blau
/// @ Contributors: Matthew Barry
/// Spawns enemies into the scene based on beats of music
/// Also handles turning on the music and event syncing
/// </summary>
public class EnemySpawner : MonoBehaviour {

    #region Variables
    // public object variables
    public GameObject player;  // The player object in the scene
    public GameObject soundManager; // Get our sound manager game object, that controls audio in the scene

    // public numeric variables
    public int numEnemiesSpawned = 0; // The total number of enemies we have spawned this instance
    public float speed = 5f; // speed of the enemy spawned
    public float delayTimer; // delay timer is the amount of time the song should wait before playing
    public float currentGameTime = 0f; // The current game time- starts when the enemy spawner turns on and the first fixedUpdate is called

    // public boolean variables
    public bool gameOver = false; // controls if game is still being played or if it is over

    // public list variables
    public List<GameObject> enemy = new List<GameObject>(); // enemy gameobject prefab to use for this scene

    // private numeric variables
    private int positionInMap = -1; // Start at a position of -1 in our beatmap so when we increment we start off at index 0
    private float spawnOffset; // controls the offest of the spawn of the next enemy
    private float timeSinceLastSpawn; // The time since we last spawned in a bullet
    private float timeUntilNextBeat;    // The time until the next beat needs to spawn
    private float bps; // number of beats per second in a song read in using JSON
    
    private Vector3 pos; // the position around the circle

    // ************ NEWSTUFF **************
    private Vector3 center; // the center of the circle
    public float MAX_SPAWN_RADIUS = 25.0f;  // the maximum distance at which we spawn out enemies from us
    private List<BeatMap> listOfMaps;   // The list of all the maps we need to parse through


    // ************************************

    // private boolean variables
    private bool spawning = false; // true if we are spawning bullets, false if not
    private bool startedMusic; // have we started playing music yet

    // private list variables
    private List<float> beatMap; // get our beatmap, which is the player inputted beatMap

    // private component variables
    private GameManager gm; // the Game Manager script
    private BeatMapper beatMapper;  //our beatMapper script, which contains the player's beatMap
    #endregion

    /// <summary>
    /// Sets initial variables and components
    /// Reads in our mapped out beats
    /// </summary>
    public void Start()
    {
        // current amount of time passed since the game started
        currentGameTime = 0f;
        startedMusic = false;
        // reset the map
        positionInMap = -1;
        
        // Get to our beatMap 
        soundManager = GameObject.FindWithTag("SoundManager");
        beatMapper = soundManager.GetComponent<BeatMapper>();
        beatMap = beatMapper.listOfBeats;

        // set GameManager script
        gm = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player");


        CalculateDistance();

        //*********
        // SET OUR LISTS HERE
        listOfMaps = new List<BeatMap>();
        // if our left list is empty, then we have 1 button input
        if (beatMapper.listOfBeatsLeft.Count == 0)
        {
            // Fill it with a generic beat
            listOfMaps.Add(new global::BeatMap(beatMapper.listGenericMegalovania, true));
            delayTimer = Mathf.Abs(-spawnOffset + listOfMaps[0].untilNextSpawn);
            
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<MusicPicker>().SetMegalovania();
            
        }
        // otherwise, we have multi-button input
        else
        {
            // add both our left and right maps
            
            listOfMaps.Add(new BeatMap(beatMapper.listOfBeatsLeft, true));
            listOfMaps.Add(new BeatMap(beatMapper.listOfBeatsRight, false));

            // set our music delay timer to be the lesser of the two beatMap lists first spawns, 
            // and start our music after that amount of time
            if (listOfMaps[0].untilNextSpawn > listOfMaps[1].untilNextSpawn)
            {
                delayTimer = Mathf.Abs(-spawnOffset + listOfMaps[1].untilNextSpawn);
                
            }
            else
            {
                delayTimer = Mathf.Abs(-spawnOffset + listOfMaps[0].untilNextSpawn);
                
            }
            
        }

        StartCoroutine("StartMusic");

        
    }
	
	/// <summary>
    /// Incremenets our currentGameTime and starts music when needed based on the first enemy's spawn time
    /// </summary>
	void Update ()
    {
        

        foreach (BeatMap map in listOfMaps)
        {// Update our sinceLastSpawn
            
        }
    }

    /// <summary>
    /// Handles the Game Over case for this EnemySpawner. Stops spawning enemies, and deactivates the class
    /// </summary>
    private void HandleGameOver()
    {
        spawning = false;
        // Don't forget to reset our position in the map
        positionInMap = 0;
        this.enabled = false;
    }

    /// <summary>
    /// Calculates the distance from the player to the place where the enemies will be spawning
    /// Also calculates various timers essential to event timing
    /// </summary>
    public void CalculateDistance()
    {
        // calulations for the circle to spawn enemies
        center = player.transform.position;

        pos = RandomCircle(MAX_SPAWN_RADIUS, false);
        // get distance from player and radius. We're changing the player's hit distance here so that our player has a little leeway
        float distance = Vector3.Distance(player.transform.position, pos) - (player.GetComponent<Player>().hitDistance - (player.GetComponent<Player>().hitDistance * .05f) );

        // divide distance by speed
        // how long it takes the enemy to get to the player from spawnpoint
        spawnOffset = distance / speed;

        // negate value 
        spawnOffset = -spawnOffset;

        // Calculate our delay timer before the song should start
        delayTimer = Mathf.Abs((spawnOffset + 0.0001f) + beatMapper.timeBetweenStartOfSongAndFirstBeat);

        //gm.TurnTheLightsOff();
    }

    /// <summary>
    /// Spawns an enemy at a pseudo-random location around the player
    /// </summary>
    private void SpawnEnemy(float timeSinceShouldHaveSpawned, bool isLeft){
        int randomNum = 0;

        // calculate our offset distance based on the timeSinceShouldHaveSpawned
        float offsetDistance = MAX_SPAWN_RADIUS - (speed * timeSinceShouldHaveSpawned);

        // Pass that to RandomCircle, which also handles left or right
        Vector3 randPosition = RandomCircle(offsetDistance, isLeft);

        // Get a random number to pick what color the enemy will be
        randomNum = Random.Range(0, enemy.Count);

        // Spawn our new enemy
        GameObject newEnemy = Instantiate(enemy[randomNum], randPosition, Quaternion.identity);

        // set the quadrant of our newEnemy
        // if we're spawning this enemy on the right, change the quadrant
        if (!isLeft)
            newEnemy.GetComponent<Enemy>().quadrant = 2;

        // Add our newEnemy to our list of enemies
        gm.enemies.Add(newEnemy);
        numEnemiesSpawned++;

       
    }

    /// <summary>
    /// Generate a circle around the player and pick a random location which an enemy will spawn from
    /// </summary>
    /// <param name="radius">How far out from the player we are spawning the enemy</param>
    /// <param name="isLeft">Is this enemy supposed to be spawned on the left(true) or the right(false)?</param>
    /// <returns>A random position in a circle around the player</returns>
    private Vector3 RandomCircle(float radius, bool isLeft){
        float angle = 0f;

        // Check to see if we're left or right
        // if we're left, then set our rotation accordingly: 120f - 240f
        if (isLeft)
            angle = Random.Range(150f, 240f);
        // if we're right, set our rotation accordingly: 300f - 420f
        else
            angle = Random.Range(-60f, 60f);


        // calculate a new Position based on the radius and the angle
        Vector3 newPosition;
        newPosition.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        newPosition.y = 2;
        newPosition.z = center.z + radius * Mathf.Cos(angle * Mathf.Deg2Rad);

        //UnityEngine.Debug.Log(newPosition);

        return newPosition;
    }

    /// <summary>
    /// Spawns enemies at the right time. Used for a non-variable time 
    /// </summary>
    private void FixedUpdate(){
        // Check to see if the game is over
        if (gameOver)
            HandleGameOver();

        currentGameTime += Time.deltaTime;
        

        #region User Inputted Map
        // Loop through every map we need to look at
        for (int i = 0; i < listOfMaps.Count; i++)
        {
            // Set our map for this loop
            BeatMap map = listOfMaps[i];

            if (map.mapPos == map.beatMap.Count)
            {
                continue;
            }
            map.sinceLastSpawn += Time.deltaTime;

            // in the map, get the next beat and compare it to our sinceLastSpawn
            // if the sinceLastSpawn is > the nextBeat time,
            if (map.sinceLastSpawn >= map.untilNextSpawn)
            {
                // update our timeSinceLastSpawn to be (it - nextBeatTime)
                map.sinceLastSpawn -= map.untilNextSpawn;

                // if this isn't our offset beat or the end of the map
                if (map.mapPos != 0 && map.mapPos != map.beatMap.Count)
                {
                    // spawn in the beat(timeSinceLastSpawn, left)
                    SpawnEnemy(map.sinceLastSpawn, map.left);
                    
                }

                // Check to see if we just hit the last beat
                if (map.mapPos == map.beatMap.Count - 1)
                {
                    map.mapPos++;
                    continue;
                }
                // update our mapPos+1
                map.mapPos++;

                // update BeatMap's untilNextSpawn
                map.untilNextSpawn = map.beatMap[map.mapPos] - map.beatMap[map.mapPos-1];
                
            }
            // otherwise, we don't care   
        }
        #endregion

    }

    /// <summary>
    /// Starts the music after waiting the correct number of seconds
    /// Update and FixedUpdate wouldn't work for this, since we can't
    /// guarantee they will ever be called at exactly the right moment,
    /// and we can't offset the music
    /// </summary>
    /// <returns></returns>
    IEnumerator StartMusic(){
        // wait for our delay to be up, then start playing the music
        yield return new WaitForSecondsRealtime(delayTimer);
        gm.PlayAudio();
        startedMusic = true;
    }

}

#region Old Methods
/* Part of start()
        // Calculate our beats per minute
        bps = this.gameObject.GetComponent<MusicManager>().rootObj.beatsPerMinute / 60f;     

        // calculate the distance between the player and where the enemies spawn
        CalculateDistance();

        // If our beatMap doesn't have anything contained in it, we want to spawn based on beats per minute
        if (beatMap.Count <= 0)
        {
            timeUntilNextBeat = (1f / bps);
            spawning = true;
        }
        // Otherwise, we do want to spawn based on our beatMap input
        else
        {
            positionInMap = 0;
            timeUntilNextBeat = beatMap[positionInMap] - beatMapper.timeBetweenStartOfSongAndFirstBeat;
            timeSinceLastSpawn = 0f;
            spawning = true;
        } 
 * */

    /* Part of Update()
     * // starting music if not already started and delay timer has been reached
        if (!startedMusic && currentGameTime > delayTimer)
        {
            gm.PlayAudio();
            startedMusic = true;
        }
     * 
     * 
     * */

/*/// <summary>
    /// Spawns in the enemies at the right time
    /// </summary>
    private void FixedUpdate()
    {

        timeSinceLastSpawn += Time.deltaTime;

        #region old
        // if we're not spawning yet, calculate the time until our next beat once the song starts
        #region first time spawn calculation
        if (gm.enemies.Count == 0)
        {
            // if we have a user generated beatmap
            if (beatMap.Count != 0)
            {
                positionInMap = 0;
                //timeUntilNextBeat = timeSinceLastSpawn - beatMap[positionInMap];
                timeUntilNextBeat = beatMap[positionInMap]; //+ spawnOffset;  // This will change later
                UnityEngine.Debug.Log("asdfasdfaeefgjghoi" + spawnOffset);
            }
            // if we don't have user input
            else
            {
                 timeUntilNextBeat = (1f / bps);// This will not change later
                UnityEngine.Debug.Log(timeUntilNextBeat);
            }
        }
        #endregion

        // if our game hasn't ended yet and we've started spawning bullets
        if ((gameOver == false) && spawning)
        {         
            // if the time that has passed is greater than the time we have until the next beat spawns in 
            if ((timeSinceLastSpawn) >= timeUntilNextBeat)
            {
                // let's spawn in our bullet
                SpawnEnemy();
                // don't need to recalculate beats per second enemies
                if ((beatMap.Count != 0) && (positionInMap < beatMap.Count))
                {
                    positionInMap++;

                    timeSinceLastSpawn = (timeSinceLastSpawn - timeUntilNextBeat);
                    timeUntilNextBeat = beatMap[positionInMap] - beatMap[positionInMap - 1];
                }
                else
                {
                    timeSinceLastSpawn = timeSinceLastSpawn - timeUntilNextBeat;
                }
            }
        }
        // if gameover = true
        else if (gameOver)
        {
            HandleGameOver();
        }
    }*/

/*/// <summary>
/// Spawns an enemy at a random location around the player
/// </summary>
private void SpawnEnemy()
{
        Quaternion rot = Quaternion.identity;
        int randomNum = 0;

        // offset distance based on how long it has been since we should have spawned in the bullet
        float offsetDistance = speed * (timeSinceLastSpawn - timeUntilNextBeat);
        float maxCircleDistance = 25.0f;

        // calulations for the circle to spawn enemies
        pos = RandomCircle(center, maxCircleDistance - offsetDistance);

        // Get a random number to pick what color the enemy will be
        randomNum = Random.Range(0, enemy.Count);

        // spawn the enemy in the correct location
        GameObject newEnemy = Instantiate(enemy[randomNum], pos, rot);

        //newEnemy.GetComponent<Enemy>().speed = speed;
        // add them to the GM list
        gm.enemies.Add(newEnemy);

    numEnemiesSpawned++;
    //UnityEngine.Debug.Log("Spawned Enemy at: " + (currentGameTime - spawnOffset - (offsetDistance / speed)));
}*/

/*/// <summary>
/// Generate a circle around the player and pick a random location which an enemy will spawn from
/// </summary>
/// <param name="center">The player's position</param>
/// <param name="radius">The radius of the circle we want to draw</param>
/// <returns>A random position in a circle around the player</returns>
Vector3 RandomCircle(Vector3 center, float radius)
{
    float ang = Random.value * 360;
    Vector3 pos;
    pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
    pos.y = 1;
    pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
    return pos;
}*/
#endregion

#region Decommissioned Coroutines. Here just for archival purposes. Will be removed.
/*/// <summary>
/// This is our method for spawning beats based on the user input
/// </summary>
/// <returns></returns>
public IEnumerator SpawnBeatInput()
{
    for (int index = 0; index < beatMap.Count; index++)
    {
        float nextEnemy;
        positionInMap++;
        // Check to see if this is our first time spawning something or not
        if (positionInMap == 0)
        {
            nextEnemy = beatMap[positionInMap] + spawnOffset;
        }
        else   // if its not, calculate the time to wait before the next beat spawn
        {
            nextEnemy = beatMap[positionInMap] - beatMap[positionInMap - 1];
        }

        //Debug.Log(nextEnemy - currentGameTime);

        // wait the required number of seconds
        yield return new WaitForSecondsRealtime(nextEnemy);
        // if our game isn't over
        if (gameOver == false)
        {
            // calulations for the circle to spawn enemies
            center = transform.position;
            pos = RandomCircle(center, 25.0f);
            Quaternion rot = Quaternion.identity;

            // Get a random number to pick what color the enemy will be
            int randomNum = Random.Range(0, enemy.Count);

            // spawn the enemy in the correct location
            GameObject newEnemy = Instantiate(enemy[randomNum], pos, rot);

            //newEnemy.GetComponent<Enemy>().speed = speed;
            // add them to the GM list
            gm.enemies.Add(newEnemy);
        }
    }
}

/// <summary>
/// Spawns in enemies based on json data
/// </summary>
/// <returns></returns>
public IEnumerator SpawnBeat()
{
    //things taken out of while loop
    center = transform.position;
    Quaternion rot = Quaternion.identity;
    int randomNum = 0;
    //

    Stopwatch timer = new Stopwatch();
    timer.Start();
    long start=0;
    long stop=0;
    float waitTime = 1000f / bps;
    float lastGameTime = 0f;

    // use an infinite loop to keep beats spawning on the same interval;
    start = timer.ElapsedMilliseconds;
    while (true)
    {
        //UnityEngine.Debug.Log(currentGameTime - lastGameTime);
        //lastGameTime = currentGameTime;
        //yield return new WaitForSecondsRealtime(waitTime - (stop-start)/1000f);
        if (timer.ElapsedMilliseconds - start < waitTime)
            continue;
        start = timer.ElapsedMilliseconds;
        yield return new WaitForSecondsRealtime(0.000001f);
        if (gameOver == false)
        {
            // calulations for the circle to spawn enemies

            pos = RandomCircle(center, 25.0f);


            // Get a random number to pick what color the enemy will be
            randomNum = Random.Range(0, enemy.Count);

            // spawn the enemy in the correct location
            GameObject newEnemy = Instantiate(enemy[randomNum], pos, rot);

            //newEnemy.GetComponent<Enemy>().speed = speed;
            // add them to the GM list
            gm.enemies.Add(newEnemy);
        }
        // if gameOver == true;
        else
        {
            // Stop our coroutine so that we can call it again on play game
            StopCoroutine(spawner);
        }
//            stop = timer.ElapsedMilliseconds;
//           UnityEngine.Debug.Log(stop-start);

    }
}

    public IEnumerator DelaySongStart()
{
    //UnityEngine.Debug.Log("Going to wait for " + delayTimer);
    yield return new WaitForSecondsRealtime(delayTimer);
    spawning = true;
    UnityEngine.Debug.Log("Now Spawning" + spawnOffset);

}
*/
#endregion