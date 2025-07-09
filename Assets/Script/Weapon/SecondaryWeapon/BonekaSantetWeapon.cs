

using UnityEngine;

// Boneka Santet (Curse/DOT)
public class BonekaSantetWeapon : WeaponBase
{
    [Header("Boneka Santet Settings")]
    public LayerMask enemyLayerMask;
    public float curseDuration = 5f;
    public float dotDamage = 2f;

    private Collider2D[] enemyBuffer = new Collider2D[30];

    protected override void Initialize()
    {
        // Boneka Santet setup
    }

    protected override void TryAttack()
    {
        Attack();
    }

    protected override void Attack()
    {
        nextFireTime = Time.time + (1f / weaponData.fireRate);

        // Find enemies to curse
        int enemyCount = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            weaponData.range,
            enemyBuffer,
            enemyLayerMask
        );

        for (int i = 0; i < enemyCount; i++)
        {
            if (enemyBuffer[i].CompareTag("Enemy"))
            {
                ApplyCurse(enemyBuffer[i].gameObject);
            }
        }
    }

    private void ApplyCurse(GameObject enemy)
    {
        CurseEffect curseEffect = enemy.GetComponent<CurseEffect>();
        if (curseEffect == null)
        {
            curseEffect = enemy.AddComponent<CurseEffect>();
        }

        curseEffect.ApplyCurse(curseDuration, dotDamage);
    }
}