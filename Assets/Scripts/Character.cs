using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private int currentHealth = 4;

    public event Action CharacterDied;
    public event Action CharacterTookDamage;

    public void Damage()
    {
        currentHealth--;
        CharacterTookDamage?.Invoke();

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        gameObject.SetActive(false);
        CharacterDied?.Invoke();
    }
}
