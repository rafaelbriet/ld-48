using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkeletonStates { Patrol, MoveTowards, Attack }

public class Skeleton : Enemy
{
    
    [SerializeField]
    private float attackRange = 4f;

    private SkeletonStates currentState;

    protected void Start()
    {
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
        Debug.Log("Attaaaaack!!!!");

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

    private void ChangeState(SkeletonStates nextState)
    {
        currentState = nextState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
