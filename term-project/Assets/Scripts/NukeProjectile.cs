using UnityEngine;
using System.Collections;

public class NukeProjectile : MonoBehaviour
{
    private const float FallSpeed = 50f;
    private const float Radius = 30f;
    private const float Force = 2000f;
    private const float Damage = 500f;

    private static AudioClip _explosionClip;
    public static bool UseOptimized { get; set; } = false;

    private bool exploded;

    public static void Create(Vector3 targetPosition)
    {
        var go = new GameObject("Nuke");
        go.transform.position = new Vector3(targetPosition.x, 100f, targetPosition.z);

        var collider = go.AddComponent<SphereCollider>();
        collider.radius = 1f;
        collider.isTrigger = true;

        var mesh = go.AddComponent<MeshRenderer>();
        mesh.material = new Material(Shader.Find("Diffuse"));
        mesh.material.color = UseOptimized ? Color.cyan : Color.yellow;

        var filter = go.AddComponent<MeshFilter>();
        filter.mesh = CreateSphereMesh(1f);

        go.AddComponent<NukeProjectile>();
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
        transform.position += Vector3.down * FallSpeed * Time.deltaTime;

        if (transform.position.y <= 0.5f && !exploded)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        CreateVisual();
        PlaySound();

        int blocksBefore = Object.FindObjectsOfType<Destructible>().Length;
        PerformanceLogger.LogExplosionStart(blocksBefore);

        if (UseOptimized)
            ApplyExplosionOptimized();
        else
            ApplyExplosion();

        StartCoroutine(LogAfterDestruction(blocksBefore));
    }

    private IEnumerator LogAfterDestruction(int blocksBefore)
    {
        yield return null;
        yield return null;

        int blocksAfter = Object.FindObjectsOfType<Destructible>().Length;
        PerformanceLogger.LogExplosionEnd(blocksBefore - blocksAfter, UseOptimized ? "optimized" : "physics");

    }

    private void PlaySound()
    {
        if (_explosionClip == null)
            _explosionClip = Resources.Load<AudioClip>("explosion");

        if (_explosionClip != null)
        {
            AudioSource.PlayClipAtPoint(_explosionClip, transform.position, 2f);
            AudioSource.PlayClipAtPoint(_explosionClip, Camera.main.transform.position, 10f);
        }
    }

    private void CreateVisual()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "NukeExplosion";
        go.transform.position = transform.position;
        go.transform.localScale = Vector3.one * Radius;

        var mat = go.GetComponent<Renderer>().material;
        mat.shader = Shader.Find("Particles/Standard Unlit");
        mat.color = UseOptimized ? new Color(0f, 1f, 1f, 0.8f) : new Color(1f, 0.3f, 0f, 0.8f);

        Object.Destroy(go.GetComponent<Collider>());
        var visual = go.AddComponent<ExplosionVisual>();
        visual.Duration = 2f;
        visual.MaxScale = 2f;
    }

    private void ApplyExplosion()
    {
        var colliders = Physics.OverlapSphere(transform.position, Radius);

        foreach (var col in colliders)
        {
            var rb = col.attachedRigidbody;
            if (rb != null)
                rb.AddExplosionForce(Force, transform.position, Radius);

            var destructible = col.GetComponentInParent<Destructible>();
            if (destructible != null)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                float ratio = 1f - Mathf.Clamp01(distance / Radius);
                destructible.TakeDamage(Damage * ratio);
            }
        }
    }

    private void ApplyExplosionOptimized()
    {
        var blocks = CastleBuilder.GetCachedBlocks();

        foreach (var block in blocks)
        {
            if (block == null) continue;

            Vector3 blockPos = block.transform.position;
            float distance = Vector3.Distance(transform.position, blockPos);

            if (distance <= Radius)
            {
                float ratio = 1f - Mathf.Clamp01(distance / Radius);
                block.TakeDamage(Damage * ratio);
            }
        }
    }
}