using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    private void Awake()
    {
        SetupPhysics();
        CreateGround();
        CreateLighting();
        SetupCamera();
        CreateCannon();
        CreateGameManager();
    }

    private void SetupPhysics()
    {
        Physics.sleepThreshold = 0.005f;
        Physics.defaultContactOffset = 0.01f;
        Time.fixedDeltaTime = 0.005f;
        Physics.defaultSolverIterations = 10;
        Physics.defaultSolverVelocityIterations = 10;
    }

    private void CreateGround()
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = Vector3.one * 100;

        var mat = new Material(Shader.Find("Diffuse"));
        mat.color = new Color(0.2f, 0.5f, 0.1f);
        ground.GetComponent<Renderer>().material = mat;

        Destroy(ground.GetComponent<Collider>());
        var collider = ground.AddComponent<BoxCollider>();
        collider.size = new Vector3(1000, 1, 1000);
        collider.center = new Vector3(0, -0.5f, 0);
    }

    private void CreateLighting()
    {
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

        var mainLight = new GameObject("MainLight");
        var light = mainLight.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        light.intensity = 1.2f;
        light.color = new Color(1f, 0.95f, 0.85f);

        var fillLight = new GameObject("FillLight");
        var fill = fillLight.AddComponent<Light>();
        fill.type = LightType.Directional;
        fill.transform.rotation = Quaternion.Euler(30f, 150f, 0f);
        fill.intensity = 0.3f;
        fill.color = new Color(0.6f, 0.7f, 1f);
    }

    private void SetupCamera()
    {
        Camera.main.transform.position = new Vector3(-15, 12, -40);
        Camera.main.transform.LookAt(new Vector3(0, 5, 15));
    }

    private void CreateCannon()
    {
        var cannonGO = new GameObject("Cannon");
        cannonGO.AddComponent<CannonController>();
    }

    private void CreateGameManager()
    {
        var gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();
        gmGO.AddComponent<GameUI>();
        gmGO.AddComponent<TrajectoryPreview>();
        gmGO.AddComponent<PerformanceTest>();
    }
}