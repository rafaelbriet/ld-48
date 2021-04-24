using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelFinish : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnTriggerEnter?.Invoke();
        }
    }
}
