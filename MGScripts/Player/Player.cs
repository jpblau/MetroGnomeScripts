using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main Player Class
/// Coded by Matthew Barry
/// @Contributors: Barrington Campbell
/// Controls player attacking and animations
/// Player dying, and time management(delays) for all the above
/// </summary>
public class Player : MonoBehaviour
{
    #region Variables
    // public object variables
    public Transform swordTransfrom; // transform of the sword
    public GameObject closestEnemy; // closest enemy in the game to the player
    public GameObject playerDestruct; // prefab of the player destructable
    public GameObject swordDestruct; // prefab of the sword destructable
    public TrailRenderer swipeRend; // holds the trail renderer for the player

    // public numeric variables
    public int hitCount = 0; // amount of notes the player has hit in a row
    public int comboCount = 0; // the number of combinations the player has hit
    public float swordDelay; // how long after a miss until player can swing again
    public float comboTimer; // how long we give the player to be able to hit a combo after sword swing complete
    public float hitDistance; // float for how far away player can reach
    public float voxelLife = 3f; // life that the destructed bolts will last

    // public boolean variables
    public bool comboEnabled = false; // if the player is in the middle of a combo attack

    // private object variables
    private GameManager gm; // game manager object reference
    private InputManager im; // input manager object reference
    private EnemySpawner es; // the EnemySpawner object reference
    private PlayerAnimControl pac;
    //private Animator anim; // component animation object reference

    // private numeric variables
    private int animTracker = 0; // tracks the current attack animation
    private float sinceLastSwing = 0; // time passed since last swing time by player
    private float swingDelayTimer = 0; // timer to incriment once a player misses a attack
    private float sinceLastKeyPressed = 0; // time since one of the last two player keys were pressed

    // private boolean variables
    private bool swinging = false; // if the player is swinging his sword
    private bool missed = false; // keeps track if the player missed a target
    
    //****
    public bool invulnerable = false; // Is the player currently using invulnerable mode?
    public Text invulnerableText; // Text for our invulnerable button-- TODO be removed
    //****

    // debugging attributes that will be deleted
    public GameObject body;
    #endregion

    /// <summary>
    /// Sets initial variables and components
    /// </summary>
    void Start()
    {
        // set component to attributes script
        gm = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        im = GameObject.FindWithTag("Manager").GetComponent<InputManager>();
        es = GameObject.FindGameObjectWithTag("Manager").GetComponent<EnemySpawner>();
        pac = GetComponent<PlayerAnimControl>();
        //anim = GetComponent<Animator>();
        // disable the swipe renderer and set animation speed
        //swipeRend.enabled = false;
        pac.Speed = 2;
        pac.SetState(1, false, false, true, false);
        //anim.speed = 5;
    }

    /// <summary>
    /// Handles user input and functionality
    /// </summary>
    void Update()
    {
        // incriment seconds once every frame *** 1 second passed every 60 frames ***
        if (swinging)
        {
            sinceLastSwing += Time.deltaTime;
            // check to see if combo timer has run out
            if(sinceLastSwing >= comboTimer)
            {
                // reset our stats back to idle 
                //animTracker = 0;
                //anim.SetInteger("attack", animTracker);
                sinceLastSwing = 0;
                //pac.SetState(1, false, false, true, false);
                swinging = false;
                comboEnabled = false;
                comboCount = 0;
            }
        }
        // if the player has missed, add to the delay
        if (missed)
        {
            swingDelayTimer += Time.deltaTime;
            // if the timer has reached the delay, reset the stats
            if (swingDelayTimer >= swordDelay)
            {
                missed = false;
                //animTracker = 0;
                //anim.SetInteger("attack", animTracker);
                pac.SetState(1, false, false, true, false);
                swingDelayTimer = 0;
            }
        }
        // if the player is able to use input and not in the middle of recovering from a missed target
        else
        {
            // if input manager has single input being true
            if(im.singleInput == true)
            {
                SingleInput();
            }
            // if input manager has double input being true
            else if(im.doubleInput == true)
            {
                DoubleInput();
            }
        }
        // some animation is being played that isn't idle
        //if(animTracker > 0)
        //{
        //    swipeRend.enabled = true;
        //}
        //// player isn't swinging at all
        //else
        //{
        //    swipeRend.enabled = false;
        //}
    }

    /// <summary>
    /// Called when player inputs one single key to attack
    /// </summary>
    public void SingleInput()
    {
        body.GetComponent<SkinnedMeshRenderer>().material.color = Color.red;
        if (swinging == false)
        {
            swinging = true;
            //animTracker = 1;
            //pac.SetState(1, true, false, false, false);
            Swing();
        }
        // player is swinging, let's check if there's a combo
        else
        {
            // if player swang in the combo timer zone, swing again but increase animation number and reset timer
            // to determine if a combo has been achieved
            if (sinceLastSwing <= comboTimer)
            {
                Swing();
                comboEnabled = true;
                //animTracker += 1;
                //// if animTracker is 5 or greater, we need to reset the the sequence to the beginning
                //if (animTracker >= 5)
                //{
                //    animTracker = 1;
                //}
                //pac.NextAttack();
            }
        }
    }

