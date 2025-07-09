using UnityEngine;
using System.Collections.Generic;

// === PRIMARY WEAPONS ===

// STR - Cerurit (Melee)
public class CeluritWeapon : WeaponBase
{
    [Header("Cerurit Settings")]
    public LayerMask enemyLayerMask;
    public Transform slashPoint;

    private Collider2D[] enemyBuffer = new Collider2D[20];
    private ParticleSystem slashEffect;

    protected override void Initialize()
    {
        slashEffect = GetComponentInChildren<ParticleSystem>();

        if (slashPoint == null)
        {
            GameObject slashPointObj = new GameObject("SlashPoint");
            slashPointObj.transform.SetParent(transform);
            slashPointObj.transform.localPosition = Vector3.right * 0.5f;
            slashPoint = slashPointObj.transform;
        }
    }

    protected override void TryAttack()
    {
        Attack();
    }

    protected override void Attack()
    {
        nextFireTime = Time.time + (1f / weaponData.fireRate);

        // Slash effect
        if (slashEffect != null)
            slashEffect.Play();

        // Hit enemies dalam range
        int hitCount = Physics2D.OverlapCircleNonAlloc(
            slashPoint.position,
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
            float finalDamage = CalculateDamage();
            enemyHealth.TakeDamage(finalDamage);

            // Knockback
            if (weaponData.knockbackForce > 0)
            {
                Vector2 knockDirection = (enemy.transform.position - transform.position).normalized;
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(knockDirection * weaponData.knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    private float CalculateDamage()
    {
        float damage = weaponData.damage;

        // Crit chance
        if (Random.value < weaponData.critChance)
        {
            damage *= weaponData.critMultiplier;
        }

        return damage;
    }
}