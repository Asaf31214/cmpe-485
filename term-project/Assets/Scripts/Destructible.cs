using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float Health = 50f;
    public bool IsTarget = false;
    public float MinCollisionDamageVelocity = 8f;

    private float maxHealth;

    private void Start()
    {
        maxHealth = Health;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Destroy(gameObject);
            if (IsTarget)
            {
                GameManager.Instance.TargetDestroyed();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsTarget) return;

        float impactVelocity = collision.relativeVelocity.magnitude;
        if (impactVelocity >= MinCollisionDamageVelocity)
        {
            TakeDamage(impactVelocity * 3f);
        }
    }
}
