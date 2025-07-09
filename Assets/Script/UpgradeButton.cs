using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class UpgradeButton : MonoBehaviour
{
    [Header("UI References")]
    public TMPro.TextMeshProUGUI upgradeName;
    public TMPro.TextMeshProUGUI upgradeDescription;
    public UnityEngine.UI.Button button;

    private UpgradeData upgradeData;
    private ExperienceManager experienceManager;

    public void Setup(UpgradeData data, ExperienceManager manager)
    {
        upgradeData = data;
        experienceManager = manager;

        if (upgradeName != null)
            upgradeName.text = data.name;

        if (upgradeDescription != null)
            upgradeDescription.text = data.description;

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        experienceManager.SelectUpgrade(upgradeData);
    }
}
