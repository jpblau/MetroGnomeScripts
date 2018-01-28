using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Player Animation Control Class
/// @Author: Barrington Campbell
/// Handles the players animation state
/// </summary>
public class PlayerAnimControl : MonoBehaviour {
    #region Attributes
    Animator anim;

    public TrailRenderer swipeRend;
    public GameObject lantern;
    int state;
    bool inAttack;
    bool inMenu;
    bool inIdle;
    bool inSpecial;

    #endregion

    #region Properties
    public float Speed
    {
        get { return anim.speed; }
        set { anim.speed = value; }
    }

    public bool InIdle
    {
        get { return inIdle; }
    }
    #endregion
    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        //swipeRend = GetComponent<TrailRenderer>();        
        SetState(0, false, true, false, false); 
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    /// <summary>
    /// Sets the current animation state
    /// </summary>
    public void SetState(int state, bool inAttack, bool inMenu, bool inIdle, bool inSpecial)
    {
        this.state = state;
        this.inMenu = inMenu;
        this.inIdle = inIdle;
        this.inSpecial = inSpecial;
        anim.SetBool("InMenu", inMenu);
        anim.SetBool("InIdle", inIdle);
        anim.SetBool("InSpecial", inSpecial);
        anim.SetInteger("State", state);

        //Set swiperender state
        swipeRend.enabled = inAttack;

        if (inMenu == false)
            lantern.SetActive(false);
    }

    //Next Attack Scripts
    public void NextAttack()
    {
        //Debug.Log(state);
        inIdle = false;
        anim.SetBool("InIdle", false);
        if (state == 4) state = 1;
        else state++;
        //else if(state == 1) state = 2;
        //else if (state == 2) state = 3;
        //else if (state == 3) state = 4;
        //Debug.Log(anim.GetInteger("State") + " | " + anim.GetBool("InIdle"));
        anim.SetInteger("State", state);
    }

}
