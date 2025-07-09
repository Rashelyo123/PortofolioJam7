using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Factory pattern untuk create weapons
public class WeaponFactory
{
    public WeaponBase CreateWeapon(WeaponDataSO weaponData, Transform parent)
    {
        if (weaponData == null || weaponData.weaponPrefab == null) return null;

        GameObject weaponObj = Object.Instantiate(weaponData.weaponPrefab, parent);
        WeaponBase weapon = weaponObj.GetComponent<WeaponBase>();

        if (weapon != null)
        {
            weapon.weaponData = weaponData;
        }

        return weapon;
    }
}
