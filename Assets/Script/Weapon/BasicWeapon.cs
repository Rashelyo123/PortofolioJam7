using UnityEngine;
using System.Collections.Generic;
public class BasicWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f; // shots per second
    public float damage = 1f;
    public float range = 8f;

    [Header("Auto-Target Settings")]
    public LayerMask enemyLayerMask = -1;

    private float nextFireTime = 0f;
    private PlayerController playerController;

    // Cached untuk performance
    private Transform cachedTransform;
    private Vector3 cachedPosition;

    // Object pooling untuk projectiles
    private ProjectilePool projectilePool;

    // Untuk menghindari allocation di FindNearestEnemy
    private Collider2D[] enemyBuffer = new Collider2D[50]; // Max 50 enemies in range

    void Start()
    {
        InitializeComponents();
        InitializeProjectilePool();
    }

    void InitializeComponents()
    {
        cachedTransform = transform;
        playerController = GetComponentInParent<PlayerController>();

        // Jika tidak ada firePoint, buat otomatis
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(cachedTransform);
            firePointObj.transform.localPosition = Vector3.zero;
            firePoint = firePointObj.transform;
        }
    }

    void InitializeProjectilePool()
    {
        if (projectilePrefab != null)
        {
            projectilePool = new ProjectilePool(projectilePrefab, 20); // Pool 20 projectiles
        }
    }

    void Update()
    {
        // Cache position untuk menghindari multiple transform access
        cachedPosition = cachedTransform.position;

        // Check jika bisa fire
        if (Time.time >= nextFireTime)
        {
            TryFire();
        }
    }

    void TryFire()
    {
        // Cari enemy terdekat dalam range
        GameObject nearestEnemy = FindNearestEnemyOptimized();

        if (nearestEnemy != null)
        {
            Fire(nearestEnemy.transform.position);
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void Fire(Vector3 targetPosition)
    {
        if (projectilePool == null) return;

        // Calculate direction ke target
        Vector2 direction = (targetPosition - firePoint.position).normalized;

        // Get projectile from pool
        GameObject projectile = projectilePool.GetProjectile();
        if (projectile == null) return;

        // Set position and rotation
        projectile.transform.position = firePoint.position;

        // Calculate rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Initialize projectile
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, damage);
        }

        projectile.SetActive(true);
    }

    GameObject FindNearestEnemyOptimized()
    {
        // Menggunakan NonAlloc untuk menghindari garbage collection
        int enemyCount = Physics2D.OverlapCircleNonAlloc(cachedPosition, range, enemyBuffer, enemyLayerMask);

        if (enemyCount == 0) return null;

        GameObject nearestEnemy = null;
        float nearestDistanceSqr = Mathf.Infinity; // Menggunakan squared distance untuk performance

        for (int i = 0; i < enemyCount; i++)
        {
            Collider2D enemyCollider = enemyBuffer[i];

            if (enemyCollider.CompareTag("Enemy"))
            {
                // Menggunakan sqrMagnitude untuk menghindari sqrt calculation
                float distanceSqr = (enemyCollider.transform.position - cachedPosition).sqrMagnitude;

                if (distanceSqr < nearestDistanceSqr)
                {
                    nearestDistanceSqr = distanceSqr;
                    nearestEnemy = enemyCollider.gameObject;
                }
            }
        }

        return nearestEnemy;
    }

    // Debug untuk melihat range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    // Public methods untuk upgrade system
    public void UpgradeDamage(float multiplier)
    {
        damage *= multiplier;
    }

    public void UpgradeFireRate(float multiplier)
    {
        fireRate *= multiplier;
    }

    public void UpgradeRange(float additionalRange)
    {
        range += additionalRange;
    }

    void OnDestroy()
    {
        // Cleanup pool
        projectilePool?.Cleanup();
    }
}

// Simple Object Pool untuk projectiles
public class ProjectilePool
{
    private GameObject prefab;
    private Queue<GameObject> pool;
    private Transform poolParent;

    public ProjectilePool(GameObject projectilePrefab, int initialSize)
    {
        prefab = projectilePrefab;
        pool = new Queue<GameObject>();

        // Create parent object untuk organization
        poolParent = new GameObject("ProjectilePool").transform;

        // Pre-populate pool
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewProjectile();
        }
    }

    GameObject CreateNewProjectile()
    {
        GameObject projectile = Object.Instantiate(prefab, poolParent);
        projectile.SetActive(false);
        pool.Enqueue(projectile);
        return projectile;
    }

    public GameObject GetProjectile()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }

        // If pool is empty, create new one
        return CreateNewProjectile();
    }

    public void ReturnProjectile(GameObject projectile)
    {
        if (projectile != null)
        {
            projectile.SetActive(false);
            pool.Enqueue(projectile);
        }
    }

    public void Cleanup()
    {
        if (poolParent != null)
        {
            Object.Destroy(poolParent.gameObject);
        }
    }
}