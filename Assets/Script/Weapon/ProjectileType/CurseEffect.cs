using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Curse Effect untuk Boneka Santet
public class CurseEffect : MonoBehaviour
{
    private float duration;
    private float dotDamage;
    private float timeRemaining;
    private Enemy enemyHealth;
    private bool isActive;

    void Start()
    {
        enemyHealth = GetComponent<Enemy>();
    }

    public void ApplyCurse(float curseDuration, float damage)
    {
        duration = curseDuration;
        dotDamage = damage;
        timeRemaining = duration;
        isActive = true;

        if (!enabled)
        {
            enabled = true;
            StartCoroutine(CurseCoroutine());
        }
        else
        {
            // Refresh curse duration
            timeRemaining = duration;
        }
    }

    IEnumerator CurseCoroutine()
    {
        while (isActive && timeRemaining > 0)
        {
            yield return new WaitForSeconds(1f);

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(dotDamage);
            }

            timeRemaining -= 1f;
        }

        isActive = false;
        enabled = false;
    }
}

