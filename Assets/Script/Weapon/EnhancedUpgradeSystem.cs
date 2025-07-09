using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedUpgradeSystem : UpgradeSystem
{
    private WeaponSelectionUI weaponSelectionUI;
    private WeaponManager weaponManager;

    public EnhancedUpgradeSystem(PlayerController player, PlayerHealth health, BasicWeapon weaponRef,
                                WeaponSelectionUI weaponUI, WeaponManager weaponMgr)
        : base(player, health, weaponRef)
    {
        weaponSelectionUI = weaponUI;
        weaponManager = weaponMgr;

        // Add secondary weapon upgrade to actions
        if (upgradeActions.ContainsKey(UpgradeType.SecondaryWeapon))
        {
            upgradeActions[UpgradeType.SecondaryWeapon] = AddRandomSecondaryWeapon;
        }
        else
        {
            upgradeActions.Add(UpgradeType.SecondaryWeapon, AddRandomSecondaryWeapon);
        }
    }

    void AddRandomSecondaryWeapon()
    {
        if (weaponSelectionUI != null && weaponManager != null)
        {
            WeaponDataSO randomWeapon = weaponSelectionUI.GetRandomSecondaryWeapon();
            if (randomWeapon != null)
            {
                weaponManager.AddSecondaryWeapon(randomWeapon);
                Debug.Log($"Added secondary weapon: {randomWeapon.weaponName}");
            }
        }
    }

    public override List<UpgradeData> GetAvailableUpgrades()
    {
        List<UpgradeData> availableUpgrades = new List<UpgradeData>(upgradePool);

        // Add secondary weapon upgrade
        availableUpgrades.Add(new UpgradeData("Secondary Weapon", "Get a random secondary weapon", UpgradeType.SecondaryWeapon));

        // Fisher-Yates shuffle
        for (int i = availableUpgrades.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            UpgradeData temp = availableUpgrades[i];
            availableUpgrades[i] = availableUpgrades[randomIndex];
            availableUpgrades[randomIndex] = temp;
        }

        // Return only 3 random upgrades
        return availableUpgrades.GetRange(0, Mathf.Min(3, availableUpgrades.Count));
    }

    public override void ApplyUpgrade(UpgradeData upgrade)
    {
        if (upgradeActions.TryGetValue(upgrade.type, out System.Action action))
        {
            action?.Invoke();
            Debug.Log($"Applied upgrade: {upgrade.name}");
        }
        else
        {
            Debug.LogWarning($"Unknown upgrade type: {upgrade.type}");
        }
    }
}
