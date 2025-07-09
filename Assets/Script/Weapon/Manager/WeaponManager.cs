using UnityEngine;
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Setup")]
    public Transform weaponParent;

    private WeaponBase primaryWeapon;
    private System.Collections.Generic.List<WeaponBase> secondaryWeapons;
    private WeaponFactory weaponFactory;

    void Start()
    {
        secondaryWeapons = new System.Collections.Generic.List<WeaponBase>();
        weaponFactory = new WeaponFactory();
    }

    public void SetPrimaryWeapon(WeaponDataSO weaponData)
    {
        // Remove existing primary weapon
        if (primaryWeapon != null)
        {
            Destroy(primaryWeapon.gameObject);
        }

        // Create new primary weapon
        primaryWeapon = weaponFactory.CreateWeapon(weaponData, weaponParent);
    }

    public void AddSecondaryWeapon(WeaponDataSO weaponData)
    {
        WeaponBase newWeapon = weaponFactory.CreateWeapon(weaponData, weaponParent);
        secondaryWeapons.Add(newWeapon);
    }

    public void UpgradeAllWeapons(UpgradeType upgradeType, float value)
    {
        switch (upgradeType)
        {
            case UpgradeType.Damage:
                primaryWeapon?.UpgradeDamage(value);
                foreach (var weapon in secondaryWeapons)
                    weapon.UpgradeDamage(value);
                break;
            case UpgradeType.FireRate:
                primaryWeapon?.UpgradeFireRate(value);
                foreach (var weapon in secondaryWeapons)
                    weapon.UpgradeFireRate(value);
                break;
            case UpgradeType.Range:
                primaryWeapon?.UpgradeRange(value);
                foreach (var weapon in secondaryWeapons)
                    weapon.UpgradeRange(value);
                break;
        }
    }
}
