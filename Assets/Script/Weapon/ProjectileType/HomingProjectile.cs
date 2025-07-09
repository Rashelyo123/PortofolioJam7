using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Homing Projectile untuk Keris dan Buku Mantra
public class HomingProjectile : MonoBehaviour
{
    private GameObject target;
    private float damage;
    private float speed;
    private float lifetime = 5f;
    private float timeAlive;
    private bool isActive;

    private Transform cachedTransform;
    private Rigidbody2D rb;

    void Awake()
    {
        cachedTransform = transform;
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(GameObject targetEnemy, float dmg, float spd)
    {
        target = targetEnemy;
        damage = dmg;
        speed = spd;
        timeAlive = 0f;
        isActive = true;
    }

    void Update()
    {
        if (!isActive) return;

        timeAlive += Time.deltaTime;

        if (timeAlive >= lifetime || target == null)
        {
            ReturnToPool();
            return;
        }

        // Homing behavior
        Vector2 direction = (target.transform.position - cachedTransform.position).normalized;

        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            cachedTransform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        // Rotate to face target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cachedTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemyHealth = other.GetComponent<Enemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        isActive = false;
        if (rb != null) rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
}
