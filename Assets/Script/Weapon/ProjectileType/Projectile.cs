using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 10f;
    public float lifetime = 3f;

    private Vector2 direction;
    private float damage;
    private float timeAlive;
    private bool isActive;

    // Cached components
    private Transform cachedTransform;
    private Rigidbody2D rb;
    private ProjectilePool parentPool;

    void Awake()
    {
        cachedTransform = transform;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 dir, float dmg)
    {
        direction = dir.normalized;
        damage = dmg;
        timeAlive = 0f;
        isActive = true;

        // Set velocity jika menggunakan rigidbody
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    void Update()
    {
        if (!isActive) return;

        timeAlive += Time.deltaTime;

        // Check lifetime
        if (timeAlive >= lifetime)
        {
            ReturnToPool();
            return;
        }

        // Move projectile jika tidak menggunakan rigidbody
        if (rb == null)
        {
            cachedTransform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Enemy"))
        {
            // Apply damage
            Enemy enemyHealth = other.GetComponent<Enemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Return to pool instead of destroying
            ReturnToPool();
        }
        else if (other.CompareTag("Wall") || other.CompareTag("Obstacle"))
        {
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        isActive = false;

        // Reset velocity
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Find and return to pool
        if (parentPool != null)
        {
            parentPool.ReturnProjectile(gameObject);
        }
        else
        {
            // Fallback: deactivate
            gameObject.SetActive(false);
        }
    }

    public void SetPool(ProjectilePool pool)
    {
        parentPool = pool;
    }

    void OnDisable()
    {
        isActive = false;
    }
}