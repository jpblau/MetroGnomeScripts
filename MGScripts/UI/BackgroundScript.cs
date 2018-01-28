using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Handle Script Class
/// Coded by Barrington Campbell
/// Controls both sliders when a player drags inbetween the two
/// </summary>
public class BackgroundScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Attributes
    GameObject leftHandle;              //Left slide handle
    GameObject rightHandle;             //Right slide handle
    SliderHandler sHandler;             //The slider handler class
    float backgroundDistance;           //Distance betweent the two handles
    [SerializeField] bool drag;         //Tells if the handle is currently being dragged
    [SerializeField] Vector2 startPoint;//Position at which the player started dragging
    [SerializeField] float buffer;      //Buffer so the handles dont intersect eachother
    #endregion

    /// <summary>
    /// Sets initial variables and components
    /// </summary>
    void Start()
    {
        sHandler = GameObject.FindGameObjectWithTag("UI_Slider").GetComponent<SliderHandler>();
        leftHandle = sHandler.LeftHandle;
        rightHandle = sHandler.RightHandle;
    }

    /// <summary>
    /// Updates the current background distance
    /// </summary>
    private void Update()
    {
        backgroundDistance = Vector3.Magnitude(leftHandle.transform.position - rightHandle.transform.position);
    }

    /// <summary>
    /// Event that detects if the player is started dragging across the screen
    /// </summary>
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {

        startPoint = eventData.pressPosition;
    }

    /// <summary>
    /// Event that detects if the player is currently dragging across the screen
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        //Determine which way the player is dragging toward
        if (eventData.position.x > startPoint.x)
        {
            leftHandle.transform.position = new Vector2(Mathf.Clamp(leftHandle.transform.position.x + 12f, sHandler.LStrtPos.x, sHandler.RStrtPos.x - backgroundDistance), leftHandle.transform.position.y);
            rightHandle.transform.position = new Vector2(Mathf.Clamp(rightHandle.transform.position.x + 12f, sHandler.LStrtPos.x + backgroundDistance, sHandler.RStrtPos.x), leftHandle.transform.position.y);
        }
        else
        {
            leftHandle.transform.position = new Vector2(Mathf.Clamp(leftHandle.transform.position.x - 12f, sHandler.LStrtPos.x, sHandler.RStrtPos.x - backgroundDistance), leftHandle.transform.position.y);
            rightHandle.transform.position = new Vector2(Mathf.Clamp(rightHandle.transform.position.x - 12f, sHandler.LStrtPos.x + backgroundDistance, sHandler.RStrtPos.x), leftHandle.transform.position.y);
        }

    }

    /// <summary>
    /// Event that detects if the player finished dragging
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        drag = false;
    }
}
