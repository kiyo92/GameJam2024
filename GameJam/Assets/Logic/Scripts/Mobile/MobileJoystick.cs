using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    public Image ring;
    public Image knob;

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
                var tempColor = ring.color;
                tempColor.a = 1f;
                ring.color = tempColor;
                knob.color = tempColor;
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
                var tempColor = ring.color;
                tempColor.a = 0.5f;
                ring.color = tempColor;
                knob.color = tempColor;

                // Reset knob position
                background.localPosition = new Vector3(0, -600, transform.position.z);
                handle.anchoredPosition = Vector2.zero;
            }
        }
    }
    
    public void OnDrag (PointerEventData eventData)
    {
    #if UNITY_EDITOR
        Vector2 direction = eventData.position - joystickPosition;
        _dir = (direction.magnitude > background.sizeDelta.x / 2f) ? direction.normalized : direction / (background.sizeDelta.x / 2f);
        handle.anchoredPosition = (dir * background.sizeDelta.x / 2f) * handleLimit;
        canAttack = false;
    #endif
    }

    public void OnPointerDown (PointerEventData eventData)
    {
    #if UNITY_EDITOR
        var tempColor = ring.color;
        tempColor.a = 1f;
        ring.color = tempColor;
        knob.color = tempColor;
        OnDrag(eventData);
    #endif
    }

    public void OnPointerUp (PointerEventData eventData)
    {
    #if UNITY_EDITOR
        var tempColor = ring.color;
        tempColor.a = 0.5f;
        ring.color = tempColor;
        knob.color = tempColor;
        canAttack = true;
        handle.anchoredPosition = Vector2.zero;
    #endif
    }
}