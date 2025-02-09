using UnityEngine;

public class FactoryProjectile : IService
{
    public Projectile ProjectilePrefab;

    public FactoryProjectile(Projectile projectilePrefab)
    {
        ProjectilePrefab = projectilePrefab;
    }

    public ProjectileType BuildProjectile<ProjectileType>(Vector3 at) where ProjectileType : Projectile =>
        (ProjectileType)Object.Instantiate(ProjectilePrefab, at, Quaternion.identity);
}
