using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
/// <summary>
/// Handle Script Class
/// Coded by Barrington Campbell
/// Controls the sliders handles
/// </summary>
public class HandleScript : MonoBehaviour, IDragHandler, IEndDragHandler
{
    #region Attributes
    GameObject oppositeHandle;          //Handle on the opposite side of the slider handle that this script is on
    SliderHandler sHandler;             //The slider handler class
    enum Handle { LEFT, RIGHT};         //Enum to differentiate between the Left and Right slider handles
    Handle handlename;                  //Enum refrerence
    [SerializeField] bool drag;         //Tells if the handle is currently being dragged
    [SerializeField] Vector2 startPoint;//Position at which the player started dragging
    [SerializeField] float buffer;      //Buffer so the handles dont intersect eachother
    #endregion

    /// <summary>
    /// Sets initial variables and components
    /// </summary>
    private void Start()
    {
        //Instantiate variables
        sHandler = GameObject.FindGameObjectWithTag("UI_Slider").GetComponent<SliderHandler>();
        startPoint = transform.position;

        //Detect which handle this is and set the opposite handle variable
        if (this.gameObject == sHandler.LeftHandle)
        {
            handlename = Handle.LEFT;
            oppositeHandle = sHandler.RightHandle;
        }
        else
        {
            handlename = Handle.RIGHT;
            oppositeHandle = sHandler.LeftHandle;
        }

        //Get the buffer necessary so the handles don't intersect with eachother
        buffer = oppositeHandle.GetComponent<CircleCollider2D>().bounds.extents.x * 2;

    }

    /// <summary>
    /// Event that detects if the player is currently dragging across the screen
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        drag = true;
        if(handlename == Handle.LEFT)
        {
            transform.position = new Vector2(Mathf.Clamp(eventData.position.x, startPoint.x, oppositeHandle.transform.position.x - buffer), transform.position.y);
           // sHandler.Background.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, this.gameObject.transform.position.x, Mathf.Abs(Mathf.Abs(this.transform.position.x) - Mathf.Abs(oppositeHandle.transform.position.x)));
        }
        else
        {
            transform.position = new Vector2(Mathf.Clamp(eventData.position.x, oppositeHandle.transform.position.x + buffer, startPoint.x), transform.position.y);
           // sHandler.Background.GetComponent<RectTransform>().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, this.gameObject.transform.position.x, sHandler.Background.GetComponent<RectTransform>().rect.width);
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