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
        rb.gravityScale = 0f;

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

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
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.OnEnemyKilled();
        }

        {
            if (xpOrbPrefab != null)
            {
                GameObject orb = Instantiate(xpOrbPrefab, transform.position, Quaternion.identity);
                XPOrb orbScript = orb.GetComponent<XPOrb>();
                orbScript.xpValue = enemyData.xpDropAmount;
            }
            // ... kode existing ...
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
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage(10f);

        }
    }
}