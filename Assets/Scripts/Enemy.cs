using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float speed = 2;
    [SerializeField]
    protected float detectionRadius = 4f;
    [SerializeField]
    protected float isGroundedDistance = 0.1f;
    [SerializeField]
    protected LayerMask environmentLayerMask;
    [SerializeField]
    protected LayerMask playerLayerMask;

    protected BoxCollider2D boxCollider;
    protected int horizontalDirection;
    protected bool canChangeDirection = true;
    protected Transform target;

    protected void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        horizontalDirection = (Random.Range(0, 2) * 2) - 1;
    }

    protected bool CanSeePlayer()
    {
        Vector3 direction = target.position - transform.position;

        RaycastHit2D hit = Physics2D.Raycast(boxCollider.bounds.center, direction, Mathf.Infinity);

        if (Debug.isDebugBuild)
        {
            Debug.DrawRay(boxCollider.bounds.center, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.blue);
        }

        return hit.transform.CompareTag("Player");
    }

    protected bool IsPlayerInRange()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayerMask);

        if (collider != null)
        {
            target = collider.transform;
        }

        return collider != null;
    }

    protected bool IsFacingWall()
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

    protected bool IsGroundend()
    {
        RaycastHit2D hit = Physics2D.Raycast(boxCollider.bounds.center, Vector3.down, 1f + isGroundedDistance, environmentLayerMask);

        if (Debug.isDebugBuild)
        {
            Debug.DrawRay(boxCollider.bounds.center, Vector3.down, Color.red);
        }

        return hit.collider != null;
    }

    protected void FlipHorizontalDirection()
    {
        horizontalDirection = horizontalDirection * -1;
    }

    protected void ChangeDirection()
    {
        if (canChangeDirection && (IsGroundend() == false || IsFacingWall()))
        {
            StartCoroutine(ChangeDirectionCoolDown());
            FlipHorizontalDirection();
        }
    }

    protected IEnumerator ChangeDirectionCoolDown()
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
}
