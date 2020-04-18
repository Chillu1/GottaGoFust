using UnityEngine;

public class camrotation : MonoBehaviour
{
    public int auto = 90;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        this.gameObject.transform.rotation = new Quaternion(auto, auto, auto, 1);
    }
}