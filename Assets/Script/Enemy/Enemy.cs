using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float maxHealth = 3f;
    public float moveSpeed = 2f;
    public float damage = 1f;
    [SerializeField] private EnemyData enemyData;



    [Header("Cleanup")]
    public float maxDistanceFromPlayer = 20f;

    private float currentHealth;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemySpawner spawner; // Reference ke spawner

    void Start()
    {
        InitializeEnemy();
    }

    void InitializeEnemy()
    {
        // Pastikan enemyData ada
        if (enemyData != null)
        {
            currentHealth = enemyData.maxHealth;
            maxHealth = enemyData.maxHealth;
            moveSpeed = enemyData.moveSpeed;
            damage = enemyData.damage;
        }
        else
        {
            currentHealth = maxHealth;
        }

        // Initialize components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
            rb.gravityScale = 0f;

        // Find player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    // Method untuk reset enemy state saat diambil dari pool
    public void ResetEnemy()
    {
        // Reset health
        if (enemyData != null)
        {
            currentHealth = enemyData.maxHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }

        // Reset visual
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        // Reset rigidbody
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Enable gameObject jika disabled
        gameObject.SetActive(true);

        // Find player jika belum ada
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    // Method untuk set spawner reference
    public void SetSpawner(EnemySpawner enemySpawner)
    {
        spawner = enemySpawner;
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

            // Gunakan transform movement untuk performa yang lebih baik
            transform.position += (Vector3)(direction * moveSpeed * Time.fixedDeltaTime);

            // Flip sprite berdasarkan direction
            if (spriteRenderer != null)
            {
                if (direction.x > 0)
                    spriteRenderer.flipX = false;
                else if (direction.x < 0)
                    spriteRenderer.flipX = true;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Vector3 textPosition = transform.position + Vector3.up * 1.5f;
        FloatingText.Create(damageAmount.ToString(), textPosition, Color.red);

        // Visual feedback
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Notify UI Manager
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.OnEnemyKilled();
        }

        // Spawn XP Orb menggunakan XPOrbManager
        if (XPOrbManager.Instance != null && enemyData != null)
        {
            XPOrbManager.Instance.SpawnXPOrb(transform.position, Mathf.RoundToInt(enemyData.xpDropAmount));
        }

        // Return to pool instead of destroying
        if (spawner != null)
        {
            spawner.ReturnEnemyToPool(this);
        }
        else
        {
            // Fallback jika spawner tidak ada
            gameObject.SetActive(false);
        }
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

    void OnTriggerEnter2D(Collider2D other)
    {
        // Damage player saat collision
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by enemy!");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }

    // Optional: Method untuk manual cleanup jika diperlukan
    void OnDisable()
    {
        // Stop all coroutines saat object disabled
        StopAllCoroutines();
    }
}