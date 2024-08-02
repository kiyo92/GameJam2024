using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the player's movement and rotation from inputs.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody rig;                   //Player's rigidbody component.

    void Awake ()
    {
        //Get missing components
        if(!rig) rig = GetComponent<Rigidbody>();
    }

    void Update ()
    {
        //If the player is able to move, then move the player.
        if(Player.inst.canMove)
        {
            Move();
        }
    }

    void LateUpdate ()
    {
        //If the player is able to move, then make them look at the mouse.
        if(Player.inst.canMove)
        {
            Look();
        }
    }

    //Moves the player based on keyboard inputs.
    void Move ()
    {
        //Get the horizontal and vertical keyboard inputs.
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //Use joystick direction if mobile controls is enabled.
        if(MobileControls.inst.enableMobileControls)
        {
            x = MobileControls.inst.movementJoystick.dir.x;
            y = MobileControls.inst.movementJoystick.dir.y;
        }

        //Get the forward and right direction of the camera.
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        //Since we don't need the Y axis, we can remove it.
        camForward.y = 0;
        camRight.y = 0;

        //Normalise the directions since a rotated camera will cause issues.
        camForward.Normalize();
        camRight.Normalize();

        //Create a direction for the player to move at, which is relative to the camera.
        Vector3 dir = (camForward * y) + (camRight * x);

        //Update player state.
        if(Player.inst.state != PlayerState.Dead)
        {
            if(dir.magnitude > 0)
            {
                if(Player.inst.state != PlayerState.Moving)
                {
                    Player.inst.state = PlayerState.Moving;
                    Player.inst.anim.SetBool("Moving", true);
                }
            }
            else
            {
                if(Player.inst.state != PlayerState.Idle)
                {
                    Player.inst.state = PlayerState.Idle;
                    Player.inst.anim.SetBool("Moving", false);
                }
            }
        }

        //Finally set that as the player's velocity, also including the player's move speed.
        rig.velocity = dir * Player.inst.moveSpeed;
    }

    //Rotate the player so they're facing the mouse cursor.
    void Look ()
    {
        //Are we playing on desktop?
        if(!MobileControls.inst.enableMobileControls)
        {
            //Create a plane and shoot a raycast at it to get the world position of our mouse cursor.
            Plane rayPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float rayDist;
            Vector3 worldPos = Vector3.zero;

            if(rayPlane.Raycast(ray, out rayDist))
                worldPos = ray.GetPoint(rayDist);

            //Get the direction of it relative to the player.
            Vector3 dir = (worldPos - transform.position).normalized;

            //Convert that direction to an angle we can apply to the player.
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            //Set the angle to be the player's Y rotation.
            transform.rotation = Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z);
        }
        //Are we playing on mobile?
        else
        {
            MobileJoystick joy = MobileControls.inst.movementJoystick;
            
            //Is the joystick not in the center?
            if(joy.dir.magnitude != 0)
            {
                //Get an angle from the joystick's direction.
                float angle = Mathf.Atan2(joy.dir.x, joy.dir.y) * Mathf.Rad2Deg;

                //Set the angle to be the player's Y rotation.
                transform.rotation = Quaternion.Euler(transform.rotation.x, angle, transform.rotation.z);
            }
        }
    }
}
