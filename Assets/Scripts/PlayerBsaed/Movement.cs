using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    //TODO Maybe make a second collider which is only with the feet of the player, and that's only for grounded

    /*
     Analyzing 
    https://github.com/ValveSoftware/source-sdk-2013/blob/master/mp/src/game/shared/gamemovement.cpp

        1373 Water Move
        1517 Step move (Basically small ledges?)
        1612 Friction
        1684 Finish Gravity

        1707 AirAccelerate
        1753 AirMove

        1822 Accelerate

        1859 StayOnGround (Slopes)

        1895 WalkMove (Ground move?)
        2025 FullWalkMove

        2026 CheckJumpButton (Important, mwheelup/down)

        2536 FullLadderMove

        2560 TryPlayerMove

        2820 OnLadder
        2854 LadderMove

        3049 CheckVelocity

        3092 AddGravity

        3145 ClipVelocity

        3226 CreateStuckTable, interesting positions of when stuck
        3377 CheckStuck

        3899 CheckFalling

        4042 CanUnCrouch
        4081 FinishUnCrouch
        4334 Crouch

        4549 PlayerMove

        4758 FullTossMove (Might be circle-jump)

        

        */

        /*
         My comments on code:

        Stopping with S is isntant




        */

    public float speed;

    public float groundAcc = 5;
    public float airAcc = 8;

    public float maxSpeedGround = 100;
    public float maxSpeedAir = 300;

    public float friction = 8.61f;

    float inputX, inputY;

    public bool grounded = true;
    public bool groundedRay = true;

    float distToGround;

    public float highest;

    ///*
    bool onLadder;

    public float groundAccCrouch;
    

    public bool crouched;
    float duckTime;// /Unduck time

    public float gravity;

    //*/

    Vector3 desiredMove;//AcellDir
    Vector3 prevVelocity;

    public float highestYVelocity;

    public float test;

    float velocity;

    //Vector3 groundContactNormal;

    private GameObject player;
    private Collider collider;
    private CharacterController cC;
    private Rigidbody rb;

    public UnityEngine.UI.Text velocityText;


    //Extra rn:
    public float jumpForce;

    private float lastJumpPress = -1f;
    private float jumpPressDuration = 0.1f;


    void Start () {
        player = this.gameObject;
        //collider = player.GetComponent<Collider>();//Old
        collider = this.GetComponentInChildren<Collider>();
        cC = this.gameObject.GetComponent<CharacterController>();
        rb = this.GetComponent<Rigidbody>();

        
	}

    void Update()
    {
        //grounded = GroundCheck();

        velocityText.text = rb.velocity.magnitude.ToString();
        velocity = rb.velocity.magnitude;

        if (Input.GetButton("Jump") || Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Mouse ScrollWheel") > 0)//TODO Make it axis and not holding the button, buggy right now cuz of mousewheel ++ usage
        {
            lastJumpPress = Time.time;//This is anti-synergy with scroll wheel
        }

        CheckForWallRay();
    }


    void FixedUpdate () {

        groundedRay = GroundCheck();

        if (Input.GetKey(KeyCode.Space) && grounded && groundedRay)//TODO
        {
            //Jump();
            rb.velocity += GetJumpVelocity(rb.velocity.y);
        }

        if(rb.velocity.y > 8)
        {
            //rb.AddRelativeForce(Vector3.down * 400);//TODO Buggy and gay af, but works for now

        }

        if(rb.velocity.y > highestYVelocity)
        {
            highestYVelocity = rb.velocity.y;
        }
        

        if (Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0 || Input.GetAxis("Horizontal") < 0 || Input.GetAxis("Vertical") < 0)
        {
            inputX = Input.GetAxis("Horizontal");
            inputY = Input.GetAxis("Vertical");

            desiredMove = transform.forward * inputY + transform.right * inputX;
            //desiredMove = Vector3.ProjectOnPlane(desiredMove, groundContactNormal).normalized;

            rb.velocity = MoveGround(desiredMove, rb.velocity);
            

            InputGotten();
        }

        //Debug.Log(inputY);

	}

    void InputGotten()
    {



    }

    void Gravity()//Member this is code gravity/friction, not Rigidbody
    {

    }

    RaycastHit hitBot;//Bottom
    RaycastHit hitMid;//Middle
    RaycastHit hitTop;//Top

    float distanceBot;
    float distanceMid;
    float distanceTop;

    float usedDistance;

    void CheckForWallRay()
    {

        //CheckWhere();

        if (Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y - 0.99f, player.transform.position.z), transform.TransformPoint(Vector3.left) /*Vector3.left*/, out hitBot) || Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y + 0.99f, player.transform.position.z), transform.TransformPoint(Vector3.left), out hitTop) || Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), transform.TransformPoint(Vector3.left), out hitMid))//Ramps are going to be buggy af, based on rotation as well?
        {
            //TODO Changed to Transform point, it doesnt work 100%

            //X+ = right, X- = left
            //Y+ = forward, Y- = back
            CheckWhere();
            //Debug.Log(distance);

            if (usedDistance < 0.53f)//TODO 0.53f might be to high. 0.501f possible
            {
                //Debug.Log("Something is close on the left");
                //(In movement) if(input a + w && wallClose && camrot is around X) { wallstrafe

                if(inputX <= 0)
                {
                    
                    rb.velocity = new Vector3(0,0,0);
                }

            }

        }

        if (Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y - 0.99f, player.transform.position.z), Vector3.forward, out hitBot) || Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y + 0.99f, player.transform.position.z), Vector3.forward, out hitTop) || Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.forward, out hitMid))
        {
            if(usedDistance < 0.53f)
            {
                //Forward


            }


        }

        if (Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y - 0.99f, player.transform.position.z), Vector3.back, out hitBot) || Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y + 0.99f, player.transform.position.z), Vector3.back, out hitTop) || Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.back, out hitMid))
        {
            if (usedDistance < 0.53f)
            {
                //Back


            }


        }

        if (Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y - 0.99f, player.transform.position.z), Vector3.right, out hitBot) || Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y + 0.99f, player.transform.position.z), Vector3.right, out hitTop) || Physics.Raycast(new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z), Vector3.right, out hitMid))
        {
            if (usedDistance < 0.53f)
            {
                //Right


            }


        }


    }

    void CheckWhere()
    {

        if (hitBot.distance != 0)
        {
            distanceBot = hitBot.distance;
        }
        else
            distanceBot = Mathf.Infinity;

        if(hitMid.distance != 0)
        {
            distanceMid = hitMid.distance;
        }
        else
            distanceMid = Mathf.Infinity;

        if (hitTop.distance != 0)
        {
            distanceTop = hitTop.distance;
        }
        else
            distanceTop = Mathf.Infinity;

        usedDistance = Mathf.Min(distanceBot, Mathf.Min(distanceMid, distanceTop));
        //Debug.Log(Mathf.Min(distanceBot, distanceMid, distanceTop));
        
    }


    bool GroundCheck()
    {
        RaycastHit hit;
        float distance;

        if(Physics.Raycast(player.transform.position, Vector3.down, out hit))
        {
            distance = hit.distance;

            if(highest < distance)
            {
                highest = distance; //1.28596 (200)
            }
        }

        distToGround = collider.bounds.extents.y;//Idk actually

        //groundContactNormal = Vector3.up;
        //Debug.Log(distToGround);

        //4 raycasts, all corners of player

        if (Physics.Raycast(new Vector3(player.transform.position.x + 0.5f, player.transform.position.y, player.transform.position.z + 0.5f), Vector3.down, distToGround - test))
        {
            return true;
        }

        if (Physics.Raycast(new Vector3(player.transform.position.x + 0.5f, player.transform.position.y, player.transform.position.z - 0.5f), Vector3.down, distToGround - test))
        {
            return true;
        }

        if (Physics.Raycast(new Vector3(player.transform.position.x - 0.5f, player.transform.position.y, player.transform.position.z - 0.5f), Vector3.down, distToGround - test))
        {
            return true;
        }

        if (Physics.Raycast(new Vector3(player.transform.position.x - 0.5f, player.transform.position.y, player.transform.position.z + 0.5f), Vector3.down, distToGround - test))
        {

            return true;
        }

        return Physics.Raycast(player.transform.position, Vector3.down, distToGround - test);//Check if grounded //0.1f is overkill? //TODO //0.01f
        

        //float dist = 10;


        //return false;

    }

    void Jump()
    {
        rb.AddRelativeForce(Vector3.up * 400);

    }

    Vector3 GetJumpVelocity(float yVel)
    {
        Vector3 jumpVel = Vector3.zero;

        if (Time.time < lastJumpPress + jumpPressDuration && yVel < jumpForce && grounded == true)
        {
            lastJumpPress = -1f;
            jumpVel = new Vector3(0f, jumpForce - yVel, 0f);
        }

        return jumpVel;
    }

    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxVelocity)
    {
        float projVel = Vector3.Dot(prevVelocity, accelDir);
        float accelVel = accelerate * Time.fixedDeltaTime;

        if(projVel + accelVel > maxVelocity)
        {
            accelVel = maxVelocity - projVel;
        }

        return prevVelocity + accelDir * accelVel;
    }

    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        float speed = prevVelocity.magnitude;

        if(speed != 0)
        {
            float drop = speed * friction * Time.fixedDeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        return Accelerate(accelDir, prevVelocity, groundAcc, maxSpeedGround);

    }

    private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    {

        return Accelerate(accelDir, prevVelocity, airAcc, maxSpeedAir);
    }
   

}
