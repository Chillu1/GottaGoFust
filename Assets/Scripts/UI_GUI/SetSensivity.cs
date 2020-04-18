using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SetSensivity : MonoBehaviour {

    public MouseMovement mouseMovement;

    public GlobalControl globalControl;

    public InputField inputField;

    public string inputFieldText;

    public Slider slider;

	void Start () {

        globalControl = GameObject.Find("Global Control").GetComponent<GlobalControl>();//REMEMBER Find
       // mouseMovement.sensitivityX = globalControl.sensivity;
       // mouseMovement.sensitivityY = globalControl.sensivity;

    }
	
	
	void Update () {
		
	}



    public void SetSensitivityFunction()
    {
        
        //inputField.onEndEdit.AddListener("asd");

        mouseMovement.sensitivityX = float.Parse(inputField.text);
        mouseMovement.sensitivityY = float.Parse(inputField.text);

        try
        {
            globalControl.sensivity = float.Parse(inputField.text);
        }

        catch (System.Exception e)
        {
            Debug.LogException(e, this);
            globalControl = GameObject.Find("Global Control").GetComponent<GlobalControl>();
        }

        SaveSensivity();

    }

    public void SetSensitivityFunctionSlider()
    {

        mouseMovement.sensitivityX = slider.value;
        mouseMovement.sensitivityY = slider.value;

        try
        {
            globalControl.sensivity = slider.value;
        }
        
        catch(System.Exception e)
        {
            Debug.LogException(e, this);
            globalControl = GameObject.Find("Global Control").GetComponent<GlobalControl>();
        }

        SaveSensivity();

    }

    public void SaveSensivity()
    {
        
        GlobalControl.WriteSensivity(globalControl.sensivity);
    }

}
