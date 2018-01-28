using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Score Text Class
/// Coded by Matthew Barry
/// Handles score text functionality with fading effect
/// Also handles the scrolling down of the text effect
/// </summary>
public class ScoringText : MonoBehaviour {

    #region Variables
    // public object variables
    public Text scoreText;
    // public numeric variables
    public float fadeDuration = 2;
    public float speed = 2;
    // private object variables
    private Color c;
    // private numberic variables
    private float fadeSpeed = 0;
    private float sinceLastFrame = 0;
    private float t;
    #endregion

    /// <summary>
    /// Set the color of the text and fade speed variable
    /// </summary>
    void Start ()
    {
        scoreText = this.GetComponent<Text>();
        fadeSpeed = 1 / fadeDuration;
        c = scoreText.color;
	}

    /// <summary>
    /// Move the text down as well as fade the alpha value per delta time
    /// </summary>
    void FixedUpdate()
    {
        // have the text move down the screen and fade
        this.transform.Translate(Vector3.down * Time.deltaTime * speed);
        t += Time.deltaTime * fadeSpeed;
        c.a = Mathf.Lerp(1, 0, t);
        scoreText.color = c;
        // if the alpha value is completely gone, destroy the text
        if(c.a <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
