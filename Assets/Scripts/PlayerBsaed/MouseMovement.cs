using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour {

    public static MouseMovement mouseMovement;

    public EscMenu escMenu;

    private GameObject player;
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float minimumX = -360f;
    public float maximumX = 360f;
    public float minimumY = -90f;
    public float maximumY = 90f;
    float rotationY = 0f;

    public float rotationX = 0f;

    Transform mainCamera;

    public Quaternion cameraRotation = Quaternion.identity;

    void Update()
    {
        if(escMenu.paused == false)
        {

            

        if (axes == RotationAxes.MouseXAndY)
        {
            rotationX = transform.localEulerAngles.y + Input.GetAxisRaw("Mouse X") * sensitivityX;
                //Debug.Log("ROtationX: " + rotationX);
                rotationY += Input.GetAxisRaw("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            //PlayerSetRotation();
        }
        else if (axes == RotationAxes.MouseX)
        {
            transform.Rotate(0, Input.GetAxisRaw("Mouse X") * sensitivityX, 0);
            //PlayerSetRotation();
        }
        else
        {
            rotationY += Input.GetAxisRaw("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            //PlayerSetRotation();
        }

        //&player.transform.Rotate(Camera.main.transform.rotation.eulerAngles);//Not all angles
        if (Mathf.Clamp(player.transform.rotation.y, Camera.main.transform.rotation.y - 2, Camera.main.transform.rotation.y + 2) != Camera.main.transform.rotation.y)
        {
          //  Debug.Log("Clamp Works");
           // player.transform.Rotate(new Quaternion(player.transform.rotation.x, Camera.main.transform.rotation.eulerAngles.y, player.transform.rotation.z, 1).eulerAngles.normalized);

            //player.transform.rotation = new Quaternion(player.transform.rotation.x, Camera.main.transform.rotation.y, player.transform.rotation.z, 1);
            //player.transform.rotation = new Quaternion(player.transform.rotation.x, Camera.main.transform.rotation.eulerAngles.y, player.transform.rotation.z, 1);
        }

        if(Vector3.Distance(Camera.main.transform.rotation.eulerAngles, player.transform.rotation.eulerAngles) < 50 || Vector3.Distance(Camera.main.transform.rotation.eulerAngles, player.transform.rotation.eulerAngles) < -50)//Bugged af
        {
           // Debug.Log("Distance works");
        }


        
        mainCamera.position = player.transform.position;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = Camera.main.transform;

        StartCoroutine(RotatePlayer());

        // Make the rigid body not change rotation
        if (player.GetComponent<Rigidbody>())
            player.GetComponent<Rigidbody>().freezeRotation = true;//TODO Check if this is right
    }

   //player.transform.Rotate(new Quaternion(player.transform.rotation.x, Camera.main.transform.rotation.eulerAngles.y, player.transform.rotation.z, 1).eulerAngles.normalized);
    

    private IEnumerator RotatePlayer()
    {
        while (true)
        {
            //When cameras y rotation is 45 it goes in circles
            //If player rotation is not camera rotation
            //player.transform.Rotate(new Vector3(0, mainCamera.rotation.y, 0));

            //Only + rotation "allowed", 
            //Quaternions are retarded
            //W is how fast U "turn"
            //player.transform.rotation = new Quaternion(0, mainCamera.rotation.y, 0, 0.67f);

            //Make - rotation

            //If rotation is close to where its suppsoed 2 be stop, spins around cuz of reasons
            cameraRotation.eulerAngles = new Vector3(player.transform.rotation.eulerAngles.x, mainCamera.transform.rotation.eulerAngles.y/* / 360*/, player.transform.rotation.eulerAngles.z);
            //Quaterinion works like this?: 180 degrees = 0.5?

            //90 = 0.25 (/360) = 90ø
           

            //BUG Character Controller

            if(Vector3.Distance(player.transform.rotation.eulerAngles, mainCamera.eulerAngles) >= 20)//Rotation on camera is minimal, while quaternion check is huge af
            {
                //Quaternion works perfectly with this eulerangles, 
                player.transform.rotation = cameraRotation; //Camera rotation is different every time the player moves? Also rotation is apperently ok on camera, but it shows up something different in euler angles debug
                
                //For checking/debugging rotations
                /*
                Debug.Log("Player rotation: " + player.transform.rotation.eulerAngles);
                Debug.Log("Camera rotation: " + mainCamera.transform.rotation.eulerAngles);
                Debug.Log("Camera rotation Quaterinion: " + mainCamera.transform.rotation);
                */

            }
            

            //player.transform.LookAt(mainCamera.position);

            yield return new WaitForSeconds(0.01f);
        }
    }

}
