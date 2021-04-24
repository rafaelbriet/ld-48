using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private int currentHealth = 4;

    public event Action CharacterDied;
    public event Action CharacterTookDamage;

    private void OnDisable()
    {
        CharacterDied?.Invoke();
    }

    public void Damage()
    {
        currentHealth--;
        CharacterTookDamage?.Invoke();

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }

    }
}
