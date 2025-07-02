using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float maxHealth = 3f;
    public float moveSpeed = 2f;
    public float damage = 1f;

    [Space]
    [SerializeField] private GameObject xpOrbPrefab;

    [Header("Cleanup")]
    public float maxDistanceFromPlayer = 20f;

    private float currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Start cleanup check
        StartCoroutine(CheckDistanceFromPlayer());
    }

    void FixedUpdate()
    {
        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            // Flip sprite berdasarkan direction
            if (direction.x > 0)
                spriteRenderer.flipX = false;
            else if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // Visual feedback
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Spawn XP Orb

        if (xpOrbPrefab != null)
        {
            Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            Color original = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = original;
        }
    }

    IEnumerator CheckDistanceFromPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); // Check every 2 seconds

            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);

                // Destroy enemy jika terlalu jauh (optimization)
                if (distance > maxDistanceFromPlayer)
                {
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Damage player saat collision
        if (other.CompareTag("Player"))
        {
            // Player damage system akan kita buat nanti
            Debug.Log("Player hit by enemy!");

            // Optional: destroy enemy setelah hit player
            // Destroy(gameObject);
        }
    }
}