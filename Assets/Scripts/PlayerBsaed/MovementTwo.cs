//TODO when you hit a wall that is more than 60 degrees up any rotation, U reck Ur speed/half ur speed?

using UnityEngine;

// Contains the command the user wishes upon the character
internal struct Cmd
{
    public float forwardMove;
    public float rightMove;
    public float upMove;
}

public class MovementTwo : MonoBehaviour
{
    public Transform playerView;     // Camera
    public float playerViewYOffset = 0.6f; // The height at which the camera is bound to
    public float xMouseSensitivity = 30.0f;
    public float yMouseSensitivity = 30.0f;
    //
    /*Frame occuring factors*/
    public float gravity = 20.0f;

    public float friction = 6; //Ground friction

    /* Movement stuff */
    public float moveSpeed = 7.0f;                // Ground move speed
    public float runAcceleration = 14.0f;         // Ground accel
    public float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;          // Air accel
    public float airDecceleration = 2.0f;         // Deacceleration experienced when ooposite strafing
    public float airControl = 0.3f;               // How precise air control is
    public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed when
    public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
    public float moveScale = 1.0f;

    //Mine:
    public static MovementTwo movementTwo;

    public float velocity;//Pos Y (downwards)

    public bool isGrounded = true;
    public bool landed = true;
    public static bool bouncable;

    /*print() style */
    public GUIStyle style;

    /*FPS Stuff */
    public float fpsDisplayRate = 4.0f; // 4 updates per sec

    private int frameCount = 0;
    private float dt = 0.0f;
    private float fps = 0.0f;

    private CharacterController _controller;

    // Camera rotations
    private float rotX = 0.0f;

    private float rotY = 0.0f;

    private Vector3 moveDirectionNorm = Vector3.zero;
    public Vector3 playerVelocity = Vector3.zero;//private
    public Vector3 palyerVelocityTwo;
    private float playerTopVelocity = 0.0f;

    // Q3: players can queue the next jump just before he hits the ground
    private bool wishJump = false;

    // Used to display real time fricton values
    private float playerFriction = 0.0f;

    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    private Cmd _cmd;

    private void Start()
    {
        // Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Put the camera inside the capsule collider
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);

        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        velocity = _controller.velocity.y;
        palyerVelocityTwo = playerVelocity;
        isGrounded = _controller.isGrounded;

