using UnityEngine;

public class CannonController : MonoBehaviour
{
    public float YawSpeed = 250f;
    public float PitchSpeed = 200f;
    public float MinPitch = 10f;
    public float MaxPitch = 70f;

    public float Power { get; private set; } = 15f;
    public float MinPower = 10f;
    public float MaxPower = 30f;
    public float PowerSpeed = 10f;

    private GameObject basePivot;
    private GameObject barrelPivot;
    private float yaw;
    private float pitch = 45f;

    public Transform Muzzle { get; private set; }

    private void Start()
    {
        CreateCannon();
        Power = MinPower;
    }

    private void CreateCannon()
    {
        // Base
        basePivot = new GameObject("CannonBase");
        basePivot.transform.position = new Vector3(0, 0.3f, -8f);

        var baseMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        baseMesh.name = "Base";
        baseMesh.transform.SetParent(basePivot.transform);
        baseMesh.transform.localPosition = Vector3.zero;
        baseMesh.transform.localScale = new Vector3(1.2f, 0.6f, 1.2f);
        baseMesh.GetComponent<Renderer>().material.color = Color.gray;
        Destroy(baseMesh.GetComponent<Collider>());

        // Barrel pivot (for pitch) — sits on top of base
        barrelPivot = new GameObject("BarrelPivot");
        barrelPivot.transform.SetParent(basePivot.transform);
        barrelPivot.transform.localPosition = new Vector3(0, 0.4f, 0);

        // Barrel — cylinder rotated to point along Z
        var barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "Barrel";
        barrel.transform.SetParent(barrelPivot.transform);
        barrel.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        barrel.transform.localScale = new Vector3(0.25f, 2f, 0.25f);
        barrel.GetComponent<Renderer>().material.color = new Color(0.35f, 0.35f, 0.35f);
        Destroy(barrel.GetComponent<Collider>());
        // Offset so back end is at pivot (cylinder half-length in local Y = 1.0 after scale)
        barrel.transform.localPosition = new Vector3(0, 0, 2f);

        // Muzzle point at tip of barrel
        Muzzle = new GameObject("Muzzle").transform;
        Muzzle.SetParent(barrelPivot.transform);
        Muzzle.localPosition = new Vector3(0, 0, 4f);
    }

    private void Update()
    {
        HandleInput();
        ApplyRotation();
    }

    private void HandleInput()
    {
        yaw += Input.GetAxis("Horizontal") * YawSpeed * Time.deltaTime;
        pitch += Input.GetAxis("Vertical") * PitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);

        if (Input.GetKey(KeyCode.PageUp))
        {
            Power = Mathf.Min(Power + PowerSpeed * Time.deltaTime, MaxPower);
        }
        if (Input.GetKey(KeyCode.PageDown))
        {
            Power = Mathf.Max(Power - PowerSpeed * Time.deltaTime, MinPower);
        }
    }

    private void ApplyRotation()
    {
        basePivot.transform.localEulerAngles = new Vector3(0, yaw, 0);
        barrelPivot.transform.localEulerAngles = new Vector3(-pitch, 0, 0);
    }

    public Vector3 GetFireDirection()
    {
        return Muzzle.forward.normalized;
    }

    public Vector3 GetMuzzlePosition()
    {
        return Muzzle.position;
    }
}
