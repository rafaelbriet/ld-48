using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIStates { Idle, MoveTowards, Attack }

public class Bat : MonoBehaviour
{
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float attackSpeed = 4f;
    [SerializeField]
    private float detectionRadius = 3f;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private AudioClip batAudio;

    private Transform target;
    private BoxCollider2D boxCollider;
    private AIStates currentState = AIStates.Idle;
    private bool canAttack = true;
    private Coroutine attackCooldownCoroutine;
    private AudioSource audioSource;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case AIStates.Idle:
                Idle();
                break;
            case AIStates.MoveTowards:
                MoveTowards();

                if (audioSource.isPlaying == false)
                {
                    audioSource.Play();
                }

                break;
            case AIStates.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void Idle()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, detectionRadius, layerMask);

        if (collider != null)
        {
            target = collider.transform;

            ChangeState(AIStates.MoveTowards);
        }
    }

    private void MoveTowards()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void Attack()
    {
        Vector3 direction = transform.position - target.position - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, direction, attackSpeed * Time.deltaTime);

        if (attackCooldownCoroutine == null)
        {
            attackCooldownCoroutine = StartCoroutine(AttackCooldown());
        }

        if (canAttack == true)
        {
            StopCoroutine(attackCooldownCoroutine);
            attackCooldownCoroutine = null;
            ChangeState(AIStates.MoveTowards);
        }
    }

    private void ChangeState(AIStates nextState)
    {
        currentState = nextState;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(boxCollider, collision.collider, true);
            StartCoroutine(CollisionCooldown(collision.collider));
            target.GetComponent<Character>().Damage();
            ChangeState(AIStates.Attack);
        }
    }

    private IEnumerator CollisionCooldown(Collider2D other)
    {
        float timer = 0;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Physics2D.IgnoreCollision(boxCollider, other, false);
    }

    private IEnumerator AttackCooldown()
    {
        float timer = 0;
        canAttack = false;

        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
