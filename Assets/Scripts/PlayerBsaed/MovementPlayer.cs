using UnityEngine;

public class MovementPlayer : MonoBehaviour
{
    //The right one

    public static MovementPlayer movementPlayer;

    private GameObject player;

    //private GameObject cC;//CharacterController
    private Rigidbody rb;

    private GameObject mainCamera;

    private Hotkeys hotkeys;

    private MouseMovement mouseMovement;

    private float distanceToGround = 0;

    public float friction;//9.8f //8 //

    public float ground_accelerate;//50 //200
    public float max_velocity_ground;//4 //6.4

    public float air_accelerate;//100 //200
    public float max_velocity_air;//8 //0.6

    private float defaultAirAccelerate;

    public float jumpForce;//330? //5

    private Vector3 desiredMove;

    public float test;//0

    private LayerMask groundLayers;//For Raycast maybe

    public bool grounded;
    public bool cantJump;

    public float highest;

    private float distance = 0;

    private Vector2 input;

    public float velocity;

    private float lastJumpPress = -1f;
    private float jumpPressDuration = 0.1f;

    public bool noclipOn;

    [SerializeField] private bool normalizeInput = false;

    // Use this for initialization
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody>();
        mainCamera = Camera.main.gameObject;

        hotkeys = this.GetComponent<Hotkeys>();
        mouseMovement = mainCamera.GetComponent<MouseMovement>();

