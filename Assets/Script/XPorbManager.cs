using UnityEngine;
using System.Collections.Generic;

public class XPOrbManager : MonoBehaviour
{
    [Header("XP Orb Settings")]
    public GameObject xpOrbPrefab;
    public int initialPoolSize = 200;
    public float orbLifetime = 30f; // Orb hilang setelah 30 detik
    public float cleanupDistance = 25f; // Cleanup orb yang jauh dari player

    [Header("Spawn Settings")]
    public float spawnRadius = 0.5f; // Radius spawn orb sekitar enemy
    public AnimationCurve spawnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private ObjectPool<XPOrb> xpOrbPool;
    private List<XPOrb> activeOrbs = new List<XPOrb>();
    private Transform player;

    // Singleton instance
    public static XPOrbManager Instance { get; private set; }

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Initialize object pool
        if (xpOrbPrefab != null)
        {
            xpOrbPool = new ObjectPool<XPOrb>(xpOrbPrefab, initialPoolSize);
        }

        // Find player
        FindPlayer();

        // Start cleanup coroutine
        StartCoroutine(CleanupOrbs());
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

    // Method untuk spawn XP orb dari enemy
    public void SpawnXPOrb(Vector3 position, int xpValue)
    {
        if (xpOrbPool == null || xpOrbPrefab == null) return;

        // Get orb dari pool
        XPOrb orb = xpOrbPool.Get();

        // Set random position di sekitar enemy
        Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
        orb.transform.position = position + randomOffset;

        // Reset orb state
        orb.ResetOrb(xpValue, orbLifetime);
        orb.SetManager(this);

        // Add ke active orbs
        activeOrbs.Add(orb);

        // Optional: Add spawn effect
        StartCoroutine(SpawnEffect(orb));
    }

    // Method untuk spawn multiple orbs (untuk boss atau special enemies)
    public void SpawnMultipleXPOrbs(Vector3 position, int xpValue, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // Spread orbs dalam circle
            float angle = (360f / count) * i * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

            SpawnXPOrb(position + offset, xpValue);
        }
    }

    // Method dipanggil dari XPOrb saat di-collect atau expired
    public void ReturnOrbToPool(XPOrb orb)
    {
        if (activeOrbs.Contains(orb))
        {
            activeOrbs.Remove(orb);
            xpOrbPool.Return(orb);
        }
    }

    // Cleanup orbs yang terlalu jauh atau terlalu lama
    System.Collections.IEnumerator CleanupOrbs()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f); // Check every 2 seconds

            FindPlayer(); // Update player reference

            if (player != null)
            {
                for (int i = activeOrbs.Count - 1; i >= 0; i--)
                {
                    if (activeOrbs[i] != null)
                    {
                        float distance = Vector3.Distance(activeOrbs[i].transform.position, player.position);

                        // Return orb jika terlalu jauh
                        if (distance > cleanupDistance)
                        {
                            ReturnOrbToPool(activeOrbs[i]);
                        }
                    }
                    else
                    {
                        // Remove null references
                        activeOrbs.RemoveAt(i);
                    }
                }
            }
        }
    }

    // Spawn effect coroutine
    System.Collections.IEnumerator SpawnEffect(XPOrb orb)
    {
        if (orb == null) yield break;

        Vector3 originalScale = orb.transform.localScale;
        orb.transform.localScale = Vector3.zero;

        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float curveValue = spawnCurve.Evaluate(progress);

            if (orb != null)
            {
                orb.transform.localScale = originalScale * curveValue;
            }

            yield return null;
        }

        if (orb != null)
        {
            orb.transform.localScale = originalScale;
        }
    }

    // Public methods untuk stats dan management
    public int GetActiveOrbCount()
    {
        return activeOrbs.Count;
    }

    public void ClearAllOrbs()
    {
        for (int i = activeOrbs.Count - 1; i >= 0; i--)
        {
            if (activeOrbs[i] != null)
            {
                ReturnOrbToPool(activeOrbs[i]);
            }
        }
        activeOrbs.Clear();
    }

    // Method untuk attract all orbs ke player (power-up effect)
    public void AttractAllOrbs()
    {
        foreach (XPOrb orb in activeOrbs)
        {
            if (orb != null)
            {
                orb.SetAttracted(true);
            }
        }
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, cleanupDistance);
        }

        // Draw spawn radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}