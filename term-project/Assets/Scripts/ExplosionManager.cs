using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public const float Radius = 5f;
    public const float Force = 500f;
    public const float Damage = 100f;

    public static void Create(Vector3 position)
    {
        // Visual marker
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Explosion";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * Radius * 2;
        var mat = go.GetComponent<Renderer>().material;
        mat.color = new Color(1f, 0.5f, 0f, 0.5f);
        mat.shader = Shader.Find("Particles/Standard Unlit");
        Destroy(go.GetComponent<Collider>());
        Destroy(go, 0.3f);

        // Radial force on all nearby rigidbodies
        var colliders = Physics.OverlapSphere(position, Radius);
        foreach (var col in colliders)
        {
            var rb = col.attachedRigidbody;
            if (rb == null) continue;
            
            // Wake up kinematic targets
            if (rb.isKinematic)
            {
                rb.isKinematic = false;
                rb.WakeUp();
            }
            
            rb.AddExplosionForce(Force, position, Radius);

            // Distance-based damage to destructible objects
            var destructible = col.GetComponentInParent<Destructible>();
            if (destructible != null)
            {
                float distance = Vector3.Distance(position, col.transform.position);
                float ratio = 1f - (distance / Radius);
                destructible.TakeDamage(Damage * ratio);
            }
        }
    }
}
