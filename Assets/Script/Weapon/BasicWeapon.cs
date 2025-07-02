using UnityEngine;


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

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();

        // Jika tidak ada firePoint, buat otomatis
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.SetParent(transform);
            firePointObj.transform.localPosition = Vector3.zero;
            firePoint = firePointObj.transform;
        }
    }

    void Update()
    {
        // Check jika bisa fire
        if (Time.time >= nextFireTime)
        {
            TryFire();
        }
    }

    void TryFire()
    {
        // Cari enemy terdekat dalam range
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            Fire(nearestEnemy.transform.position);
            nextFireTime = Time.time + (1f / fireRate);
        }
    }

    void Fire(Vector3 targetPosition)
    {
        if (projectilePrefab == null) return;

        // Calculate direction ke target
        Vector2 direction = (targetPosition - firePoint.position).normalized;

        // Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Initialize projectile
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(direction, damage);
        }

        // Rotate projectile sprite to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    GameObject FindNearestEnemy()
    {
        // Find all enemies dalam range
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, range, enemyLayerMask);

        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
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

    // Public methods untuk upgrade system nanti
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
}