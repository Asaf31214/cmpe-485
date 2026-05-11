using UnityEngine;

public class Projectile : MonoBehaviour
{
    private const float Radius = 0.3f;

    private Rigidbody rb;

    public static GameObject Create(Vector3 position, Vector3 direction, float power)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Projectile";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * Radius * 2;

        var renderer = go.GetComponent<Renderer>();
        renderer.material.color = Color.black;

        var collider = go.GetComponent<SphereCollider>();
        var material = new PhysicMaterial
        {
            dynamicFriction = 0.3f,
            staticFriction = 0.2f,
            bounciness = 0.6f,
            frictionCombine = PhysicMaterialCombine.Multiply,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        collider.material = material;

        var projectile = go.AddComponent<Projectile>();
        projectile.rb = go.AddComponent<Rigidbody>();
        projectile.rb.mass = 5f;
        projectile.rb.useGravity = true;
        projectile.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        go.GetComponent<Rigidbody>().velocity = direction * power;

        return go;
    }
}
