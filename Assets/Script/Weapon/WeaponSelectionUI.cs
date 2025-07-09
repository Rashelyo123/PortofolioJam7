using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Weapon Selection UI
public class WeaponSelectionUI : MonoBehaviour
{
    [Header("Weapon Selection")]
    public WeaponDataSO[] primaryWeapons;
    public WeaponDataSO[] secondaryWeapons;
    public WeaponManager weaponManager;

    [Header("UI Elements")]
    public GameObject selectionPanel;
    public Transform weaponButtonParent;
    public GameObject weaponButtonPrefab;

    void Start()
    {
        if (selectionPanel != null)
        {
            selectionPanel.SetActive(true);
            CreateWeaponButtons();
        }
    }

    void CreateWeaponButtons()
    {
        foreach (WeaponDataSO weapon in primaryWeapons)
        {
            CreateWeaponButton(weapon);
        }
    }

    void CreateWeaponButton(WeaponDataSO weaponData)
    {
        GameObject buttonObj = Instantiate(weaponButtonPrefab, weaponButtonParent);
        WeaponButton button = buttonObj.GetComponent<WeaponButton>(); // Ganti ini

        if (button != null)
        {
            button.Setup(weaponData, this);
        }
    }
    public void SelectPrimaryWeapon(WeaponDataSO weaponData)
    {
        weaponManager.SetPrimaryWeapon(weaponData);

        if (selectionPanel != null)
        {
            selectionPanel.SetActive(false);
        }

        // Start game
        Time.timeScale = 1f;
    }

    public WeaponDataSO GetRandomSecondaryWeapon()
    {
        if (secondaryWeapons.Length == 0) return null;

        int randomIndex = Random.Range(0, secondaryWeapons.Length);
        return secondaryWeapons[randomIndex];
    }
}

