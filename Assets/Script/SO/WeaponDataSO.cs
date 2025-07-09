using UnityEngine;
[CreateAssetMenu(fileName = "New Weapon", menuName = "Indonesian Roguelike/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Basic Info")]
    public string weaponName;
    public string description;
    public WeaponType weaponType;
    public WeaponCategory category;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;

    [Header("Stats")]
    public float damage = 10f;
    public float fireRate = 1f;
    public float range = 5f;
    public float speed = 10f;
    public float lifetime = 3f;

    [Header("Special Properties")]
    public bool canPierce = false;
    public int pierceCount = 1;
    public float knockbackForce = 0f;
    public float critChance = 0f;
    public float critMultiplier = 2f;

    [Header("Visual/Audio")]
    public GameObject projectilePrefab;
    public GameObject hitEffectPrefab;
    public AudioClip fireSound;
    public AudioClip hitSound;
}

public enum WeaponType
{
    Melee,    // STR - Cerurit
    Ranged,   // INT - Paku
    Homing,   // LCK - Keris
    AOE       // WIS - Air Garem
}

public enum WeaponCategory
{
    Primary,
    Secondary
}