        // Do FPS calculation
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / fpsDisplayRate)
        {
            fps = Mathf.Round(frameCount / dt);
            frameCount = 0;
            dt -= 1.0f / fpsDisplayRate;
        }
        /* Ensure that the cursor is locked into the screen */
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Input.GetButtonDown("Fire1"))
                Cursor.lockState = CursorLockMode.Locked;
        }

        /* Camera rotation stuff, mouse controls this shit */
        rotX -= Input.GetAxisRaw("Mouse Y") * xMouseSensitivity * 0.02f;
        rotY += Input.GetAxisRaw("Mouse X") * yMouseSensitivity * 0.02f;

        // Clamp the X rotation
        if (rotX < -90)
            rotX = -90;
        else if (rotX > 90)
            rotX = 90;

        this.transform.rotation = Quaternion.Euler(0, rotY, 0); // Rotates the collider
        playerView.rotation = Quaternion.Euler(rotX, rotY, 0); // Rotates the camera

        /* Movement, here's the important part */
        QueueJump();
        if (_controller.isGrounded)
            GroundMove();
        else if (!_controller.isGrounded)
            AirMove();

        // Move the controller
        _controller.Move(playerVelocity * Time.deltaTime);

        /* Calculate top velocity */
        Vector3 udp = playerVelocity;
        udp.y = 0.0f;
        if (playerVelocity.magnitude > playerTopVelocity)
            playerTopVelocity = playerVelocity.magnitude;

        //Need to move the camera after the player has been moved because otherwise the camera will clip the player if going fast enough and will always be 1 frame behind.
        // Set the camera's position to the transform
        playerView.position = new Vector3(
            transform.position.x,
            transform.position.y + playerViewYOffset,
            transform.position.z);

        FallingDown();
    }

    /*******************************************************************************************************\
   |* MOVEMENT
   \*******************************************************************************************************/

    /**
     * Sets the movement direction based on player input
     */

    private void SetMovementDir()
    {
        _cmd.forwardMove = Input.GetAxisRaw("Vertical");
        _cmd.rightMove = Input.GetAxisRaw("Horizontal");
    }

    /**
     * Queues the next jump just like in Q3
     */

    private void QueueJump()
    {
        if (Input.GetButtonDown("Jump") && !wishJump)
            wishJump = true;
        if (Input.GetButtonUp("Jump"))
            wishJump = false;

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && !wishJump)//Mine //Buggy
            wishJump = true;
    }

    /**
     * Execs when the player is in the air
    */

    private void AirMove()
    {
        Vector3 wishdir;
        float wishvel = airAcceleration;
        float accel;

        float scale = CmdScale();

        SetMovementDir();

        /*
        if(Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0)//Mine //Disables W and S
        {
            return;
        }
        */

        wishdir = new Vector3(_cmd.rightMove, 0, _cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        wishdir.Normalize();
        moveDirectionNorm = wishdir;
        wishspeed *= scale;

        // CPM: Aircontrol
        float wishspeed2 = wishspeed;
        if (Vector3.Dot(playerVelocity, wishdir) < 0)
            accel = airDecceleration;
        else
            accel = airAcceleration;
        // If the player is ONLY strafing left or right
        if (_cmd.forwardMove == 0 && _cmd.rightMove != 0)
        {
            if (wishspeed > sideStrafeSpeed)
                wishspeed = sideStrafeSpeed;
            accel = sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);

        if (airControl > 0)
            AirControl(wishdir, wishspeed2);
        // !CPM: Aircontrol

        // Apply gravity
        playerVelocity.y -= gravity * Time.deltaTime;
    }

    /**
     * Air control occurs when the player is in the air, it allows
     * players to move side to side much faster rather than being
     * 'sluggish' when it comes to cornering.
     */

    private void AirControl(Vector3 wishdir, float wishspeed)
    {
        float zspeed;
        float speed;
        float dot;
        float k;

        // Can't control movement if not moving forward or backward
        if (Mathf.Abs(_cmd.forwardMove) < 0.001 || Mathf.Abs(wishspeed) < 0.001)
            return;
        zspeed = playerVelocity.y;
        playerVelocity.y = 0;
        /* Next two lines are equivalent to idTech's VectorNormalize() */
        speed = playerVelocity.magnitude;
        playerVelocity.Normalize();

        dot = Vector3.Dot(playerVelocity, wishdir);
        k = 32;
        k *= airControl * dot * dot * Time.deltaTime;

        //A Change direction while slowing down
        if (dot > 0)
        {
            playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
            playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
            playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

            playerVelocity.Normalize();
            moveDirectionNorm = playerVelocity;
        }

        playerVelocity.x *= speed;
        playerVelocity.y = zspeed; // Note this line
        playerVelocity.z *= speed;
    }

    /**
     * Called every frame when the engine detects that the player is on the ground
     */

    private void GroundMove()
    {
        Vector3 wishdir;

        // Do not apply friction if the player is queueing up the next jump
        if (!wishJump)
            ApplyFriction(1.0f);
        else
            ApplyFriction(0);

        float scale = CmdScale();

        wishdir = new Vector3(_cmd.rightMove, 0, _cmd.forwardMove);
        wishdir = transform.TransformDirection(wishdir);
        wishdir.Normalize();
        moveDirectionNorm = wishdir;

        var wishspeed = wishdir.magnitude;
        wishspeed *= moveSpeed;

        Accelerate(wishdir, wishspeed, runAcceleration);

        // Reset the gravity velocity
        playerVelocity.y = 0;

        if (wishJump)
        {
            playerVelocity.y = jumpSpeed;
            wishJump = false;
        }
    }

    /**
     * Applies friction to the player, called in both the air and on the ground
     */

    private void ApplyFriction(float t)
    {
        Vector3 vec = playerVelocity; // Equivalent to: VectorCopy();
        float speed;
        float newspeed;
        float control;
        float drop;

        vec.y = 0.0f;
        speed = vec.magnitude;
        drop = 0.0f;

        /* Only if the player is on the ground then apply friction */
        if (_controller.isGrounded)
        {
            control = speed < runDeacceleration ? runDeacceleration : speed;
            drop = control * friction * Time.deltaTime * t;
        }

        newspeed = speed - drop;
        playerFriction = newspeed;
        if (newspeed < 0)
            newspeed = 0;
        if (speed > 0)
            newspeed /= speed;

        playerVelocity.x *= newspeed;
        playerVelocity.z *= newspeed;
    }

    private void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float addspeed;
        float accelspeed;
        float currentspeed;

        currentspeed = Vector3.Dot(playerVelocity, wishdir);
        addspeed = wishspeed - currentspeed;
        if (addspeed <= 0)
            return;
        accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        playerVelocity.x += accelspeed * wishdir.x;
        playerVelocity.z += accelspeed * wishdir.z;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0, 0, 400, 100), "FPS: " + fps, style);
        var ups = _controller.velocity;
        ups.y = 0;
        GUI.Label(new Rect(0, 15, 400, 100), "Speed: " + Mathf.Round(ups.magnitude * 100) / 100 + "ups", style);
        GUI.Label(new Rect(0, 30, 400, 100), "Top Speed: " + Mathf.Round(playerTopVelocity * 100) / 100 + "ups", style);
    }

    private float velocityOnLanding = -8.5f;//y velocity
    private Vector3 directionSet = new Vector3();

    private void FallingDown()
    {
        //If player is in air longer than x, do this:
        //speed += velocity?

        if (velocity <= -8.5f)//Falling
        {
            //playerVelocity.y += 9.4f;//Fun

            //Now dependent on where you look (rotation and strafe key) Your gna get boosted there/more velocity
            //Or Dependent on your velocity right now, yeah that's a better idea

            //When hit ground and there's a rotation, the speed addition is based of the rotation of the ground
            //If(groundRotation != 0)

            velocityOnLanding = velocity;
            if (velocityOnLanding <= velocity)
            {
                velocityOnLanding = velocity;
                //playerVelocity.x += velocityOnLanding;
                landed = false;
            }

            //Rotation Points (nulls) = 0, 90, 180, 270, 360(0)
            //+15 && -30
            //-30 && +15
            //360 = 0, 270 = -90, 180 = -180, 90 = -270

            Vector3 rOP = CollisionDetector.rotationOfPanel;

            if (playerVelocity.z >= 10) //+Forward //-Back
            {
                //Also base of rotation
                if (true/*rOP.x > 2 && rOP.x < 60*/)//This looks right //TODO also if there's 30 degree rotation on z, then combine them into the right direction
                {
                    //   directionSet.z = -velocityOnLanding;//Triggers 9k times

                    //Make it also based of rotation, so if its fits perfectly that 40 degree, get some good ass boost
                    //1-10 10% or something

                    //   Debug.Log(Mathf.Log10(5));//1.6 //0.6
                    //   Debug.Log(Mathf.Log10(15));//2.7 //1.2
                    //   Debug.Log(Mathf.Log10(40));//3.7 //1.6
                    //ROtation = "percentage" speed+(theoretical) = actual speed+
                    //1-10 = 10% = +-6
                    //11-25 = 40% = +-24
                    //26-40  = 80% = +-55
                    //41-60 = 100% = +- 73
                    //Debug.Log("Player velocity > 10");
                }

                if (rOP.x > 1 && rOP.x < 6)
                {
                    directionSet.z = -velocityOnLanding / 10;
                }
                else if (rOP.x >= 10 && rOP.x < 25)
                {
                    directionSet.z = -velocityOnLanding / 3f;
                }
                else if (rOP.x >= 25 && rOP.x < 40)
                {
                    directionSet.z = -velocityOnLanding / 1.1f;
                }
                else if (rOP.x >= 40 && rOP.x < 60)
                {
                    directionSet.z = -velocityOnLanding;
                }
            }
            else if (playerVelocity.z <= -6) //Back //Turn the object 180 degree
            {
                if (rOP.x > 1 && rOP.x < 10)
                {
                    directionSet.z = velocityOnLanding / 10;
                }
                else if (rOP.x >= 10 && rOP.x < 25)
                {
                    directionSet.z = velocityOnLanding / 3f;
                }
                else if (rOP.x >= 25 && rOP.x < 40)
                {
                    directionSet.z = velocityOnLanding / 1.1f;
                }
                else if (rOP.x >= 40 && rOP.x < 60)
                {
                    directionSet.z = velocityOnLanding;
                }
            }
            else if (playerVelocity.x >= 6) //+Right //-Left //TODO Kinda works, but U can use this movement on walls which have x axes of 30+ (abuse)
            {
                if (rOP.x >= 1 && rOP.x < 10) //-64+- downards speed
                {
                    directionSet.x = -velocityOnLanding / 10;
                    // Debug.Log(velocityOnLanding / 10);//-6
                }
                else if (rOP.x >= 10 && rOP.x < 25)
                {
                    directionSet.x = -velocityOnLanding / 3f;
                    //   Debug.Log(velocityOnLanding / 3f);//-24
                }
                else if (rOP.x >= 25 && rOP.x < 40)
                {
                    directionSet.x = -velocityOnLanding / 1.1f;
                    //   Debug.Log(velocityOnLanding / 1.1f);//-55
                }
                else if (rOP.x >= 40 && rOP.x < 60)
                {
                    directionSet.x = -velocityOnLanding;
                    //   Debug.Log(velocityOnLanding);//-73
                }
            }
            else if (playerVelocity.x <= -6) //Left
            {
                if (rOP.x > 1 && rOP.x < 10)
                {
                    directionSet.x = velocityOnLanding / 10;
                }
                else if (rOP.x >= 10 && rOP.x < 25)
                {
                    directionSet.x = velocityOnLanding / 3f;
                }
                else if (rOP.x >= 25 && rOP.x < 40)
                {
                    directionSet.x = velocityOnLanding / 1.1f;
                }
                else if (rOP.x >= 40 && rOP.x < 60)
                {
                    directionSet.x = velocityOnLanding;
                }
            }
        }
        else if (velocity >= -0.1f && !landed && bouncable)//And jump was bigger than X
        {
            Landed(directionSet);
        }
    }

    private void Landed(Vector3 direction)
    {
        landed = true;
        // Debug.Log(velocityOnLanding + " downwards speed");
        playerVelocity += direction;
        //Debug.Log(directionSet);
        directionSet = new Vector3(0, 0, 0);//Fixed the "remember" bug
        //Debug.Log(direction + " speed+");
        //playerVelocity += direction;//Kinda works, but just doubles the speed every time I hit the ground, make it dependent on:
        //If there's enough speed, and *based of the rotation of the panel
        //playerVelocity.x += velocityOnLanding;
    }

    /*
    ============
    PM_CmdScale

    Returns the scale factor to apply to cmd movements
    This allows the clients to use axial -127 to 127 values for all directions
    without getting a sqrt(2) distortion in speed.
    ============
    */

    private float CmdScale()
    {
        int max;
        float total;
        float scale;

        max = (int)Mathf.Abs(_cmd.forwardMove);
        if (Mathf.Abs(_cmd.rightMove) > max)
            max = (int)Mathf.Abs(_cmd.rightMove);
        if (max <= 0)
            return 0;

        total = Mathf.Sqrt(_cmd.forwardMove * _cmd.forwardMove + _cmd.rightMove * _cmd.rightMove);
        scale = moveSpeed * max / (moveScale * total);

        return scale;
    }
}