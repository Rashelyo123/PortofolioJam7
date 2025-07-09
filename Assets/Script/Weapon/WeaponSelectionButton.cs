using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelectionButton : MonoBehaviour
{
    [Header("UI References")]
    public UnityEngine.UI.Image weaponIcon;
    public TMPro.TextMeshProUGUI weaponName;
    public TMPro.TextMeshProUGUI weaponDescription;
    public TMPro.TextMeshProUGUI weaponStats;
    public UnityEngine.UI.Button button;

    private WeaponDataSO weaponData;
    private WeaponSelectionUI selectionUI;

    public void Setup(WeaponDataSO data, WeaponSelectionUI ui)
    {
        weaponData = data;
        selectionUI = ui;

        if (weaponIcon != null && weaponData.weaponIcon != null)
            weaponIcon.sprite = weaponData.weaponIcon;

        if (weaponName != null)
            weaponName.text = weaponData.weaponName;

        if (weaponDescription != null)
            weaponDescription.text = weaponData.description;

        if (weaponStats != null)
        {
            weaponStats.text = $"Damage: {weaponData.damage}\n" +
                             $"Fire Rate: {weaponData.fireRate}\n" +
                             $"Range: {weaponData.range}";
        }

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        selectionUI.SelectPrimaryWeapon(weaponData);
    }
}