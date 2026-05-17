using UnityEngine;

public class ExplosionVisual : MonoBehaviour
{
    public float Duration = 0.8f;
    public float MaxScale = 1.5f;

    private float _elapsed;
    private Vector3 _startScale;
    private Material _material;
    private Color _startColor;

    void Start()
    {
        _elapsed = 0f;
        _startScale = transform.localScale;
        _material = GetComponent<Renderer>().material;
        _startColor = _material.color;
    }

    void Update()
    {
        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / Duration);

        // Scale: grow then shrink
        float scaleT = t < 0.2f ? t / 0.2f : 1f - ((t - 0.2f) / 0.8f);
        transform.localScale = _startScale * (1f + (MaxScale - 1f) * scaleT);

        // Fade out
        Color c = _startColor;
        c.a = 1f - t;
        _material.color = c;

        if (t >= 1f)
            Destroy(gameObject);
    }
}