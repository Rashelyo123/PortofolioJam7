using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INT - Paku (Ranged)
public class PakuWeapon : WeaponBase
{
    [Header("Paku Settings")]
    public Transform firePoint;
    public LayerMask enemyLayerMask;

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
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy != null)
        {
            Attack();
        }
    }

    protected override void Attack()
    {
        GameObject nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null) return;

        nextFireTime = Time.time + (1f / weaponData.fireRate);

        Vector2 direction = (nearestEnemy.transform.position - firePoint.position).normalized;
        FireProjectile(direction);
    }

    private void FireProjectile(Vector2 direction)
    {
        if (projectilePool == null) return;

        GameObject projectile = projectilePool.GetProjectile();
        if (projectile == null) return;

        projectile.transform.position = firePoint.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // ✅ Aktifkan dulu baru inisialisasi
        projectile.SetActive(true);

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, weaponData.damage);
        }
    }


    private GameObject FindNearestEnemy()
    {
        int enemyCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            weaponData.range,
            enemyBuffer,
            enemyLayerMask
        );

        if (enemyCount == 0) return null;

        GameObject nearestEnemy = null;
        float nearestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < enemyCount; i++)
        {
            if (enemyBuffer[i].CompareTag("Enemy"))
            {
                float distanceSqr = (enemyBuffer[i].transform.position - transform.position).sqrMagnitude;
                if (distanceSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distanceSqr;
                    nearestEnemy = enemyBuffer[i].gameObject;
                }
            }
        }

        return nearestEnemy;
    }
}

