using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public static SceneSetup Instance { get; private set; }

    public GameObject Ground { get; private set; }
    public Light DirectionalLight { get; private set; }
    public CannonController Cannon { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CreateGround();
        CreateLighting();
        SetupCamera();
        CreateCannon();
    }

    private void CreateCannon()
    {
        var cannonGO = new GameObject("Cannon");
        Cannon = cannonGO.AddComponent<CannonController>();
    }

    private void CreateGround()
    {
        Ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        Ground.name = "Ground";
        Ground.transform.position = new Vector3(0, 0, 0);
        Ground.GetComponent<Renderer>().material.color = new Color(0.3f, 0.5f, 0.3f);
    }

    private void CreateLighting()
    {
        var lightGO = new GameObject("DirectionalLight");
        DirectionalLight = lightGO.AddComponent<Light>();
        DirectionalLight.type = LightType.Directional;
        DirectionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        DirectionalLight.intensity = 1f;
    }

    private void SetupCamera()
    {
        Camera.main.transform.position = new Vector3(0, 8, -18);
        Camera.main.transform.LookAt(new Vector3(0, 3, 10));
    }
}
