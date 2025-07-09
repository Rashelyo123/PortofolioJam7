using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WIS - Air Garem (AOE)
public class AirGaremWeapon : WeaponBase
{
    [Header("Air Garem Settings")]
    public LayerMask enemyLayerMask;
    public GameObject aoeEffectPrefab;

    private Collider2D[] enemyBuffer = new Collider2D[100];

    protected override void Initialize()
    {
        // Air Garem doesn't need special initialization
    }

    protected override void TryAttack()
    {
        // Air Garem attacks constantly based on fire rate
        Attack();
    }

    protected override void Attack()
    {
        nextFireTime = Time.time + (1f / weaponData.fireRate);

        // Create AOE effect
        if (aoeEffectPrefab != null)
        {
            GameObject effect = Instantiate(aoeEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Damage all enemies in range
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            weaponData.range,
            enemyBuffer,
            enemyLayerMask
        );

        for (int i = 0; i < hitCount; i++)
        {
            if (enemyBuffer[i].CompareTag("Enemy"))
            {
                DamageEnemy(enemyBuffer[i].gameObject);
            }
        }
    }

    private void DamageEnemy(GameObject enemy)
    {
        Enemy enemyHealth = enemy.GetComponent<Enemy>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(weaponData.damage);
        }
    }
}
