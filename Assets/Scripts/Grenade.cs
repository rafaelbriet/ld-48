using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    private string tagToCompare = "Enemy";
    [SerializeField]
    private Vector2 size = new Vector2(2f, 2f);

    private void Awake()
    {
        StartCoroutine(GrenadeTimer());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    private void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, size, 0);

        foreach (Collider2D collider in colliders)
        {
            if (tagToCompare == "Enemy")
            {
                if (collider.CompareTag(tagToCompare))
                {
                    if (collider.TryGetComponent<Satan>(out Satan satan))
                    {
                        satan.Damage();
                    }
                    else
                    {
                        Destroy(collider.gameObject);
                    }
                }
            }
            else if (tagToCompare == "Player")
            {
                if (collider.CompareTag(tagToCompare))
                {
                    if (collider.TryGetComponent<Character>(out Character character))
                    {
                        character.Damage();
                    }
                }
            }
        }

        Destroy(gameObject);
    }

    private IEnumerator GrenadeTimer()
    {
        float timer = 0f;

        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Explode();
    }
}
