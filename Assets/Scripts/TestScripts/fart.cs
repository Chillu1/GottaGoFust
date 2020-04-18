using UnityEngine;

public class fart : MonoBehaviour
{
    public GameObject go;
    public float speed = 300f;
    public Rigidbody rb;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1)
        {
            rb.AddForce(transform.forward * speed);
            timer = 0;
        }
    }
}