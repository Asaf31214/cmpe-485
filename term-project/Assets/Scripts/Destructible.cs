using UnityEngine;

public class Destructible : MonoBehaviour
{
    public static event System.Action OnTargetDestroyed;

    public float Health = 50f;
    public bool IsTarget;
    public float MinCollisionDamageVelocity = 8f;

    private bool destroyed;

    public void TakeDamage(float amount)
    {
        if (destroyed) return;

        Health -= amount;
        if (Health <= 0)
        {
            Destroy();
        }
    }

    private void Destroy()
    {
        destroyed = true;

        if (IsTarget)
        {
            OnTargetDestroyed?.Invoke();
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsTarget || destroyed) return;

        float impactVelocity = collision.relativeVelocity.magnitude;
        if (impactVelocity >= MinCollisionDamageVelocity)
        {
            TakeDamage(impactVelocity * 20f);
        }
    }
}