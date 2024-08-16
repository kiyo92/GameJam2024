using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Range(0f, 2f)] public float handleLimit = 1f;

    public Vector2 dir
    {
        get
        {
            return _dir;
        }
    }

    private Vector2 _dir;

    [Header("Components")]
    public RectTransform background;
    public RectTransform handle;

    [Header("Attack Controller")]
    public bool canAttack = false;

    private Vector2 joystickPosition;

    void Start ()
    {
        
        joystickPosition = background.position;
    }

    private void Update()
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {

                // assign new position to where finger was pressed
                background.position = new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, transform.position.z);
                joystickPosition = new Vector3(Input.GetTouch(i).position.x, Input.GetTouch(i).position.y, transform.position.z);
            }
            else if (Input.GetTouch(i).phase == TouchPhase.Moved) {
                Vector2 direction = Input.GetTouch(i).position - joystickPosition;
                _dir = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
                handle.anchoredPosition = (dir * background.sizeDelta.x / 2f) * handleLimit;
                canAttack = false;
            } else if (Input.GetTouch(i).phase == TouchPhase.Ended) {
                canAttack = true;
                _dir = Vector2.zero;
                handle.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void OnDrag (PointerEventData eventData)
    {
        /*
        Vector2 direction = eventData.position - joystickPosition;
        _dir = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        handle.anchoredPosition = (dir * background.sizeDelta.x / 2f) * handleLimit;
        canAttack = false;
        */
    }

    public void OnPointerDown (PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        /*
        canAttack = true;
        _dir = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        */
    }
}