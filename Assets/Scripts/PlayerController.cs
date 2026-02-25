using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float force = 10f;

    void FixedUpdate()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ);
        rb.AddForce(movement * force);
    }
}