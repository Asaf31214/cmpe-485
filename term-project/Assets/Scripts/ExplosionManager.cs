using UnityEngine;

public static class ExplosionManager
{
    public const float Radius = 2.5f;
    public const float Force = 500f;
    public const float Damage = 100f;

    private static AudioClip _explosionClip;

    public static void Create(Vector3 position)
    {
        CreateVisual(position);
        PlaySound(position);
        ApplyExplosion(position);
    }

    private static void PlaySound(Vector3 position)
    {
        if (_explosionClip == null)
            _explosionClip = Resources.Load<AudioClip>("explosion");

        if (_explosionClip != null)
            AudioSource.PlayClipAtPoint(_explosionClip, position);
    }

    private static void CreateVisual(Vector3 position)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Explosion";
        go.transform.position = position;
        go.transform.localScale = Vector3.one * Radius * 0.5f;

        var mat = go.GetComponent<Renderer>().material;
        mat.shader = Shader.Find("Particles/Standard Unlit");
        mat.color = new Color(1f, 0.5f, 0f, 1f);

        Object.Destroy(go.GetComponent<Collider>());
        go.AddComponent<ExplosionVisual>();
    }

    private static void ApplyExplosion(Vector3 position)
    {
        var colliders = Physics.OverlapSphere(position, Radius);

        foreach (var col in colliders)
        {
            var rb = col.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(Force, position, Radius);
            }

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