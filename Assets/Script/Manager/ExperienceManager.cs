using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience Settings")]
    public float baseXPRequired = 10f;
    public float xpGrowthRate = 1.5f;
    public int maxLevel = 100;

    [Header("UI References")]
    public UnityEngine.UI.Slider xpBar;
    public TextMeshProUGUI levelText;
    public GameObject levelUpPanel;
    public Transform upgradeButtonParent;
    public GameObject upgradeButtonPrefab;

    // Current stats
    private int currentLevel = 1;
    private float currentXP = 0f;
    private float xpRequiredForNextLevel;

    // Events
    public System.Action<int> OnLevelUp;
    public System.Action<float> OnXPGained;

    void Start()
    {
        CalculateXPRequired();
        UpdateUI();

        // Hide level up panel initially
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }

    public void GainXP(float amount)
    {
        currentXP += amount;
        OnXPGained?.Invoke(amount);

        // Check for level up
        while (currentXP >= xpRequiredForNextLevel && currentLevel < maxLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        currentXP -= xpRequiredForNextLevel;
        currentLevel++;

        CalculateXPRequired();
        OnLevelUp?.Invoke(currentLevel);

        // Show upgrade selection
        ShowUpgradeSelection();

        Debug.Log($"Level Up! Now Level {currentLevel}");
    }

    void CalculateXPRequired()
    {
        xpRequiredForNextLevel = baseXPRequired * Mathf.Pow(xpGrowthRate, currentLevel - 1);
    }

    void UpdateUI()
    {
        if (xpBar != null)
        {
            float xpProgress = currentXP / xpRequiredForNextLevel;
            xpBar.value = xpProgress;
        }

        if (levelText != null)
        {
            levelText.text = $"Level {currentLevel}";
        }
    }

    void ShowUpgradeSelection()
    {
        if (levelUpPanel == null) return;

        // Pause game
        Time.timeScale = 0f;

        // Show panel
        levelUpPanel.SetActive(true);

        // Generate upgrade options
        GenerateUpgradeOptions();
    }

    void GenerateUpgradeOptions()
    {
        if (upgradeButtonParent == null || upgradeButtonPrefab == null) return;

        // Clear existing buttons
        foreach (Transform child in upgradeButtonParent)
        {
            Destroy(child.gameObject);
        }

        // Get available upgrades
        List<UpgradeData> availableUpgrades = GetAvailableUpgrades();

        // Create buttons for upgrades (max 3 options)
        int optionsToShow = Mathf.Min(3, availableUpgrades.Count);

        for (int i = 0; i < optionsToShow; i++)
        {
            CreateUpgradeButton(availableUpgrades[i]);
        }
    }

    List<UpgradeData> GetAvailableUpgrades()
    {
        List<UpgradeData> upgrades = new List<UpgradeData>();

        // Basic upgrades 
        upgrades.Add(new UpgradeData("Damage Up", "Increase weapon damage by 20%", UpgradeType.Damage));
        upgrades.Add(new UpgradeData("Fire Rate Up", "Increase fire rate by 30%", UpgradeType.FireRate));
        upgrades.Add(new UpgradeData("Range Up", "Increase weapon range by 2", UpgradeType.Range));
        upgrades.Add(new UpgradeData("Speed Up", "Increase movement speed by 15%", UpgradeType.Speed));
        upgrades.Add(new UpgradeData("Health Up", "Increase max health by 20", UpgradeType.Health));

        // Shuffle for variety
        for (int i = 0; i < upgrades.Count; i++)
        {
            UpgradeData temp = upgrades[i];
            int randomIndex = Random.Range(i, upgrades.Count);
            upgrades[i] = upgrades[randomIndex];
            upgrades[randomIndex] = temp;
        }

        return upgrades;
    }

    void CreateUpgradeButton(UpgradeData upgrade)
    {
        GameObject buttonObj = Instantiate(upgradeButtonPrefab, upgradeButtonParent);
        UpgradeButton upgradeButton = buttonObj.GetComponent<UpgradeButton>();

        if (upgradeButton != null)
        {
            upgradeButton.Setup(upgrade, this);
        }
    }

    public void SelectUpgrade(UpgradeData upgrade)
    {
        // Apply upgrade
        ApplyUpgrade(upgrade);

        // Hide panel and resume game
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.Damage:
                ApplyDamageUpgrade();
                break;
            case UpgradeType.FireRate:
                ApplyFireRateUpgrade();
                break;
            case UpgradeType.Range:
                ApplyRangeUpgrade();
                break;
            case UpgradeType.Speed:
                ApplySpeedUpgrade();
                break;
            case UpgradeType.Health:
                ApplyHealthUpgrade();
                break;
        }

        Debug.Log($"Applied upgrade: {upgrade.name}");
    }

    void ApplyDamageUpgrade()
    {
        BasicWeapon weapon = GetComponentInChildren<BasicWeapon>();
        if (weapon != null)
            weapon.UpgradeDamage(1.2f);
    }

    void ApplyFireRateUpgrade()
    {
        BasicWeapon weapon = GetComponentInChildren<BasicWeapon>();
        if (weapon != null)
            weapon.UpgradeFireRate(1.3f);
    }

    void ApplyRangeUpgrade()
    {
        BasicWeapon weapon = GetComponentInChildren<BasicWeapon>();
        if (weapon != null)
            weapon.UpgradeRange(2f);
    }

    void ApplySpeedUpgrade()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
            playerController.moveSpeed *= 1.15f;
    }

    void ApplyHealthUpgrade()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.IncreaseMaxHealth(20f);
    }

    // Public getters
    public int GetCurrentLevel() { return currentLevel; }
    public float GetCurrentXP() { return currentXP; }
    public float GetXPRequired() { return xpRequiredForNextLevel; }
}
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
    Health
}
