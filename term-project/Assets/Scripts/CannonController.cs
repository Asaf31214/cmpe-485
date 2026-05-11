using UnityEngine;
using System;

public class CannonController : MonoBehaviour
{
    public static event Action<Vector3, Vector3, float> OnFire;
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
        basePivot = new GameObject("CannonBase");
        basePivot.transform.position = new Vector3(0, 0, -25f);

        var mainMat = new Material(Shader.Find("Diffuse"));
        mainMat.color = new Color(0.3f, 0.3f, 0.3f);

        var darkMat = new Material(Shader.Find("Diffuse"));
        darkMat.color = new Color(0.2f, 0.2f, 0.2f);

        // Axle
        var axle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        axle.name = "Axle";
        axle.transform.SetParent(basePivot.transform);
        axle.transform.localScale = new Vector3(0.2f, 1.5f, 0.2f);
        axle.transform.localRotation = Quaternion.Euler(0, 0, 90);
        axle.transform.localPosition = new Vector3(0, 1.2f, 0);
        axle.GetComponent<Renderer>().material = mainMat;
        Destroy(axle.GetComponent<Collider>());

        // Left wheel
        var wheelL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelL.name = "WheelL";
        wheelL.transform.SetParent(basePivot.transform);
        wheelL.transform.localScale = new Vector3(2.4f, 0.15f, 2.4f);
        wheelL.transform.localRotation = Quaternion.Euler(0, 0, 90);
        wheelL.transform.localPosition = new Vector3(1.1f, 1.2f, 0);
        wheelL.GetComponent<Renderer>().material = mainMat;
        Destroy(wheelL.GetComponent<Collider>());

        // Right wheel
        var wheelR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wheelR.name = "WheelR";
        wheelR.transform.SetParent(basePivot.transform);
        wheelR.transform.localScale = new Vector3(2.4f, 0.15f, 2.4f);
        wheelR.transform.localRotation = Quaternion.Euler(0, 0, 90);
        wheelR.transform.localPosition = new Vector3(-1.1f, 1.2f, 0);
        wheelR.GetComponent<Renderer>().material = mainMat;
        Destroy(wheelR.GetComponent<Collider>());

        // Support frame (holds barrel above axle)
        var frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = "Frame";
        frame.transform.SetParent(basePivot.transform);
        frame.transform.localScale = new Vector3(0.6f, 1.2f, 0.5f);
        frame.transform.localPosition = new Vector3(0, 1.1f, 0);
        frame.GetComponent<Renderer>().material = darkMat;
        Destroy(frame.GetComponent<Collider>());

        // Barrel pivot (pitch)
        barrelPivot = new GameObject("BarrelPivot");
        barrelPivot.transform.SetParent(basePivot.transform);
        barrelPivot.transform.localPosition = new Vector3(0, 1.7f, 0.5f);

        // Main tube (hollow cylinder)
        var tube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tube.name = "Tube";
        tube.transform.SetParent(barrelPivot.transform);
        tube.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        tube.transform.localScale = new Vector3(1.0f, 3f, 1.0f);
        tube.GetComponent<Renderer>().material = mainMat;
        Destroy(tube.GetComponent<Collider>());
        tube.transform.localPosition = new Vector3(0, 0, 1.5f);

        // Back cap (closes the back)
        var cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cap.name = "Cap";
        cap.transform.SetParent(barrelPivot.transform);
        cap.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        cap.transform.localScale = new Vector3(1.36f, 0.1f, 1.36f);
        cap.GetComponent<Renderer>().material = darkMat;
        Destroy(cap.GetComponent<Collider>());
        cap.transform.localPosition = new Vector3(0, 0, 0);

        // Muzzle ring (front edge)
        var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "MuzzleRing";
        ring.transform.SetParent(barrelPivot.transform);
        ring.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        ring.transform.localScale = new Vector3(1.5f, 0.08f, 1.5f);
        ring.GetComponent<Renderer>().material = darkMat;
        Destroy(ring.GetComponent<Collider>());
        ring.transform.localPosition = new Vector3(0, 0, 3f);

        Muzzle = new GameObject("Muzzle").transform;
        Muzzle.SetParent(barrelPivot.transform);
        Muzzle.localPosition = new Vector3(0, 0, 3.2f);
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

        if (Input.GetKey(KeyCode.C))
        {
            Power = Mathf.Min(Power + PowerSpeed * Time.deltaTime, MaxPower);
        }
        if (Input.GetKey(KeyCode.V))
        {
            Power = Mathf.Max(Power - PowerSpeed * Time.deltaTime, MinPower);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnFire?.Invoke(GetMuzzlePosition(), GetFireDirection(), Power);
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
