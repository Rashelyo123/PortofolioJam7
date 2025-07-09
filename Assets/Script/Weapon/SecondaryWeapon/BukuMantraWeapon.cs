using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Buku Mantra (Magic Missile)
public class BukuMantraWeapon : WeaponBase
{
    [Header("Buku Mantra Settings")]
    public Transform firePoint;
    public LayerMask enemyLayerMask;
    public int missileCount = 3;

    private Collider2D[] enemyBuffer = new Collider2D[50];
    private ProjectilePool projectilePool;

    protected override void Initialize()
    {
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = Vector3.zero;
            firePoint = firePointObj.transform;
        }

        if (weaponData.projectilePrefab != null)
        {
            projectilePool = new ProjectilePool(weaponData.projectilePrefab, 15);
        }
    }

    protected override void TryAttack()
    {
        if (HasEnemiesInRange())
        {
            Attack();
        }
    }

    protected override void Attack()
    {
        nextFireTime = Time.time + (1f / weaponData.fireRate);

        // Fire multiple magic missiles
        for (int i = 0; i < missileCount; i++)
        {
            GameObject target = FindRandomEnemy();
            if (target != null)
            {
                FireMagicMissile(target);
            }
        }
    }

    private void FireMagicMissile(GameObject target)
    {
        if (projectilePool == null) return;

        GameObject projectile = projectilePool.GetProjectile();
        if (projectile == null) return;

        projectile.transform.position = firePoint.position;

        HomingProjectile homingScript = projectile.GetComponent<HomingProjectile>();
        if (homingScript != null)
        {
            homingScript.Initialize(target, weaponData.damage, weaponData.speed);
        }

        projectile.SetActive(true);
    }

    private bool HasEnemiesInRange()
    {
        int enemyCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            weaponData.range,
            enemyBuffer,
            enemyLayerMask
        );

        return enemyCount > 0;
    }

    private GameObject FindRandomEnemy()
    {
        int enemyCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            weaponData.range,
            enemyBuffer,
            enemyLayerMask
        );

        if (enemyCount == 0) return null;

        List<GameObject> validEnemies = new List<GameObject>();
        for (int i = 0; i < enemyCount; i++)
        {
            if (enemyBuffer[i].CompareTag("Enemy"))
            {
                validEnemies.Add(enemyBuffer[i].gameObject);
            }
        }

        if (validEnemies.Count > 0)
        {
            return validEnemies[Random.Range(0, validEnemies.Count)];
        }

        return null;
    }
}
