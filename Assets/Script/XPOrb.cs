using UnityEngine;
using System.Collections;

public class XPOrb : MonoBehaviour
{
    [Header("XP Settings")]
    public int xpValue = 1;
    public float collectRange = 1.5f;
    public float attractRange = 8f;
    public float moveSpeed = 5f;
    public float attractSpeed = 12f;

    [Header("Visual Effects")]
    public AnimationCurve bobCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;
    public GameObject collectEffect;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D orbCollider;
    private XPOrbManager manager;

    private bool isAttracted = false;
    private bool isCollected = false;
    private float lifetime;
    private float currentLifetime;
    private Vector3 startPosition;
    private float bobTimer = 0f;

    void Start()
    {
        InitializeComponents();
    }

    void InitializeComponents()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        orbCollider = GetComponent<Collider2D>();

        // Setup rigidbody
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.drag = 5f; // Add drag untuk smooth movement
        }

        // Find player
        FindPlayer();
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    // Method untuk reset orb state saat diambil dari pool
    public void ResetOrb(int value, float orbLifetime)
    {
        xpValue = value;
        lifetime = orbLifetime;
        currentLifetime = lifetime;
        isAttracted = false;
        isCollected = false;
        bobTimer = Random.Range(0f, Mathf.PI * 2f); // Random start phase

        // Reset visual
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
            spriteRenderer.enabled = true;
        }

        // Reset collider
        if (orbCollider != null)
        {
            orbCollider.enabled = true;
        }

        // Reset rigidbody
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Store start position for bobbing
        startPosition = transform.position;

        // Start lifetime countdown
        StartCoroutine(LifetimeCountdown());

        // Enable gameObject
        gameObject.SetActive(true);
    }

    // Method untuk set manager reference
    public void SetManager(XPOrbManager orbManager)
    {
        manager = orbManager;
    }

    // Method untuk force attract orb
    public void SetAttracted(bool attracted)
    {
        isAttracted = attracted;
    }

    void Update()
    {
        if (isCollected) return;

        FindPlayer();

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // Check if should be attracted
            if (distanceToPlayer <= attractRange || isAttracted)
            {
                MoveTowardsPlayer();
            }
            else
            {
                // Bobbing effect saat idle
                BobEffect();
            }

            // Check collection
            if (distanceToPlayer <= collectRange)
            {
                CollectOrb();
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (player != null && rb != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float speed = isAttracted ? attractSpeed : moveSpeed;

            rb.velocity = direction * speed;
        }
    }

    void BobEffect()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // Bobbing animation
        bobTimer += Time.deltaTime * bobSpeed;
        float bobOffset = bobCurve.Evaluate(Mathf.Sin(bobTimer)) * bobHeight;

        Vector3 targetPosition = startPosition + Vector3.up * bobOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3f);
    }

    void CollectOrb()
    {
        if (isCollected) return;

        isCollected = true;

        // Add XP ke player
        ExperienceManager playerLevel = player.GetComponent<ExperienceManager>();
        if (playerLevel != null)
        {
            playerLevel.GainXP(xpValue);
        }

        // Spawn collect effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Show floating text
        FloatingText.Create($"+{xpValue} XP", transform.position, Color.cyan);

        // Play collect sound (if you have audio manager)
        // AudioManager.Instance?.PlaySound("XPCollect");

        // Return to pool
        ReturnToPool();
    }

    void ReturnToPool()
    {
        // Stop all coroutines
        StopAllCoroutines();

        // Disable visual components
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (orbCollider != null)
        {
            orbCollider.enabled = false;
        }

        // Return to pool via manager
        if (manager != null)
        {
            manager.ReturnOrbToPool(this);
        }
        else
        {
            // Fallback
            gameObject.SetActive(false);
        }
    }

    IEnumerator LifetimeCountdown()
    {
        while (currentLifetime > 0 && !isCollected)
        {
            currentLifetime -= Time.deltaTime;

            // Fade out effect saat mendekati expired
            if (currentLifetime <= 3f && spriteRenderer != null)
            {
                float alpha = Mathf.Lerp(0f, 1f, currentLifetime / 3f);
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            yield return null;
        }

        // Expired - return to pool
        if (!isCollected)
        {
            ReturnToPool();
        }
    }

    // Optional: Magnetic effect saat player dekat
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isAttracted = true;
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractRange);
    }
}