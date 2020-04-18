using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour {

	


	void Start () {
		
	}
	
	
	void Update () {
	    	
	}


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ground")
        {
            //Movement.inAir = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Ground")
        {
          //  Movement.inAir = true;
        }

    }

}
