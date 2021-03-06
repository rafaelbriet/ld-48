using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    [SerializeField]
    private GameObject grenade;
    [SerializeField]
    private float launchForce = 5f;

    private PlayerController playerController;
    private BoxCollider2D boxCollider;
    private int launchDirection = 1;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void Launch()
    {
        Vector3 spawnPosition = transform.position;

        if (playerController != null)
        {
            if (playerController.IsFacingRight)
            {
                spawnPosition = new Vector3(boxCollider.bounds.center.x + boxCollider.bounds.extents.x + 0.25f, boxCollider.bounds.center.y, 0);
                launchDirection = 1;
            }
            else
            {
                spawnPosition = new Vector3(boxCollider.bounds.center.x - boxCollider.bounds.extents.x - 0.25f, boxCollider.bounds.center.y, 0);
                launchDirection = -1;
            }
        }
        else
        {
            Skeleton skeleton = GetComponent<Skeleton>();

            Vector3 direction = skeleton.Target.position - transform.position;
            float dot = Vector3.Dot(direction.normalized, transform.right);

            if (dot > 0)
            {
                spawnPosition = new Vector3(boxCollider.bounds.center.x + boxCollider.bounds.extents.x + 0.3f, boxCollider.bounds.center.y, 0);
                launchDirection = 1;
            }
            else if (dot < 0)
            {
                spawnPosition = new Vector3(boxCollider.bounds.center.x - boxCollider.bounds.extents.x - 0.3f, boxCollider.bounds.center.y, 0);
                launchDirection = -1;
            }
        }

        GameObject grenadeGo = Instantiate(grenade, spawnPosition, Quaternion.identity);

        grenadeGo.GetComponent<Grenade>().Owner = gameObject;

        Rigidbody2D grenadeRb = grenadeGo.GetComponent<Rigidbody2D>();

        Vector2 force = new Vector2(launchForce * launchDirection, launchForce);
        grenadeRb.AddForce(force, ForceMode2D.Impulse);
    }
}
