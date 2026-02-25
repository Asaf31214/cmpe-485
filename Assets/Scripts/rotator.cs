using UnityEngine;

public class rotator : MonoBehaviour
{
    public float speed = 50f;

    void Update()
    {
        transform.Rotate(Vector3.down, speed * Time.deltaTime);
    }
}