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
        Ground.transform.localScale = Vector3.one * 5;
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
        Camera.main.transform.position = new Vector3(0, 15, -45);
        Camera.main.transform.LookAt(new Vector3(0, 3, 15));
    }
}
