using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;
    public GameObject[] bossPrefabs;
    public Transform player;
    public float spawnRate = 2f;
    public int maxEnemiesOnScreen = 50;
    public float spawnDistance = 12f;

    [Header("Wave Settings")]
    public bool useWaveProgression = true;
    public float waveInterval = 30f;
    public float difficultyMultiplier = 1.1f;

    [Header("Object Pooling")]
    public int initialPoolSize = 100;

    [Header("Spawn Area")]
    public Camera gameCamera;

    // Object pools untuk setiap enemy type
    private Dictionary<GameObject, ObjectPool<Enemy>> enemyPools = new Dictionary<GameObject, ObjectPool<Enemy>>();
    private List<Enemy> activeEnemies = new List<Enemy>();

    private float nextSpawnTime = 0f;
    private int currentEnemyCount = 0;
    private int currentWave = 1;
    private float gameStartTime;

    void Start()
    {
        gameStartTime = Time.time;

        // Initialize player reference
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Initialize camera reference
        if (gameCamera == null)
            gameCamera = Camera.main;

        // Initialize object pools
        InitializeObjectPools();

        // Start spawning
        StartCoroutine(SpawnEnemies());

        if (useWaveProgression)
            StartCoroutine(WaveProgression());
    }

    void InitializeObjectPools()
    {
        // Buat pool untuk setiap enemy type
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            if (enemyPrefab != null)
            {
                ObjectPool<Enemy> pool = new ObjectPool<Enemy>(enemyPrefab, initialPoolSize);
                enemyPools[enemyPrefab] = pool;
            }
        }

        // Buat pool untuk boss (opsional)
        foreach (GameObject bossPrefab in bossPrefabs)
        {
            if (bossPrefab != null)
            {
                ObjectPool<Enemy> pool = new ObjectPool<Enemy>(bossPrefab, 5); // Boss pool lebih kecil
                enemyPools[bossPrefab] = pool;
            }
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Check jika bisa spawn
            if (currentEnemyCount < maxEnemiesOnScreen && enemyPrefabs.Length > 0)
            {
                SpawnEnemy();
            }

            // Wait berdasarkan spawn rate
            float waitTime = 1f / spawnRate;
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnEnemy()
    {
        if (player == null) return;

        // Pilih random enemy type
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Get enemy dari pool
        if (enemyPools.ContainsKey(enemyPrefab))
        {
            Enemy enemy = enemyPools[enemyPrefab].Get();

            // Set spawn position
            Vector3 spawnPosition = GetRandomSpawnPosition();
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = Quaternion.identity;

            // Reset enemy state (jika perlu)
            enemy.ResetEnemy();

            // Add ke active enemies list
            activeEnemies.Add(enemy);
            currentEnemyCount++;

            // Set spawner reference untuk return ke pool
            enemy.SetSpawner(this);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (player == null) return Vector3.zero;

        // Get random angle
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        // Calculate position di luar screen tapi tidak terlalu jauh
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        Vector3 spawnPosition = player.position + direction * spawnDistance;

        return spawnPosition;
    }

    // Method dipanggil dari Enemy script saat mati
    public void ReturnEnemyToPool(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            currentEnemyCount--;

            // Cari pool yang tepat untuk enemy ini
            foreach (var kvp in enemyPools)
            {
                if (enemy.name.Contains(kvp.Key.name.Replace("(Clone)", "")))
                {
                    kvp.Value.Return(enemy);
                    break;
                }
            }
        }
    }

    // Method untuk spawn boss
    public void SpawnBoss()
    {
        if (bossPrefabs.Length == 0 || player == null) return;

        GameObject bossPrefab = bossPrefabs[Random.Range(0, bossPrefabs.Length)];

        if (enemyPools.ContainsKey(bossPrefab))
        {
            Enemy boss = enemyPools[bossPrefab].Get();
            boss.transform.position = GetRandomSpawnPosition();
            boss.transform.rotation = Quaternion.identity;
            boss.ResetEnemy();
            boss.SetSpawner(this);

            activeEnemies.Add(boss);
            currentEnemyCount++;
        }
    }

    IEnumerator WaveProgression()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveInterval);

            // Increase wave
            currentWave++;

            // Increase difficulty
            spawnRate *= difficultyMultiplier;
            maxEnemiesOnScreen = Mathf.RoundToInt(maxEnemiesOnScreen * difficultyMultiplier);

            // Spawn boss setiap 5 wave
            if (currentWave % 5 == 0)
            {
                SpawnBoss();
            }

            // Optional: Show wave notification
            Debug.Log($"Wave {currentWave}! Spawn Rate: {spawnRate:F1}, Max Enemies: {maxEnemiesOnScreen}");
        }
    }

    // Cleanup method untuk performance
    void Update()
    {
        // Cleanup enemies yang terlalu jauh dari player
        CleanupDistantEnemies();
    }

    void CleanupDistantEnemies()
    {
        float cleanupDistance = spawnDistance * 2f; // Double spawn distance

        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null && player != null)
            {
                float distance = Vector3.Distance(activeEnemies[i].transform.position, player.position);
                if (distance > cleanupDistance)
                {
                    ReturnEnemyToPool(activeEnemies[i]);
                }
            }
        }
    }

    // Public methods untuk UI atau game management
    public int GetCurrentWave()
    {
        return currentWave;
    }

    public float GetGameTime()
    {
        return Time.time - gameStartTime;
    }

    public int GetEnemyCount()
    {
        return currentEnemyCount;
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }

    // Method untuk modify spawn settings saat runtime
    public void SetSpawnRate(float newRate)
    {
        spawnRate = newRate;
    }

    public void SetMaxEnemies(int newMax)
    {
        maxEnemiesOnScreen = newMax;
    }

    // Method untuk clear semua enemies (untuk restart game)
    public void ClearAllEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] != null)
            {
                ReturnEnemyToPool(activeEnemies[i]);
            }
        }
        activeEnemies.Clear();
        currentEnemyCount = 0;
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, spawnDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, spawnDistance * 2f); // Cleanup distance
        }
    }
}