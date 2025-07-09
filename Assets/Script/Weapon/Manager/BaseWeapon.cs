using UnityEngine;

// Base class untuk semua weapon
public abstract class WeaponBase : MonoBehaviour
{
    [Header("Base Weapon Settings")]
    public WeaponDataSO weaponData;

    protected float nextFireTime = 0f;
    protected Transform cachedTransform;
    protected PlayerController playerController;

    protected virtual void Start()
    {
        cachedTransform = transform;
        playerController = GetComponentInParent<PlayerController>();
        Initialize();
    }

    protected virtual void Update()
    {
        if (Time.time >= nextFireTime)
        {
            TryAttack();
        }
    }

    protected abstract void Initialize();
    protected abstract void TryAttack();
    protected abstract void Attack();

    // Upgrade methods
    public virtual void UpgradeDamage(float multiplier)
    {
        weaponData.damage *= multiplier;
    }

    public virtual void UpgradeFireRate(float multiplier)
    {
        weaponData.fireRate *= multiplier;
    }

    public virtual void UpgradeRange(float additionalRange)
    {
        weaponData.range += additionalRange;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (weaponData != null)
        {
            Gizmos.color = weaponData.weaponType switch
            {
                WeaponType.Melee => Color.red,
                WeaponType.Ranged => Color.blue,
                WeaponType.Homing => Color.green,
                WeaponType.AOE => Color.yellow,
                _ => Color.white
            };
            Gizmos.DrawWireSphere(transform.position, weaponData.range);
        }
    }
}
