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
        CreateGameManager();
    }

    private void CreateGameManager()
    {
        var gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();
        gmGO.AddComponent<PowerBar>();
        gmGO.AddComponent<GameUI>();
        gmGO.AddComponent<TrajectoryPreview>();
        gmGO.AddComponent<ProfilerDisplay>();
        gmGO.AddComponent<PerformanceTest>();
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
        Ground.transform.localScale = Vector3.one * 100;
        var groundMat = new Material(Shader.Find("Diffuse"));
        groundMat.color = new Color(0.2f, 0.5f, 0.1f);
        Ground.GetComponent<Renderer>().material = groundMat;

        // Use box collider instead of mesh - plane mesh is too thin for physics
        Destroy(Ground.GetComponent<Collider>());
        var groundCollider = Ground.AddComponent<BoxCollider>();
        groundCollider.size = new Vector3(1000, 1, 1000);
        groundCollider.center = new Vector3(0, -0.5f, 0);
    }

    private void CreateLighting()
    {
        // Physics tuning for stacked block stability (Technical Challenge #1)
        Physics.sleepThreshold = 0.005f;
        Physics.defaultContactOffset = 0.01f;
        Time.fixedDeltaTime = 0.005f; // 200 Hz physics step
        Physics.defaultSolverIterations = 10;
        Physics.defaultSolverVelocityIterations = 10;

        RenderSettings.skybox = null;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = new Color(0.4f, 0.65f, 0.9f);

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.6f);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 100f;
        RenderSettings.fogEndDistance = 500f;
        RenderSettings.fogColor = new Color(0.6f, 0.75f, 0.9f);

        var lightGO = new GameObject("DirectionalLight");
        DirectionalLight = lightGO.AddComponent<Light>();
        DirectionalLight.type = LightType.Directional;
        DirectionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        DirectionalLight.intensity = 1.2f;
        DirectionalLight.color = new Color(1f, 0.95f, 0.85f);

        // Ambient fill light from opposite side
        var fillGO = new GameObject("FillLight");
        var fillLight = fillGO.AddComponent<Light>();
        fillLight.type = LightType.Directional;
        fillLight.transform.rotation = Quaternion.Euler(30f, 150f, 0f);
        fillLight.intensity = 0.3f;
        fillLight.color = new Color(0.6f, 0.7f, 1f);
    }

    private void SetupCamera()
    {
        // Angled camera view to see trajectory preview clearly
        Camera.main.transform.position = new Vector3(-15, 12, -40);
        Camera.main.transform.LookAt(new Vector3(0, 5, 15));
    }
}
