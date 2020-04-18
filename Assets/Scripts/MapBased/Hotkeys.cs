using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.Linq;

public class Hotkeys : MonoBehaviour {

    public GameObject player;
    public GameObject respawnPoint;

    public GameObject[] checkpoints;

    public EscMenu escMenu;

    public bool cheating = false;

	// Use this for initialization
	void Start () {
        //amountOfCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

	}
	
	// Update is called once per frame
	void Update () {

        if (!escMenu.paused)//If game is not paused
        {
            CheckForInput();

            //"Checkpoints" (cheating)
            Cheating();
        }
        

        
        
        
	}

    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.T))//Player's position back to start
        {
            //TODO, instead of resetting the position, reset the scene?


            player.transform.position = respawnPoint.transform.position;
            player.transform.rotation = respawnPoint.transform.rotation;//Doesnt fix the problem

            //Reset speed as well
            GetComponent<MovementTwo>().playerVelocity = new Vector3(0, 0, 0);
            //Now timer
            Timer.time = 0;
        }

        else if(Input.GetKeyDown(KeyCode.R))//Restart
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

    }

    void Cheating()
    {
        if(cheating == true && escMenu.paused == false)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && checkpoints.Length > 0)//1
            {
                player.transform.position = checkpoints[0].transform.position;
                player.transform.rotation = checkpoints[0].transform.rotation;//Doesnt work cuz of mouse movement?
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && checkpoints.Length > 1)
            {
                player.transform.position = checkpoints[1].transform.position;

            }

            if (Input.GetKeyDown(KeyCode.Alpha3) && checkpoints.Length > 2)
            {
                player.transform.position = checkpoints[2].transform.position;

            }

            if (Input.GetKeyDown(KeyCode.Alpha4) && checkpoints.Length > 3)
            {
                player.transform.position = checkpoints[3].transform.position;

            }

            if (Input.GetKeyDown(KeyCode.Alpha5) && checkpoints.Length > 4)
            {
                player.transform.position = checkpoints[4].transform.position;

            }

            if (Input.GetKeyDown(KeyCode.Alpha6) && checkpoints.Length > 5)
            {
                player.transform.position = checkpoints[5].transform.position;

            }


            checkpoints.OrderBy(gameObject => gameObject.name);//TODO Doesn't work as intended, I think
        }

        else if (!cheating)
        {
            //U cant cheat
        }

    }


}
