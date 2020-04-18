using UnityEngine;

using UnityEngine.UI;

public class MovementSettings : MonoBehaviour
{
    private GameObject player;

    public InputField inputField;

    public Toggle toggle;

    private bool grounded = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        /*if(player.GetComponent<Movement>().grounded != grounded)
        {
            GroundedChanged();
        }*/
    }

    public void GroundedChanged()
    {
        if (grounded)
        {
            grounded = false;
            toggle.isOn = grounded;
        }
        else if (!grounded)
        {
            grounded = true;
            toggle.isOn = grounded;
        }
    }

    public void SetFriction()
    {
    }
}