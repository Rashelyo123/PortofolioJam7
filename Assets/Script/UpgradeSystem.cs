using System.Collections.Generic;
using UnityEngine;

// Upgrade System yang lebih modular dan extensible
// Base Upgrade System - Updated untuk kompatibilitas
public class UpgradeSystem
{
    protected PlayerController playerController;
    protected PlayerHealth playerHealth;
    protected BasicWeapon weapon;

    // Dictionary untuk upgrade actions - lebih performant dari switch case
    protected Dictionary<UpgradeType, System.Action> upgradeActions;

    // Pool untuk upgrade data agar tidak membuat object baru terus
    protected List<UpgradeData> upgradePool;

    public UpgradeSystem(PlayerController player, PlayerHealth health, BasicWeapon weaponRef)
    {
        playerController = player;
        playerHealth = health;
        weapon = weaponRef;

        InitializeUpgradeActions();
        InitializeUpgradePool();
    }

    protected virtual void InitializeUpgradeActions()
    {
        upgradeActions = new Dictionary<UpgradeType, System.Action>
        {
            { UpgradeType.Damage, () => weapon?.UpgradeDamage(1.2f) },
            { UpgradeType.FireRate, () => weapon?.UpgradeFireRate(1.3f) },
            { UpgradeType.Range, () => weapon?.UpgradeRange(2f) },
            { UpgradeType.Speed, () => { if (playerController != null) playerController.moveSpeed *= 1.15f; } },
            { UpgradeType.Health, () => playerHealth?.IncreaseMaxHealth(20f) }
        };
    }

    protected virtual void InitializeUpgradePool()
    {
        upgradePool = new List<UpgradeData>
        {
            new UpgradeData("Damage Up", "Increase weapon damage by 20%", UpgradeType.Damage),
            new UpgradeData("Fire Rate Up", "Increase fire rate by 30%", UpgradeType.FireRate),
            new UpgradeData("Range Up", "Increase weapon range by 2", UpgradeType.Range),
            new UpgradeData("Speed Up", "Increase movement speed by 15%", UpgradeType.Speed),
            new UpgradeData("Health Up", "Increase max health by 20", UpgradeType.Health)
        };
    }

    public virtual List<UpgradeData> GetAvailableUpgrades()
    {
        List<UpgradeData> availableUpgrades = new List<UpgradeData>(upgradePool);

        // Fisher-Yates shuffle untuk better randomization
        for (int i = availableUpgrades.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            UpgradeData temp = availableUpgrades[i];
            availableUpgrades[i] = availableUpgrades[randomIndex];
            availableUpgrades[randomIndex] = temp;
        }

        return availableUpgrades;
    }

    public virtual void ApplyUpgrade(UpgradeData upgrade)
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

// Upgrade Data and Enum - Updated untuk secondary weapon
[System.Serializable]
public class UpgradeData
{
    public string name;
    public string description;
    public UpgradeType type;

    public UpgradeData(string n, string desc, UpgradeType t)
    {
        name = n;
        description = desc;
        type = t;
    }
}

public enum UpgradeType
{
    Damage,
    FireRate,
    Range,
    Speed,
    Health,
    SecondaryWeapon
}