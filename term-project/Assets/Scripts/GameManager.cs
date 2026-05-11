using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        CannonController.OnFire += FireProjectile;
    }

    private void OnDestroy()
    {
        CannonController.OnFire -= FireProjectile;
    }

    private void FireProjectile(Vector3 position, Vector3 direction, float power)
    {
        Projectile.Create(position, direction, power);
    }
}
