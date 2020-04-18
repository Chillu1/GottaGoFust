using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class CollisionDetector : MonoBehaviour
{
    //public MovementTwo movementTwo;//Old Code
    private MovementPlayer movementPlayer;//New Code

    private Movement movement;//Even Newer Code
    public FinishMenu finishMenu;//In Canvas

    public static Vector3 rotationOfPanel;
    public GameObject respawnPoint;
    public GameObject player;

    private Collider collider;

    public bool started = false;

    private GameObject collisionDetector;
    //GameObject collider;//Change

    private bool groundedRay = false;
    private float distToGround;

    public List<GameObject> groundParts;//TODO Contains all parts which player can jump on (for grounded)

    //public GameObject[] groundParts;
    private int amountOfObjects = 0;//To Array

    public static bool onRamp = false;

    public float launchSpeed;

    // Use this for initialization
    private void Start()
    {
        movementPlayer = GetComponentInParent<MovementPlayer>();//CHANGE
        movement = GetComponentInParent<Movement>();

        collisionDetector = this.gameObject;
        collider = player.gameObject.transform.GetChild(0).gameObject.GetComponent<Collider>();//Collider //0?

        if (collider.name != "Collision Detector")
        {
            Debug.LogError("CollisionDetector: Collision Detector isn't taken");
        }

        started = false;
    }

    // Update is called once per frame
    private void Update()
    {
        //If player input?
        collisionDetector.transform.rotation = Quaternion.identity;
        collider.transform.rotation = Quaternion.identity;

        groundedRay = GroundCheck();

        if (groundParts.Count > 0)
        {
            //movement.grounded = true;
        }
    }

    private void HoldRotationSteady()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if(other.tag == "Panel")
        {
            MovementTwo.bouncable = true;
            Debug.Log("Bouncable: True");
            rotationOfPanel = other.gameObject.transform.rotation.eulerAngles;
            //Debug.Log(rotationOfPanel);
        }*/

        if (other.tag == "Death")
        {
            Debug.Log("Dead");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            //player.transform.position = respawnPoint.transform.position;
        }

        /* Removed for now, so if player goes out, he cant restart by going into the start again
        else if(other.tag == "Start")
        {
            started = false;
        }
        */
        else if (other.tag == "Finish")
        {
            //Finished the map
            //Time
            Debug.Log("Finished");
            Debug.Log("Time: " + Timer.time);
            //Save time
            //GlobalControl.Save();//Doesnt work
            GlobalControl.WriteString(SceneManager.GetActiveScene().name, Timer.time);//Works

            finishMenu.finished = true;
            started = false;
        }

        /*else if(other.tag == "BacktrackCheckpoint")
        {
            Debug.Log("BacktrackCheckpoint passed");
            part++;
            switch (part)
            {
                case 1:
                   // parts[0].SetActive(true);//Prob a bad idea with activating parts, better with render/collder/movementSlow
                    break;

                case 2:
                    //parts[1].SetActive(true);//Deactivated cuz were not using them
                    break;
            }

            //Another area is opened/ can be accssed
            //TODO If player doesn't go through checkpoint negative ideas: His movement is x time slower, next part of the map is not rendered/doesn't have colliders
        }*/
        else if (other.tag == "Ground" || other.tag == "Object" || other.tag == "Panel")//REMEMBER All tags on which you can jump
        {
            if (groundedRay)
            {
                //Grounded
                movementPlayer.grounded = true;//CHANGE reck this and the bottom one when testing
                                               // movement.grounded = true;

                //groundParts = new GameObject[amountOfObjects+1];
                //groundParts = new List<GameObject>(amountOfObjects + 1);

                //Can find right rotation by checking scale, so U can see what rotation is used for what
                if (other.gameObject.transform.rotation.x <= 90)
                {//TODO Hardcoding rotation for now
                 //TODO
                }
                groundParts.Add(other.gameObject);//REMEMBER Rotation based, y should be rotation which doesnt matter if its a wall
                amountOfObjects++;

                //groundParts[amountOfObjects] = other.gameObject;
            }
        }
        else if (other.tag == "Ramp")
        {
            if (true)
            {
                movementPlayer.grounded = true;
                groundParts.Add(other.gameObject);
                amountOfObjects++;
            }
            onRamp = true;
            //movementPlayer.friction = 0;
        }
        else if (other.tag == "Launch")
        {
            if (true)
            {
                movementPlayer.grounded = true;
                groundParts.Add(other.gameObject);
                amountOfObjects++;
            }

            movementPlayer.friction = launchSpeed;
        }
    }

    /*
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Object" || other.tag == "Panel")
        {
            if (!groundParts.Contains(other.gameObject))
            {
                groundParts.Add(other.gameObject);
            }
        }
    }
    */

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Start")//REMEMBER
        {
            Debug.Log("Started");
            //TODO Start timer and reduce velocity the max velocity You can get by 1 hop
            started = true;
        }
        else if (other.tag == "Ground" || other.tag == "Object" || other.tag == "Panel")
        {
            movementPlayer.grounded = false;//CHANGE
                                            // movement.grounded = false;

            //REMEMBER All grounded gameobjects might have to have a different name

            if (groundParts.Contains(other.gameObject))
            {
                //Debug.Log("Exists");
                groundParts.Remove(other.gameObject);
            }
            else if (!groundParts.Contains(other.gameObject))
            {
                Debug.LogError("Error: CollisionDetetctor: Doesn't exist. GO: " + other.name);
            }

            amountOfObjects--;
        }
        else if (other.tag == "Ramp")
        {
            movementPlayer.grounded = false;
            if (groundParts.Contains(other.gameObject))
            {
                //Debug.Log("Exists");
                groundParts.Remove(other.gameObject);
            }
            amountOfObjects--;
            onRamp = false;

            movementPlayer.friction = 15;
        }
        else if (other.tag == "Launch")
        {
            movementPlayer.grounded = false;
            if (groundParts.Contains(other.gameObject))
            {
                //Debug.Log("Exists");
                groundParts.Remove(other.gameObject);
            }
            amountOfObjects--;

            movementPlayer.friction = 15;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (groundedRay)
        {
            if (other.tag == "Ground" || other.tag == "Object" || other.tag == "Panel")
            {
                if (groundParts.Contains(other.gameObject))
                {
                    movementPlayer.grounded = true;
                }
                else if (!groundParts.Contains(other.gameObject))
                {
                    Debug.LogError("CollisionDetector -- OnTriggerStay -- GO: " + other.gameObject + ", doesn't exist in GroundParts Array");
                }
            }
        }

        //TODO WallStrafing (tag wall)
    }

    private bool GroundCheck()
    {
        RaycastHit hit;
        float distance;

        if (Physics.Raycast(player.transform.position, Vector3.down, out hit))
        {
            distance = hit.distance;
        }

        distToGround = collider.bounds.extents.y;//Idk actually

        //groundContactNormal = Vector3.up;
        //Debug.Log(distToGround);

        //4 raycasts, all corners of player

        if (Physics.Raycast(new Vector3(player.transform.position.x + 0.5f, player.transform.position.y - 0.49f, player.transform.position.z + 0.5f), Vector3.down, distToGround - -0.01f))
        {
            if (Physics.Raycast(new Vector3(player.transform.position.x + 0.5f, player.transform.position.y - 0.49f, player.transform.position.z + 0.5f), Vector3.down, out hit))
            {
                if (hit.collider.gameObject.tag == "Object" || hit.collider.gameObject.tag == "Ground")
                {
                    return true;
                }
            }
        }

        if (Physics.Raycast(new Vector3(player.transform.position.x + 0.5f, player.transform.position.y - 0.49f, player.transform.position.z - 0.5f), Vector3.down, distToGround - -0.01f))
        {
            if (Physics.Raycast(new Vector3(player.transform.position.x + 0.5f, player.transform.position.y - 0.49f, player.transform.position.z - 0.5f), Vector3.down, out hit))
            {
                if (hit.collider.gameObject.tag == "Object" || hit.collider.gameObject.tag == "Ground")
                {
                    return true;
                }
            }
            //return true;//Causes player to jump up when its not suppoesd 2
        }

        if (Physics.Raycast(new Vector3(player.transform.position.x - 0.5f, player.transform.position.y - 0.49f, player.transform.position.z - 0.5f), Vector3.down, distToGround - -0.01f))
        {
            if (Physics.Raycast(new Vector3(player.transform.position.x - 0.5f, player.transform.position.y - 0.49f, player.transform.position.z - 0.5f), Vector3.down, out hit))
            {
                if (hit.collider.gameObject.tag == "Object" || hit.collider.gameObject.tag == "Ground")
                {
                    return true;
                }
            }
            //return true;
        }

        if (Physics.Raycast(new Vector3(player.transform.position.x - 0.5f, player.transform.position.y - 0.49f, player.transform.position.z + 0.5f), Vector3.down, distToGround - -0.01f))
        {
            if (Physics.Raycast(new Vector3(player.transform.position.x - 0.5f, player.transform.position.y - 0.49f, player.transform.position.z + 0.5f), Vector3.down, out hit))
            {
                if (hit.collider.gameObject.tag == "Object" || hit.collider.gameObject.tag == "Ground")
                {
                    return true;
                }
            }
            // return true;
        }

        return Physics.Raycast(player.transform.position, Vector3.down, distToGround - -0.01f);//Check if grounded //0.1f is overkill? //TODO //0.01f //Middle?

        //float dist = 10;

        //return false;
    }
}