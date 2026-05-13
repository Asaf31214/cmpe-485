using UnityEngine;
using System;

public class CannonController : MonoBehaviour
{
    public static event Action<Vector3, Vector3, float> OnFire;

    public float YawSpeed = 60f;
    public float PitchSpeed = 200f;
    public float MinPitch = 10f;
    public float MaxPitch = 70f;
    public float PowerSpeed = 10f;

    public Transform Muzzle { get; private set; }
    public float Power { get; private set; }

    public float MinPower => LevelData.CannonPowerMin;
    public float MaxPower => LevelData.CannonPowerMax;

    private GameObject basePivot;
    private GameObject barrelPivot;
    private float yaw;
    private float pitch = 45f;

    private void Start()
    {
        CreateCannon();
        Power = MinPower;
    }

    private void CreateCannon()
    {
        basePivot = new GameObject("CannonBase");
        basePivot.transform.SetParent(transform);
        basePivot.transform.position = new Vector3(0, 0, -25f);

        var mainMat = new Material(Shader.Find("Diffuse"));
        mainMat.color = new Color(0.3f, 0.3f, 0.3f);

        var darkMat = new Material(Shader.Find("Diffuse"));
        darkMat.color = new Color(0.2f, 0.2f, 0.2f);

        CreateAxle(basePivot.transform, mainMat);
        CreateWheels(basePivot.transform, mainMat);
        CreateFrame(basePivot.transform, darkMat);

        barrelPivot = new GameObject("BarrelPivot");
        barrelPivot.transform.SetParent(basePivot.transform);
        barrelPivot.transform.localPosition = new Vector3(0, 1.7f, 0.5f);

        CreateBarrel(barrelPivot.transform, mainMat, darkMat);

        Muzzle = new GameObject("Muzzle").transform;
        Muzzle.SetParent(barrelPivot.transform);
        Muzzle.localPosition = new Vector3(0, 0, 3.2f);
    }

    private void CreateAxle(Transform parent, Material mat)
    {
        var axle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        axle.name = "Axle";
        axle.transform.SetParent(parent);
        axle.transform.localScale = new Vector3(0.2f, 1.5f, 0.2f);
        axle.transform.localRotation = Quaternion.Euler(0, 0, 90);
        axle.transform.localPosition = new Vector3(0, 1.2f, 0);
        axle.GetComponent<Renderer>().material = mat;
        Destroy(axle.GetComponent<Collider>());
    }

    private void CreateWheels(Transform parent, Material mat)
    {
        for (int i = 0; i < 2; i++)
        {
            var wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = $"Wheel{i}";
            wheel.transform.SetParent(parent);
            wheel.transform.localScale = new Vector3(2.4f, 0.15f, 2.4f);
            wheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
            wheel.transform.localPosition = new Vector3(i == 0 ? 1.1f : -1.1f, 1.2f, 0);
            wheel.GetComponent<Renderer>().material = mat;
            Destroy(wheel.GetComponent<Collider>());
        }
    }

    private void CreateFrame(Transform parent, Material mat)
    {
        var frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frame.name = "Frame";
        frame.transform.SetParent(parent);
        frame.transform.localScale = new Vector3(0.6f, 1.2f, 0.5f);
        frame.transform.localPosition = new Vector3(0, 1.1f, 0);
        frame.GetComponent<Renderer>().material = mat;
        Destroy(frame.GetComponent<Collider>());
    }

    private void CreateBarrel(Transform parent, Material mainMat, Material darkMat)
    {
        var tube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tube.name = "Tube";
        tube.transform.SetParent(parent);
        tube.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        tube.transform.localScale = new Vector3(1.0f, 3f, 1.0f);
        tube.transform.localPosition = new Vector3(0, 0, 1.5f);
        tube.GetComponent<Renderer>().material = mainMat;
        Destroy(tube.GetComponent<Collider>());

        var cap = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cap.name = "Cap";
        cap.transform.SetParent(parent);
        cap.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        cap.transform.localScale = new Vector3(1.36f, 0.1f, 1.36f);
        cap.transform.localPosition = Vector3.zero;
        cap.GetComponent<Renderer>().material = darkMat;
        Destroy(cap.GetComponent<Collider>());

        var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "MuzzleRing";
        ring.transform.SetParent(parent);
        ring.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        ring.transform.localScale = new Vector3(1.5f, 0.08f, 1.5f);
        ring.transform.localPosition = new Vector3(0, 0, 3f);
        ring.GetComponent<Renderer>().material = darkMat;
        Destroy(ring.GetComponent<Collider>());
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
            Power = Mathf.Min(Power + PowerSpeed * Time.deltaTime, MaxPower);
        if (Input.GetKey(KeyCode.V))
            Power = Mathf.Max(Power - PowerSpeed * Time.deltaTime, MinPower);

        if (Input.GetKeyDown(KeyCode.Space))
            OnFire?.Invoke(Muzzle.position, Muzzle.forward.normalized, Power);
    }

    private void ApplyRotation()
    {
        basePivot.transform.localEulerAngles = new Vector3(0, yaw, 0);
        barrelPivot.transform.localEulerAngles = new Vector3(-pitch, 0, 0);
    }

    public Vector3 GetFireDirection() => Muzzle.forward.normalized;
}