        defaultAirAccelerate = air_accelerate;
    }

    private void Update()
    {
        CheckForNoclip();

        if (!noclipOn)//If normal movement
        {
            //CheckForCircleJump();

            desiredMove = player.transform.forward * Input.GetAxis("Vertical") + player.transform.right * Input.GetAxis("Horizontal");
            desiredMove = desiredMove.normalized;

            //TODO Normalize input somehow, but make it so it slowly build up speed
            //if(input.magnitude < 1)
            //{
            if (!normalizeInput)
            {
                input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
            else if (normalizeInput)
            {
                input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            }
            //}
            //else
            //input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            /*if (input.x + input.y > input.normalized.x + input.normalized.y)
            {
                input = input.normalized;
            }*/
            //input = input.normalized;

            if (grounded)
            {
                jumpTime += Time.deltaTime;
            }
            else
            {
                jumpTime = 0;
            }

            //print(new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude);//Checking Velocity //Debug log?

            if (Input.GetButton("Jump") || Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetAxis("Mouse ScrollWheel") > 0)//TODO Make it axis and not holding the button, buggy right now cuz of mousewheel ++ usage
            {
                lastJumpPress = Time.time;//This is anti-synergy with scroll wheel
            }

            velocity = rb.velocity.magnitude;
        }
    }

    private float jumpTime;

    private void FixedUpdate()
    {//REMEMBER FixedUpdate instead of update, so its based on rigidbidy
        if (!noclipOn)
        {
            //TODO If theres no keyboard clicks + mouse movement + not in air, stop?

            // rb.velocity = MoveGround(desiredMove, rb.velocity);//Movement

            Vector3 playerVelocity = rb.velocity;

            if (grounded == true && cantJump == false)//New type of grounded //Experimental needed
            {
                //playerVelocity = CalculateFrictionOld(playerVelocity);//Old friction //TODO When Y (rotation) /ramp is not 0, dont apply as much friction/not at all?
            }

            if (grounded)//New friction, idk about cant jump tho
            {
                playerVelocity = CalcualteFriction(playerVelocity);
            }

            playerVelocity += CalculateMovement(input, playerVelocity);
            rb.velocity = playerVelocity;

            if (highest < player.transform.position.y)
            {
                highest = player.transform.position.y;
                //Debug.Log(highest);//2.078523 //1.746076 is right
            }

            RaycastHit hit;

            if (Physics.Raycast(player.transform.position, Vector3.down, out hit))
            {
                distance = hit.distance;
                if (highest < distance)
                {
                    highest = distance;
                }
            }

            cantJump = false;//TODO FIXME

            if (Input.GetAxis("Mouse ScrollWheel") < 2 || Input.GetAxis("Mouse ScrollWheel") > 2)//Old Code, redundant?//Doesnt check for in air
            {
                //TODO U can always use trigger enter tbh, might be easier but worse

                /*
                Debug.Log("Distance: "+distance);
                Debug.Log("Velocity: " + rb.velocity.y);
                */

                if (grounded == true/* && distance < 1.001f*/ && cantJump == false && rb.velocity.y < 0.1f && rb.velocity.y > -0.1f)//A lot of random shit here, just to make it work, prob has 2 be polished after some time //Cant properly jump on shit which is curved cuz of distance
                {
                    //rb.AddRelativeForce(Vector3.up * 200);
                    //rb.AddForce(Vector3.up * 300);//Seems better? //200 seems to be to low //Old Jump
                    cantJump = true;
                    //Debug.Log(rb.velocity.y);
                }

                else;
                //Debug.Log("Cant jump in mid-air");
            }

            JumpingSounds();
        }
        else if (noclipOn)
        {
            //Noclip();//Redundant?
        }
    }

    private Vector3 CalcualteFriction(Vector3 currentVelocity)
    {
        float speed = currentVelocity.magnitude;

        if (grounded == false || speed == 0f/* || cantJump == true*/)
        {
            return currentVelocity;
        }

        float drop = speed * friction * Time.deltaTime;

        if (jumpTime > 0.08f && !CollisionDetector.onRamp)//Sec elapsed //Grounded for more than 0.08secs
        {
            //Debug.Log("0.08f sec has passed");
            Debug.Log(friction + " Friction added");
            return currentVelocity * (Mathf.Max(speed - drop, 0f) / speed);
        }

        return currentVelocity;
    }

    //Asuro

    private Vector3 CalculateFrictionOld(Vector3 currentVelocity)
    {
        float speed = currentVelocity.magnitude;

        if (grounded == false || speed == 0f/* || cantJump == true*/)
        {
            return currentVelocity;
        }

        float drop = speed * friction * Time.deltaTime;
        return currentVelocity * (Mathf.Max(speed - drop, 0f) / speed);
    }

    private Vector3 CalculateMovement(Vector2 input, Vector3 velocity)
    {
        float currentAcc = ground_accelerate;
        float curMaxSpeed = max_velocity_ground;

        if (grounded == false/* && cantJump == true*/)
        {
            currentAcc = air_accelerate;
            curMaxSpeed = max_velocity_air;
        }
        Vector3 strafeRotation;
        strafeRotation = new Vector3(mouseMovement.rotationX, 0, mouseMovement.rotationX);

        Vector3 camRotation = new Vector3(0f, mainCamera.transform.rotation.eulerAngles.y, 0f);
        Vector3 inputVelocity = Quaternion.Euler(camRotation) * new Vector3(input.x * currentAcc, 0f, input.y * currentAcc);

        //inputVelocity += new Vector3(mouseMovement.rotationX/2, 0f, mouseMovement.rotationX/2).normalized*10;//0.7,0.0,0.7, hmm
        //Debug.Log("ROtationX: " + new Vector3(mouseMovement.rotationX / 2, 0f, mouseMovement.rotationX / 2).magnitude);
        Vector3 alignedInputVelocity = new Vector3(inputVelocity.x, 0f, inputVelocity.z) * Time.deltaTime;

        Vector3 currentVelocity = new Vector3(velocity.x, 0f, velocity.z);

        float max = Mathf.Max(0f, 1 - (currentVelocity.magnitude / curMaxSpeed));//Interesting af

        float velocityDot = Vector3.Dot(currentVelocity, alignedInputVelocity);

        Vector3 modifiedVelocity = alignedInputVelocity * max;

        Vector3 correctVelocity = Vector3.Lerp(alignedInputVelocity, modifiedVelocity, velocityDot);//Hardcore shit

        correctVelocity += GetJumpVelocity(velocity.y);
        //Dont apply jump
        //Debug.Log(currentVelocity);

        //if mouse X axis (diff) == velocity (diff) { movement++}
        Debug.Log("Mouse Horizontal: " + Input.GetAxisRaw("Horizontal"));
        //Debug.Log(rb.velocity.)
        if (Input.GetAxis("Horizontal") > 0)
        {
        }

        return correctVelocity;
    }

    private Vector3 GetJumpVelocity(float yVel)
    {
        Vector3 jumpVel = Vector3.zero;

        if (Time.time < lastJumpPress + jumpPressDuration && yVel < jumpForce && grounded == true)
        {
            lastJumpPress = -1f;
            jumpVel = new Vector3(0f, jumpForce - yVel, 0f);
        }

        return jumpVel;
    }

    private Vector3 CalculateRamp(Vector2 input, Vector3 playerVelocity)//Vector2 Input, Vector3 playerVelocity
    {
        //Use Tag ramp

        return new Vector3(0, 0, 0);//Just so it doesnt reck error, remove afterwards/when done
    }

    private float circleJumpTimer;
    private float groundedTimer;

    private void CheckForCircleJump()
    {
        if (grounded /*&& Time > 0.5f */)//If grounded in the last 0.6 secs, give air accel++?
        {
            circleJumpTimer = 0.6f;
            //air_accelerate = 300;

            groundedTimer += Time.deltaTime;

            if (groundedTimer >= 0.6f)
            {
                groundedTimer = 0.6f;
            }

            air_accelerate = 300;
        }
        else if (!grounded)
        {
            if (groundedTimer > 0.2f)
            {
                circleJumpTimer -= Time.deltaTime;
                if (true)
                {
                    air_accelerate = defaultAirAccelerate;
                    //Another time to set groundedTimer to zero?
                    return;
                }
            }

            air_accelerate = 300;
            groundedTimer = 0;
        }

        //return new Vector3(0, 0, 0);
    }

    private float cooldown;

    private void JumpingSounds()
    {
        if (!grounded)
        {
            cooldown += Time.deltaTime;
        }

        if (jumpTime > 0f && grounded && cooldown > 0.1f)
        {
            AudioClip ac = Resources.Load<AudioClip>("Sounds/JumpLand");

            AudioSource.PlayClipAtPoint(ac, new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z));//TODO, fix so Audio doesn't stay back
            cooldown = 0;
        }
    }

    private void CheckForNoclip()
    {
        if (Input.GetKeyDown(KeyCode.N) && GetComponent<Hotkeys>().cheating)
        {
            if (noclipOn)
            {
                noclipOn = false;
                NoclipOff();
            }
            else if (!noclipOn)
            {
                noclipOn = true;
            }
        }

        if (hotkeys.cheating && noclipOn)
        {
            Noclip();//Use only camera/disable collisions in Player
        }
    }

    private float noclipSpeed = 0.5f;

    private void Noclip()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            noclipSpeed = 1;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            noclipSpeed = 0.05f;
        }
        else
        {
            noclipSpeed = 0.5f;
        }

        Physics.gravity = new Vector3(0, 0, 0);
        player.GetComponent<Collider>().enabled = false;
        player.transform.GetChild(0).gameObject.SetActive(false);
        rb.velocity = new Vector3(0, 0, 0);
        player.transform.rotation = mainCamera.transform.rotation;
        //Space=Up, Ctrl/Shift = down, wasd = movement
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            player.transform.Translate(Input.GetAxisRaw("Horizontal") * noclipSpeed, 0, Input.GetAxisRaw("Vertical") * noclipSpeed);
        }
    }

    private void NoclipOff()
    {
        Physics.gravity = new Vector3(0, -15f, 0);
        player.GetComponent<Collider>().enabled = true;
        player.transform.GetChild(0).gameObject.SetActive(true);
    }
}