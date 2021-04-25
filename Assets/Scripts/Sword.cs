using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField]
    private Vector2 attackSize = new Vector2(1f, 0.5f);
    [SerializeField]
    private LayerMask layerMask;

    private PlayerController playerController;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Attack()
    {
        Vector2 point;

        if (playerController.IsFacingRight)
        {
            point = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
        }
        else
        {
            point = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
        }

        Collider2D collider = Physics2D.OverlapBox(point, attackSize, 0f, layerMask);

        if (collider != null)
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

    private void OnDrawGizmos()
    {
        if (boxCollider != null)
        {
            Vector2 point;

            if (playerController.IsFacingRight)
            {
                point = new Vector2(boxCollider.bounds.center.x + boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
            }
            else
            {
                point = new Vector2(boxCollider.bounds.center.x - boxCollider.bounds.extents.x, boxCollider.bounds.center.y);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(point, attackSize);
        }
    }
}
