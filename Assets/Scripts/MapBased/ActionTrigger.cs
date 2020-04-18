using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTrigger : MonoBehaviour {

    //OLD, DONT USE

    public GameObject mainObject;

    public float speed;

    public string choice;

    public static bool activated;

    //-1.4 SlidingDoor
	
	void Start () {
        //mainObject = this.gameObject;
        mainObject = this.gameObject.transform.GetChild(0).gameObject;
        //mainObject.transform.position = moveUp();
        
	}
	
	
	void Update () {
        if (activated)
        {
            if (choice == "Up" || choice == "up")
            {
                MoveUp();//TODO Make it based on gameobjects name? If it has up, go up, if it has down, go down?
            }

            else if (choice == "Down" || choice == "down")
            {
                MoveDown();
            }
        }
        
        
	}


    public void MoveUp()
    {
        if(mainObject.transform.localPosition.y < 4.1f)
        {
            mainObject.transform.Translate(Vector3.up * Time.deltaTime * speed);
        }
        
        
    }

    public void MoveDown()
    {
        if (mainObject.transform.localPosition.y > -1.4f)
        {
            mainObject.transform.Translate(Vector3.down * Time.deltaTime * speed);
        }

    }

}
