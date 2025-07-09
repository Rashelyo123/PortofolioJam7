using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Optimized Experience Manager dengan integrasi yang lebih baik
public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    [Header("Experience Settings")]
    public float baseXPRequired = 10f;
    public float xpGrowthRate = 1.5f;
    public int maxLevel = 100;

    [Header("Level Up Panel")]
    public GameObject levelUpPanel;
    public Transform upgradeButtonParent;
    public GameObject upgradeButtonPrefab;

    // Current stats
    private int currentLevel = 1;
    private float currentXP = 0f;
    private float xpRequiredForNextLevel;

    // Events - UI akan listen ke events ini
    public System.Action<int> OnLevelUp;
    public System.Action<float> OnXPGained;
    public System.Action<float> OnXPProgressChanged;
    public System.Action<int> OnLevelChanged;

    // Cached components untuk performance
    private PlayerController cachedPlayerController;
    private PlayerHealth cachedPlayerHealth;
    private BasicWeapon cachedWeapon;
    private WeaponSelectionUI cachedWeaponUI;
    private WeaponManager cachedWeaponManager;

    // Enhanced upgrade system
    private EnhancedUpgradeSystem upgradeSystem;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeComponents();
        CalculateXPRequired();

        // Trigger initial UI update
        OnXPProgressChanged?.Invoke(currentXP / xpRequiredForNextLevel);
        OnLevelChanged?.Invoke(currentLevel);

        // Hide level up panel initially
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }

    void InitializeComponents()
    {
        // Cache components untuk menghindari GetComponent() berulang
        cachedPlayerController = FindObjectOfType<PlayerController>();
        cachedPlayerHealth = FindObjectOfType<PlayerHealth>();
        cachedWeapon = FindObjectOfType<BasicWeapon>();
        cachedWeaponUI = FindObjectOfType<WeaponSelectionUI>();
        cachedWeaponManager = FindObjectOfType<WeaponManager>();

        // Initialize enhanced upgrade system
        upgradeSystem = new EnhancedUpgradeSystem(
            cachedPlayerController,
            cachedPlayerHealth,
            cachedWeapon,
            cachedWeaponUI,
            cachedWeaponManager
        );
    }

    public void GainXP(float amount)
    {
        if (currentLevel >= maxLevel) return;

        currentXP += amount;
        OnXPGained?.Invoke(amount);

        // Check for level up
        while (currentXP >= xpRequiredForNextLevel && currentLevel < maxLevel)
        {
            LevelUp();
        }

        // Update UI via events
        OnXPProgressChanged?.Invoke(currentXP / xpRequiredForNextLevel);
    }

    void LevelUp()
    {
        currentXP -= xpRequiredForNextLevel;
        currentLevel++;

        CalculateXPRequired();

        // Trigger events
        OnLevelUp?.Invoke(currentLevel);
        OnLevelChanged?.Invoke(currentLevel);

        // Show upgrade selection
        ShowUpgradeSelection();

        Debug.Log($"Level Up! Now Level {currentLevel}");
    }

    void CalculateXPRequired()
    {
        xpRequiredForNextLevel = baseXPRequired * Mathf.Pow(xpGrowthRate, currentLevel - 1);
    }

    void ShowUpgradeSelection()
    {
        if (levelUpPanel == null) return;

        // Pause game
        Time.timeScale = 0f;

        // Show panel
        levelUpPanel.SetActive(true);

        // Generate upgrade options
        StartCoroutine(GenerateUpgradeOptionsCoroutine());
    }

    // Menggunakan coroutine untuk menghindari frame drop
    IEnumerator GenerateUpgradeOptionsCoroutine()
    {
        if (upgradeButtonParent == null || upgradeButtonPrefab == null) yield break;

        // Clear existing buttons
        foreach (Transform child in upgradeButtonParent)
        {
            Destroy(child.gameObject);
        }

        yield return null; // Wait one frame

        // Get available upgrades
        List<UpgradeData> availableUpgrades = upgradeSystem.GetAvailableUpgrades();

        // Create buttons for upgrades (max 3 options)
        int optionsToShow = Mathf.Min(3, availableUpgrades.Count);

        for (int i = 0; i < optionsToShow; i++)
        {
            CreateUpgradeButton(availableUpgrades[i]);
            if (i % 2 == 0) yield return null; // Spread creation across frames
        }
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
        // Apply upgrade melalui upgrade system
        upgradeSystem.ApplyUpgrade(upgrade);

        // Hide panel and resume game
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    // Public getters
    public int GetCurrentLevel() { return currentLevel; }
    public float GetCurrentXP() { return currentXP; }
    public float GetXPRequired() { return xpRequiredForNextLevel; }
    public float GetXPProgress() { return currentXP / xpRequiredForNextLevel; }
}
