using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponButton : MonoBehaviour
{
    [Header("UI Components")]
    public Button button;
    public TextMeshProUGUI weaponNameText;
    public Image weaponIcon;

    private WeaponDataSO weaponData;
    private WeaponSelectionUI selectionUI;

    public void Setup(WeaponDataSO weapon, WeaponSelectionUI ui)
    {
        weaponData = weapon;
        selectionUI = ui;

        // Update UI elements
        if (weaponNameText != null)
            weaponNameText.text = weapon.weaponName;

        if (weaponIcon != null && weapon.weaponIcon != null)
            weaponIcon.sprite = weapon.weaponIcon;

        // Setup button click event
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }
    }

    void OnButtonClick()
    {
        if (selectionUI != null && weaponData != null)
        {
            selectionUI.SelectPrimaryWeapon(weaponData);
        }
    }
}