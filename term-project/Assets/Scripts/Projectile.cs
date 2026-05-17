using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const float Radius = 0.3f;
    private const float MinImpactVelocity = 2f;

    public Rigidbody Rigidbody { get; private set; }

    private bool exploded;

    public static Projectile Create(Vector3 position, Vector3 direction, float power)
    {
        var go = new GameObject("Projectile");
        go.transform.position = position;

        var collider = go.AddComponent<SphereCollider>();
        collider.radius = Radius;
        collider.material = new PhysicMaterial
        {
            dynamicFriction = 0.3f,
            staticFriction = 0.2f,
            bounciness = 0.6f,
            frictionCombine = PhysicMaterialCombine.Multiply,
            bounceCombine = PhysicMaterialCombine.Maximum
        };

        var mesh = go.AddComponent<MeshRenderer>();
        mesh.material = new Material(Shader.Find("Diffuse"));
        mesh.material.color = Color.black;

        var filter = go.AddComponent<MeshFilter>();
        filter.mesh = CreateSphereMesh(Radius);

        var projectile = go.AddComponent<Projectile>();
        projectile.Rigidbody = go.AddComponent<Rigidbody>();
        projectile.Rigidbody.mass = 5f;
        projectile.Rigidbody.useGravity = true;
        projectile.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        projectile.Rigidbody.velocity = direction * power;

        go.AddComponent<ProjectileCollisionHandler>();

        return projectile;
    }

    private static Mesh CreateSphereMesh(float radius)
    {
        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = Vector3.one * radius * 2;
        var mesh = sphere.GetComponent<MeshFilter>().mesh;
        Destroy(sphere);
        return mesh;
    }

    private void Update()
    {
        if (!exploded && transform.position.y < -10f)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        Rigidbody.isKinematic = true;
        ExplosionManager.Create(transform.position);
        Destroy(gameObject, 0.3f);
    }
}

public class ProjectileCollisionHandler : MonoBehaviour
{
    private const float MinImpactVelocity = 2f;

    private Projectile projectile;

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > MinImpactVelocity)
        {
            projectile.Explode();
        }
    }
}