using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileControls : MonoBehaviour
{
    public bool enableMobileControls;

    [Header("Controllers")]
    public MobileJoystick movementJoystick;
    public MobileButton shootButton;

    //Instance
    public static MobileControls inst;
    void Awake () { inst = this; }
}
