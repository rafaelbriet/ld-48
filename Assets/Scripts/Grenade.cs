using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField]
    private string tagToCompare = "Enemy";
    [SerializeField]
    private Vector2 size = new Vector2(2f, 2f);
    [SerializeField]
    private AudioClip grenadeExplodeAudio;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

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

        if (audioSource != null)
        {
            audioSource.PlayOneShot(grenadeExplodeAudio);
        }

        StartCoroutine(DestroyGrenade());
    }

    private IEnumerator DestroyGrenade()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

        while (audioSource.isPlaying)
        {
            yield return null;
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
