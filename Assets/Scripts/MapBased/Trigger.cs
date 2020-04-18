using System.Collections;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    //Solution to mulitple objects: Prob just break instance from Prefab, prob...
    //Better solution? Have a function in the name?

    //Trigger Fast, when close, so it doesn't block

    private GameObject trigger;//No point in asigning this
    public GameObject mainObject;

    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private Vector3 originalScale;

    public float rotation;

    public float delay = 2;

    public float speed = 1;
    public float fastSpeed = 10;

    public float speedDoor;

    public float localLength = 0;

    public float length = 0;

    public Vector2 lengthDoor;

    public bool activated;

    private float framesPassed = 0;

    private void Start()
    {
        trigger = this.gameObject;

        originalPosition = mainObject.transform.position;
        originalRotation = mainObject.transform.rotation.eulerAngles;

        RotationCheckXYZ();//0s become 360s

        originalScale = mainObject.transform.lossyScale;//TODO idk check

        if (speed == 0)
        {
            speed = fastSpeed;
        }
    }

    private void Update()
    {
        if (Time.frameCount >= framesPassed + 30)//Checkes every 30 frames
        {
            framesPassed = Time.frameCount;
        }

        if (!activated && trigger.name.Contains("Activate"))
        {
            mainObject.SetActive(false);
        }

        if (activated && trigger.name.Contains("Fast"))//Makes it speed up to instant
        {
            //Remove?
        }
        else
        {
            //Remove?
        }

        if (activated && trigger.name.Contains("DownRot-30"))//Test, works. Not 10/10 solution, but still good I guess
        {
            ChangeZRotationByX();
        }

        if (activated && trigger.name.Contains("Move"))
        {
            Move();
        }

        if (activated && trigger.name.Contains("Explode"))
        {
            StartCoroutine(Explode());
        }

        if (activated && trigger.name.Contains("Rot"))
        {
            ChangeRotation();
        }

        if (activated && trigger.name.Contains("Activate"))
        {
            Activate();
        }

        if (activated && trigger.name.Contains("Deactivate"))
        {
            Deactivate();
        }

        if (activated && trigger.name.Contains("OpenDoor"))
        {
            OpenDoor();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        activated = true;
    }

    private void Move()
    {
        if (localLength != 0)
        {
            if (trigger.name.Contains("Forward"))//Front
            {
                if (mainObject.transform.localPosition.x < localLength)
                {
                    mainObject.transform.Translate(Vector3.right * Time.deltaTime * speed);//Right, for some reason
                }
            }

            if (trigger.name.Contains("Back"))
            {
                if (mainObject.transform.localPosition.x > localLength)
                {
                    mainObject.transform.Translate(Vector3.left * Time.deltaTime * speed);//Left
                }
            }

            if (trigger.name.Contains("Right"))
            {
                if (mainObject.transform.localPosition.z > localLength)
                {
                    mainObject.transform.Translate(Vector3.back * Time.deltaTime * speed);//Back, Idk why
                }
            }

            if (trigger.name.Contains("Left"))
            {
                //Forawrd
                if (mainObject.transform.localPosition.z < localLength)
                {
                    mainObject.transform.Translate(Vector3.forward * Time.deltaTime * speed);//Forward
                }
            }

            if (trigger.name.Contains("Up"))
            {
                if (mainObject.transform.localPosition.y < localLength)
                {
                    mainObject.transform.Translate(Vector3.up * Time.deltaTime * speed);
                }
            }

            if (trigger.name.Contains("Down"))
            {
                if (mainObject.transform.localPosition.y > localLength)
                {
                    mainObject.transform.Translate(Vector3.down * Time.deltaTime * speed);
                }
            }
        }
        else if (length != 0)
        {
            if (trigger.name.Contains("Forward"))//Front
            {
                if (mainObject.transform.position.x < length)
                {
                    mainObject.transform.Translate(Vector3.right * Time.deltaTime * speed);//Right, for some reason
                }
            }

            if (trigger.name.Contains("Back"))
            {
                if (mainObject.transform.position.x > length)
                {
                    mainObject.transform.Translate(Vector3.left * Time.deltaTime * speed);//Left
                }
            }

            if (trigger.name.Contains("Right"))
            {
                if (mainObject.transform.position.z > length)
                {
                    mainObject.transform.Translate(Vector3.back * Time.deltaTime * speed);//Back, Idk why
                }
            }

            if (trigger.name.Contains("Left"))
            {
                //Forawrd
                if (mainObject.transform.position.z < length)
                {
                    mainObject.transform.Translate(Vector3.forward * Time.deltaTime * speed);//Forward
                }
            }

            if (trigger.name.Contains("Up"))
            {
                if (mainObject.transform.position.y < length)
                {
                    mainObject.transform.Translate(Vector3.up * Time.deltaTime * speed);
                }
            }

            if (trigger.name.Contains("Down"))
            {
                if (mainObject.transform.position.y > length)
                {
                    mainObject.transform.Translate(Vector3.down * Time.deltaTime * speed);
                }
            }
        }
    }

    private void ChangeZRotationByX()
    {
        //mainObject.transform.rotation = new Quaternion(mainObject.transform.rotation.x, mainObject.transform.rotation.y, mainObject.transform.rotation.z + rotation, 1);
        if (mainObject.transform.rotation.eulerAngles.z < 350 || mainObject.transform.rotation.eulerAngles.z > 351)//-10 = 355
        {
            //Debug.Log(mainObject.transform.rotation.eulerAngles.z);
            mainObject.transform.Rotate(0, 0, rotation * Time.deltaTime * speed);
        }
    }

    private void ChangeRotation()
    {
        if (trigger.name.Contains("RotX"))
        {
            mainObject.transform.Rotate(rotation * Time.deltaTime * speed, 0, 0);
        }
        else if (trigger.name.Contains("RotY"))
        {
            mainObject.transform.Rotate(0, rotation * Time.deltaTime * speed, 0);
        }
        else if (trigger.name.Contains("RotZ"))
        {
            mainObject.transform.Rotate(0, 0, rotation * Time.deltaTime * speed);
        }
    }

    private IEnumerator Explode()//TODO , remove/destroy for now
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Destroy(mainObject);
        }
    }

    private void Activate()
    {
        mainObject.SetActive(true);
    }

    private void Deactivate()
    {
        mainObject.SetActive(false);
    }

    private void OpenDoor()
    {
        //if left or right

        if (trigger.name.Contains("Left"))
        {
            //Debug.Log(mainObject.transform.rotation.eulerAngles.y);

            if (mainObject.transform.rotation.eulerAngles.y <= originalRotation.y - rotation || mainObject.transform.rotation.eulerAngles.y > originalRotation.y - rotation + 5)//REMEMBER Unity Doesnt support minus rotations, also 5 is too big, but 1 is too low, fix
            {
                mainObject.transform.Rotate(0, -rotation * Time.deltaTime * speed, 0);
            }

            //mainObject.transform.Translate(mainObject.transform.position.x + 2, transform.position.y, transform.position.z + 16);//Gets the fck out
            //speed = speed * length
            if (mainObject.transform.position.x != lengthDoor.x)
            {
                mainObject.transform.Translate(Vector3.left * speedDoor * lengthDoor.x * Time.deltaTime);//TODO Left is hardcoded

                //mainObject.transform.Translate(mainObject.transform.position.x + lengthDoor.x, 0, 0);//TODO Stopped here
            }

            //16z 2x roty90
        }
        else if (trigger.name.Contains("Right"))
        {
            if (mainObject.transform.rotation.eulerAngles.y <= rotation)//REMEMBER Euler Angles = actual rotation not quaterinion? //Right rot90, speed1
            {
                mainObject.transform.Rotate(0, rotation * Time.deltaTime * speed, 0);
            }
        }

        if (trigger.name.Contains("RotX"))
        {
            mainObject.transform.Rotate(rotation * Time.deltaTime * speed, 0, 0);
        }
        else if (trigger.name.Contains("RotY"))
        {
            mainObject.transform.Rotate(0, rotation * Time.deltaTime * speed, 0);
        }
        else if (trigger.name.Contains("RotZ"))
        {
            mainObject.transform.Rotate(0, 0, rotation * Time.deltaTime * speed);
        }
    }

    private void RotationCheckXYZ()//0
    {
        if (mainObject.transform.rotation.x == 0)
        {
            originalRotation.x = 360;
        }

        if (mainObject.transform.rotation.y == 0)
        {
            originalRotation.y = 360;
        }

        if (mainObject.transform.rotation.z == 0)
        {
            originalRotation.z = 360;
        }
    }
}