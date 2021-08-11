using UnityEngine;
using UnityEngine.EventSystems;

public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //[HideInInspector]
    public Vector2 TouchDist;
    //[HideInInspector]
    public Vector2 PointerOld;
    //[HideInInspector]
    protected int PointerId;
    //[HideInInspector]
    public bool Pressed;
    public bool IsChange = false;
    public Vector2 RealXY;
    public Vector2 ResetPXY;
    public Vector2 FirstRealXY;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Pressed)
        {
            if (PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
            }
            else
            {
                TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                PointerOld = Input.mousePosition;
            }
            if (!IsChange)
            {
                ResetPXY = PointerOld;
                FirstRealXY = RealXY;
                IsChange = true;
            }
        }
        else
        {
            TouchDist = new Vector2();
            IsChange = false;
        }
        RealXY = FirstRealXY - ResetPXY + PointerOld;

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }

}
