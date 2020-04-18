using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    private void Start()
    {
    }

    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            //Movement.inAir = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            //  Movement.inAir = true;
        }
    }
}