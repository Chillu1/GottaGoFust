﻿using UnityEngine;

public class DefineObject : MonoBehaviour
{
    public bool isWall;

    public bool isGround;

    public bool isTrigger;

    public bool isLocal;

    //What kinda of trigger, String?
    public object Object;

    public string function;//? //If function = X editor add rot/speed or some shit
    public string test;

    public string[] functions = new string[] { "Wall", "Ground", "Trigger" };
    public int index = 0;

    //Trigger
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

    /* static void Init()
     {
         EditorWindow window = GetWindow(typeof(DefineObject));
         window.Show();
     }*/

    private void Start()
    {
    }

    private void Update()
    {
    }
}