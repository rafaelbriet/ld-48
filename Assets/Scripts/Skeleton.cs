using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkeletonStates { Patrol, MoveTowards, Attack }

public class Skeleton : Enemy
{
    
    [SerializeField]
    private float attackRange = 4f;
    [SerializeField]
    private float grenadeCooldown = 1f;
    [SerializeField]
    private AudioClip attasckAudio;

    private SkeletonStates currentState;
    private GrenadeLauncher grenade;
    private bool hasLaunchedGrenade;
    private AudioSource audioSource;

    public Transform Target => target;

    protected void Start()
    {
        grenade = GetComponent<GrenadeLauncher>();
        audioSource = GetComponent<AudioSource>();

        ChangeState(SkeletonStates.Patrol);
    }

    private void Update()
    {
        switch (currentState)
        {
            case SkeletonStates.Patrol:
                Patrol();
                break;
            case SkeletonStates.MoveTowards:
                MoveTowards();
                break;
            case SkeletonStates.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void Patrol()
    {
        Vector3 translation = new Vector3(horizontalDirection * speed * Time.deltaTime, 0, 0);

        transform.Translate(translation);

        ChangeDirection();

        if (IsPlayerInRange())
        {
            ChangeState(SkeletonStates.MoveTowards);
        }
    }

    private void MoveTowards()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (CanSeePlayer() == false)
        {
            target = null;
            ChangeState(SkeletonStates.Patrol);
        }
        else if (Vector3.Distance(transform.position, target.position) < attackRange)
        {
            ChangeState(SkeletonStates.Attack);
        }
    }

    private void Attack()
    {
        if (hasLaunchedGrenade == false)
        {
            grenade.Launch();

            if (audioSource != null)
            {
                audioSource.PlayOneShot(attasckAudio);
            }
            
            StartCoroutine(GrenadeCooldown());
        }

        if (CanSeePlayer() == false)
        {
            target = null;
            ChangeState(SkeletonStates.Patrol);
        }
        else if (Vector3.Distance(transform.position, target.position) > attackRange)
        {
            ChangeState(SkeletonStates.MoveTowards);
        }
    }

    private IEnumerator GrenadeCooldown()
    {
        hasLaunchedGrenade = true;
        float timer = 0;

        while (timer < grenadeCooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        hasLaunchedGrenade = false;
    }

    private void ChangeState(SkeletonStates nextState)
    {
        currentState = nextState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
