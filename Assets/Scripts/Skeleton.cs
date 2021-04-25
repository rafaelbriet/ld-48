using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkeletonStates { Patrol, MoveTowards, Attack }

public class Skeleton : MonoBehaviour
{
    [SerializeField]
    private float speed = 2;
    [SerializeField]
    private float detectionRadius = 4f;
    [SerializeField]
    private float attackRange = 4f;
    [SerializeField]
    private LayerMask environmentLayerMask;
    [SerializeField]
    private LayerMask playerLayerMask;

    private SkeletonStates currentState;
    private BoxCollider2D boxCollider;
    private int horizontalDirection;
    private bool canChangeDirection = true;
    private Transform target;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        horizontalDirection = (Random.Range(0, 2) * 2) - 1;

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

    private bool CanSeePlayer()
    {
        Vector3 direction = target.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(boxCollider.bounds.center, direction, Mathf.Infinity);

        if (Debug.isDebugBuild)
        {
            Debug.DrawRay(boxCollider.bounds.center, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.blue);
        }

        return hit.transform.CompareTag("Player");
    }

    private bool IsPlayerInRange()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayerMask);

        if (collider != null)
        {
            target = collider.transform;
        }

        return collider != null;
    }

    private bool IsFacingWall()
    {
        float rayDistance = 0.5f;

        RaycastHit2D hitLeft = Physics2D.Raycast(boxCollider.bounds.center, Vector3.left, rayDistance, environmentLayerMask);
        RaycastHit2D hitRight = Physics2D.Raycast(boxCollider.bounds.center, Vector3.right, rayDistance, environmentLayerMask);

        if (Debug.isDebugBuild)
        {
            Debug.DrawRay(boxCollider.bounds.center, Vector3.left, Color.red);
            Debug.DrawRay(boxCollider.bounds.center, Vector3.right, Color.red);
        }

        return hitLeft.collider != null || hitRight.collider != null;
    }

    private bool IsGroundend()
    {
        RaycastHit2D hit = Physics2D.Raycast(boxCollider.bounds.center, Vector3.down, 2f, environmentLayerMask);

        if (Debug.isDebugBuild)
        {
            Debug.DrawRay(boxCollider.bounds.center, Vector3.down, Color.red);
        }
        
        return hit.collider != null;
    }

    private void FlipHorizontalDirection()
    {
        horizontalDirection = horizontalDirection * -1;
    }

    private void ChangeDirection()
    {
        if (canChangeDirection && (IsGroundend() == false || IsFacingWall()))
        {
            StartCoroutine(ChangeDirectionCoolDown());
            FlipHorizontalDirection();
        }
    }

    private IEnumerator ChangeDirectionCoolDown()
    {
        canChangeDirection = false;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canChangeDirection = true;
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
