using UnityEngine;

public static class ExplosionManager
{
    public const float Radius = 2.5f;
    public const float Force = 500f;
    public const float Damage = 100f;

    public static void Create(Vector3 position)
    {
        CreateVisual(position);
        ApplyExplosion(position);
    }

    private static void CreateVisual(Vector3 position)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Explosion";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * Radius * 2;

        var mat = go.GetComponent<Renderer>().material;
        mat.shader = Shader.Find("Particles/Standard Unlit");
        mat.color = new Color(1f, 0.5f, 0f);

        Object.Destroy(go.GetComponent<Collider>());
        Object.Destroy(go, 0.3f);
    }

    private static void ApplyExplosion(Vector3 position)
    {
        var colliders = Physics.OverlapSphere(position, Radius);

        foreach (var col in colliders)
        {
            var rb = col.attachedRigidbody;
            if (rb == null) continue;

            if (rb.isKinematic)
            {
                rb.isKinematic = false;
                rb.WakeUp();
            }

            rb.AddExplosionForce(Force, position, Radius);

            var destructible = col.GetComponentInParent<Destructible>();
            if (destructible != null)
            {
                float distance = Vector3.Distance(position, col.transform.position);
                float ratio = 1f - Mathf.Clamp01(distance / Radius);
                destructible.TakeDamage(Damage * ratio);
            }
        }
    }
}