using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
/// <summary>
/// Slider Handler Script
/// Coded by Barrington Campbell
/// Hanldes all the necessary gameobjects to be manipulated
/// </summary>
public class SliderHandler : MonoBehaviour
{
    #region Attributes
    [SerializeField] GameObject leftHandle; //Left slider handle 
    [SerializeField] GameObject rightHandle;//Right slider handle  
    [SerializeField] GameObject background; //Slider background 
    Vector2 leftStartPos;                   //Starting position of the left slider handle
    Vector2 rightStartPos;                  //Starting position of the right slider handle
    #endregion

    #region Properties
    public GameObject LeftHandle
    {
        get { return leftHandle; }
    }
    public GameObject RightHandle
    {
        get { return rightHandle; }
    }
    public GameObject Background
    {
        get { return background; }
    }

    public Vector2 LStrtPos
    {
        get { return leftStartPos; }
    }

    public Vector2 RStrtPos
    {
        get { return rightStartPos; }
    }
    #endregion
    /// <summary>
    /// Sets initial variables and components
    /// </summary>
    private void Start()
    {
        leftStartPos = leftHandle.transform.position;
        rightStartPos = rightHandle.transform.position;
    }
    /// <summary>
    /// Update's used to manage the backgrounds BoxCollider's size when the player 
    /// manipulates the two sliders
    /// </summary>
    private void Update()
    {
        background.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, leftHandle.transform.position.x, Mathf.Abs(Mathf.Abs(leftHandle.transform.position.x) - Mathf.Abs(rightHandle.transform.position.x)));
        GetComponent<BoxCollider2D>().size = new Vector2(Vector3.Magnitude(leftHandle.transform.position - rightHandle.transform.position), 26.8f);
    }
}
