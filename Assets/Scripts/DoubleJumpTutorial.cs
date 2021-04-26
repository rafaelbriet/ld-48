using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpTutorial : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(HideTutorial());
    }

    private IEnumerator HideTutorial()
    {
        float timer = 0;

        while (timer < 10f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FadeTutorial());
    }

    private IEnumerator FadeTutorial()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 0.05f;
            yield return null;
        }

        Destroy(gameObject);
    }
}
