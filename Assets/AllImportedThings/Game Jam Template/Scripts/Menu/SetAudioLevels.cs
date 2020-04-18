using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetAudioLevels : MonoBehaviour {

	public AudioMixer mainMixer;					//Used to hold a reference to the AudioMixer mainMixer

    public MouseMovement mouseMovemement;

    public InputField inputField;

    public InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();

	//Call this function and pass in the float parameter musicLvl to set the volume of the AudioMixerGroup Music in mainMixer
	public void SetMusicLevel(float musicLvl)
	{
		mainMixer.SetFloat("musicVol", musicLvl);
	}

	//Call this function and pass in the float parameter sfxLevel to set the volume of the AudioMixerGroup SoundFx in mainMixer
	public void SetSfxLevel(float sfxLevel)
	{
		mainMixer.SetFloat("sfxVol", sfxLevel);
	}

    /*
    public void SetSensivity(float sensivity)
    {
        //submitEvent.AddListener();
        mouseMovemement = Camera.main.GetComponent<MouseMovement>();

        inputField.onEndEdit.AddListener(SubmitString);//NullReference

        
        //inputField = 
        //mouseMovemement.sensitivityX = GetComponent<InputField>().

    }*/

   

}
