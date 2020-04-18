using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camrotation : MonoBehaviour {
	
	public int auto = 90;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.gameObject.transform.rotation = new Quaternion (auto, auto, auto, 1);
	}
}
