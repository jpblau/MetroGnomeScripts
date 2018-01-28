using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy Class
/// @Author: Matthew Barry
/// @Contributors: John Blau
/// Used to control AI to make them move towards player
/// Detects if the enemy has reached the player
/// </summary>
public class Enemy : MonoBehaviour {

    #region Variables
    // public object variables
    public GameObject player; // gameobject for the player prefab
    public GameObject playerWaypoint; // gameobject for the player prefab
    public GameObject enemyWaypoint; // the waypoint of the enemy
    public GameObject boltDestruct; // prefab of the dead bolt
    public GameObject boltDestructTablet; // Prefab for the dead bolt on a tablet TODO find a better solution for this
    public GameObject body; // body of the enemy that displays its mesh
    public GameObject spotlight; // the spotlight above the enemy
    public GameObject projector; // the projector below the enemy 
    public Color currentColor; // current color of our enemy
    public Material projectorMaterial; // material of the projector
    public Gradient colorGradient; // gradient for the color of the enemy

    // public numeric variables
    public int quadrant; // The quadrant that our enemy is active in. 1 is left / default, 2 is right
    public float speed = 5f; // how fast the enemy moves
    public float gradientIncrease; // how fast the gradient on the bolt will change
    public float health; // how many hits the enemy can take
    public float distanceToPlayer; // distance from this enemy to the player
    public float distanceToRadius; // distance from this enemy to the edge of the player's radius
    public float disappearTimer; // timer until enemy disappears
    public float voxelLife = 4f; // life that the destructed bolts will last
    public float DESTRUCTION_FORCE; // The force with which we push away the enemy on death. Should be around 10000

    // public boolean variables
    public bool gameOver; // check to see if game is over

    // public array variables
    public List<Gradient> colorGradients = new List<Gradient>(); // gradients for the colors of the enemy
    public List<Material> projectorMaterials; // materials of the projector
    public List<GameObject> pieces = new List<GameObject>(); // the pieces that will fall off the enemy

    // private object variables
    private GameManager gm; // game manager component
    private EnemySpawner es; // the enemy spawner, for keeping track of the current game time

    // private numberic variables
    private float sinceLastFrame = 0; // keeps track of how much time has passed since last frame
    private float startingDistanceToPlayerRadius; // how far the enemy has to go from spawn to reach the player's radius
    private float FORCE_VARIANCE = 5000f; // The variance in the amount of force we are applying to a destroyed enemy. Total force = destruction_force +- force_variance

    // private boolean variables
    private bool dead; // check if enemy is dead or not
    #endregion

    /// <summary>
    /// Sets initial variables and components
    /// </summary>
    void Start ()
    {
        // set player object, needs to have correct tag
        player = GameObject.FindWithTag("Player");
        playerWaypoint = GameObject.FindWithTag("PlayerWaypoint");
        // set component script
        gm = GameObject.FindWithTag("Manager").GetComponent<GameManager>();
        es = GameObject.FindWithTag("Manager").GetComponent<EnemySpawner>();
        // have bolt face player
        transform.LookAt(GameObject.FindGameObjectWithTag("PlayerWaypoint").transform.position);

        // Set our default quadrant to 1
        this.quadrant = 1;

        // set our speed to be the speed of the number set in EnemySpawner
        speed = es.speed;
        // get distance for player and player's radius from start
        distanceToPlayer = Vector3.Distance(playerWaypoint.transform.position, enemyWaypoint.transform.position);
        startingDistanceToPlayerRadius = distanceToPlayer - player.GetComponent<Player>().hitDistance;
        // set what color this enemy will be
        int colorNum;
        colorNum = Random.Range(0, colorGradients.Count);
        colorGradient = colorGradients[colorNum];
        projectorMaterial = projectorMaterials[colorNum];
        projector.GetComponent<Projector>().material = projectorMaterial;
    }

