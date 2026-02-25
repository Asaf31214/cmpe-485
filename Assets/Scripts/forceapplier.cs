using UnityEngine;

public class ForceApplier : MonoBehaviour
{
    public Rigidbody rb;
    public float force = 1f;

    void FixedUpdate()
    {
        rb.AddForce(Vector3.forward * force);
    }
}