    /// <summary>
    /// Called when player inputs two keys at the same time to attack
    /// </summary>
    public void DoubleInput()
    {
        // for debugging purposes, turn the mesh blue
        body.GetComponent<SkinnedMeshRenderer>().material.color = Color.blue;
        // if not already swinging, start the first sequence of swings
        if (swinging == false)
        {
            swinging = true;
            animTracker = 1;
            Swing();
        }
        // player is swinging, let's check if there's a combo
        else
        {
            // if player swang in the combo timer zone, swing again but increase animation number and reset timer
            // to determine if a combo has been achieved
            if (sinceLastSwing <= comboTimer)
            {
                Swing();
                comboEnabled = true;
                animTracker += 1;
                // if animTracker is 5 or greater, we need to reset the the sequence to the beginning
                if (animTracker >= 5)
                {
                    animTracker = 1;
                }
            }
        }
    }

    /// <summary>
    /// Method to swing the sword
    /// </summary>
    public void Swing()
    {
        // reset the time since last swing
        sinceLastSwing = 0;
        // turn towards the enemy first *** will be the closest enemy ***
        if (closestEnemy != null && closestEnemy != gm.gameObject)
        {
            transform.LookAt(closestEnemy.transform);

            //Stops the player from rotating when hitting a bolt
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        }
        // make sure there is a closest enemy
        if (closestEnemy != null)
        {
            // get distance to closest enemy
            float distance = Vector3.Distance(this.transform.position, closestEnemy.transform.position);
            // if the distance is instead the hit radius we have set for the player
            if (distance <= hitDistance && closestEnemy != gm.gameObject)
            {
                // enemy is in range, good job player, kill the enemy (or do damage at least)
                closestEnemy.GetComponent<Enemy>().health -= 1;
                // get the distance difference between the player's range and how close the enemy was
                // *** Add the 1 for compensation since we'll round down, so a 9.1 is really a 10.1 which will be a 10
                float distanceDifference = (distance / hitDistance) * 10 + 1;
                distanceDifference -= (distanceDifference % 1f);
                gm.scoreAdded = distanceDifference;
                // show our enviromental effects for it
                
                gm.EnvironmentEffect();
                /*
                // to test camera shake mechanics, every 50 points
                if (gm.score % 50 == 0)
                {
                    gm.Shake(0.05f, 0.002f);
                }
                */
                //Debug.Log("Destroy: " + es.currentGameTime);
                ///anim.SetInteger("attack", animTracker);
                if(pac.InIdle == true) pac.SetState(1, true, false, false, false);
                else pac.NextAttack();
                // increase hit count and combo count if combo is enabled
                hitCount++;
                if(comboEnabled)
                {
                    comboCount++;
                }
            }
            // target out of player's range, call missed method
            else
            {
                Missed();
            }
        }
        // there are no enemies on the screen so miss anyway
        else
        {
            Missed();
        }
    }

    /// <summary>
    /// Method when player misses a swing
    /// </summary>
    public void Missed()
    {
        // we missed the player, set stats back to 0 and false
        //anim.SetInteger("attack", animTracker);
        //pac.NextAttack();
        pac.SetState(1, false, false, true, false);
        missed = true;
        swinging = false;
        sinceLastSwing = 0;
        hitCount = 0;
        comboCount = 0;
    }

    /// <summary>
    /// Method called when player dies
    /// </summary>
    public void Die()
    {
        // if we're not invulnerable, we can die
        if (!invulnerable)
        {
            // make player disinigrate into voxels
            GameObject sword = Instantiate(swordDestruct, new Vector3(swordTransfrom.position.x, swordTransfrom.position.y + 2.11f, swordTransfrom.position.z), swordTransfrom.rotation);
            GameObject bolt = Instantiate(playerDestruct, new Vector3(transform.position.x, transform.position.y + 2.11f, transform.position.z), transform.rotation);
            // destroy the real gameobjects so it's just the broken voxels
            Destroy(this.gameObject);
            Destroy(sword, voxelLife);
            Destroy(bolt, voxelLife);
        }
    }

    /// <summary>
    /// Toggle whether the player is invulnerable or not, and update the invulnerable button
    /// </summary>
    public void toggleInvuln()
    {
        invulnerable = !invulnerable;
        if (invulnerable)
            invulnerableText.text = "Invuln: True";
        else 
            invulnerableText.text = "Invuln: False";
        
    }
}