    /// <summary>
    /// Keeps track of enemy health and functionality based on that value
    /// </summary>
    void Update ()
    {
        // if the enemy has some health left and player still exist
        if (health > 0 && player != null)
        {
            // move towards the player's position
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerWaypoint.transform.position, step);
            // check to see how far away player is from this enemy
            CheckDistance();
        }
        // player doesn't exist or no health remaining
        else
        {
            // if not dead already, become dead
            if (!dead)
            {
                // destruct the bolt
                Destruct();
                // die();
                dead = true;
            }
        }
    }

    /// <summary>
    /// Checks to see if enemy has reached player
    /// </summary>
    public void CheckDistance()
    {
        // get distance
        distanceToPlayer = Vector3.Distance(playerWaypoint.transform.position, enemyWaypoint.transform.position);
        distanceToRadius = distanceToPlayer - player.GetComponent<Player>().hitDistance;

        // change color of bolt as it get's closer to player's radius
        if (distanceToRadius >= 0.01f)
        {
            // update the enemy's color based on gradient values
            float gradientPrecentage = distanceToRadius / startingDistanceToPlayerRadius;
            currentColor = colorGradient.Evaluate(gradientPrecentage);
            //float glowPrecentage = (0.5f * (1 - gradientPrecentage)) + 0.5f;
            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].GetComponent<MeshRenderer>().material.color = currentColor; // update the enemy's color based on gradient values
            }
            body.GetComponent<MeshRenderer>().material.color = currentColor; // update the enemy's color based on gradient values
            spotlight.GetComponent<Light>().color = currentColor;
            //body.GetComponent<MeshRenderer>().material.SetFloat("_GlowIntensity", glowPrecentage);

            //body.GetComponent<MeshRenderer>().material.SetFloat("_Metallic", body.GetComponent<MeshRenderer>().material.GetFloat("_Metallic") - gradientIncrease);
        }
        // check to see if enemy has reached player's hit radius for a perfect hit
        else
        {
            // update the enemy's color based on gradient values
            float gradientPrecentage = (Mathf.Abs(distanceToRadius) / (player.GetComponent<Player>().hitDistance));
            currentColor = colorGradient.Evaluate(gradientPrecentage);

            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].GetComponent<MeshRenderer>().material.color = currentColor; // update the enemy's color based on gradient values
            }
            if (pieces.Count > 0)
            {
                int blockNum = Random.Range(0, pieces.Count - 1);
                pieces[blockNum].GetComponent<Rigidbody>().isKinematic = false;
                pieces[blockNum].GetComponent<Rigidbody>().useGravity = true;
                pieces[blockNum].transform.SetParent(null);
                //pieces[blockNum].GetComponent<MeshRenderer>().material = glowMaterial;
                //float glowPrecentage = (1.5f * (1 - gradientPrecentage));
                //pieces[blockNum].GetComponent<MeshRenderer>().material.SetFloat("_GlowIntensity", glowPrecentage);
                Destroy(pieces[blockNum], 1f);
                pieces.Remove(pieces[blockNum]);
            }

            body.GetComponent<MeshRenderer>().material.color = currentColor; // update the enemy's color based on gradient values
            spotlight.GetComponent<Light>().color = currentColor;
            //body.GetComponent<MeshRenderer>().material = glowMaterial;
            //body.GetComponent<MeshRenderer>().material.SetFloat("_GlowIntensity", 0.75f);
        }

        if (distanceToPlayer <= 1.2f)
        {
            // enemy has reached player...RIP
            Destroy(this.gameObject);
            gm.GameOver();
        }
    }

    /// <summary>
    /// Kill the enemy and start disappear timer
    /// </summary>
    public void Die()
    {
        if (gameOver == false)
        {
            gm.score += 1;
        }
        // remove from the list
        gm.enemies.Remove(this.gameObject);
        //Debug.Log("Destroy: " + es.currentGameTime);
        // change mat and start timer to go away
        Disappear();
    }

    /// <summary>
    /// Destruct the bolt
    /// </summary>
    void Destruct()
    {
        GameObject bolt;
        // For the moment, we only want to instantiate the tablet boltDestruct if we're in tablet mode.
        if (gm.MOBILEGAME)
        {
            bolt = Instantiate(boltDestructTablet, transform.position, transform.rotation);
        }
        else
        {
            bolt = Instantiate(boltDestruct, transform.position, transform.rotation);
        }
        int index = 0;
        foreach (Transform child in bolt.transform)
        {
            if(index == 27)
            {
                child.GetComponent<Light>().color = currentColor;
            }
            child.GetComponent<MeshRenderer>().material.color = currentColor;
            index++;
        }
        DestructForce(bolt);
        Destroy(bolt, voxelLife);
        Destroy(gameObject);
    }

    /// <summary>
    /// Adds forces to the destruction blocks associated with boltDestruct so that an enemy flies all over the map when it is destroyed
    /// </summary>
    /// <param name="bolt"></param>
    private void DestructForce(GameObject bolt)
    {
        Transform boltTransform = bolt.transform;

        int numChildren = boltTransform.childCount;
        Vector3 awayVector = Vector3.zero;

        Vector3 boltPosition = boltTransform.position;
        Vector3 playerPosition = player.transform.position;

        // Get the total force that we will be applying to the blocks
        float totalForce = DESTRUCTION_FORCE + (Random.Range(-FORCE_VARIANCE, FORCE_VARIANCE));

        // Loop through every block in our bolt
        for (int i = 0; i < numChildren; i++)
        {
            // Only add forces to every other block in the bolt
            if (i%2 == 0)
            {
                // Get a vector that points in the opposite direction of our current path
                awayVector = -(playerPosition - boltPosition);
                // Scale our awayVector by our totalForce and add it to the block
                boltTransform.GetChild(i).GetComponent<Rigidbody>().AddForce(awayVector * totalForce);
            }
        }
    }

    /// <summary>
    /// Delay by disappearTimer and then destroy this gameobject
    /// </summary>
    public void Disappear()
    {
        // while timer is still below our restart time limit
        while (sinceLastFrame <= disappearTimer)
        {
            // adds time since last frame to the float
            sinceLastFrame += Time.deltaTime;
            if(sinceLastFrame >= disappearTimer)
            {
                // timer has been reached, destroy this gameobject
                Destroy(this.gameObject);
            }
        }
    }
}