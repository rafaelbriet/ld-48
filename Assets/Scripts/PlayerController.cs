using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpsForce;
    [SerializeField]
    private float gravityScale;
    [SerializeField]
    private Vector2 groundCheckSize = new Vector2(0.1f, 0.2f);
    [SerializeField]
    private Vector2 wallCheckSize = new Vector2(0.1f, 1f);
    [SerializeField]

    private LayerMask layerMask;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private bool isGrounded = true;
    private Sword sword;
    private GameManager gameManager;
    private int jumpCount;

    public bool IsFacingRight { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        sword = GetComponent<Sword>();
    }

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager.CanDoubleJump == false)
        {
            jumpCount = 99;
        }
    }

    private void Update()
    {
        Move();
        Jump();
        CheckGrounded();
        CheckDoubleJump();

        if (Input.GetButtonDown("Fire1"))
        {
            sword.Attack();
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        FacingRightCheck(horizontal);

        Vector2 leftPoint = new Vector2(boxCollider2D.bounds.center.x - boxCollider2D.bounds.extents.x, boxCollider2D.bounds.center.y);
        Vector2 leftSize = new Vector2(wallCheckSize.x, wallCheckSize.y);

        Collider2D leftCollider = Physics2D.OverlapBox(leftPoint, leftSize, 0f, layerMask);

        Vector2 rightCenter = new Vector2(boxCollider2D.bounds.center.x + boxCollider2D.bounds.extents.x, boxCollider2D.bounds.center.y);
        Vector2 rightSize = new Vector2(wallCheckSize.x, wallCheckSize.y);

        Collider2D rightCollider = Physics2D.OverlapBox(rightCenter, rightSize, 0f, layerMask);

        if (leftCollider != null && horizontal < 0)
        {
            horizontal = 0;
        }
        else if (rightCollider != null && horizontal > 0)
        {
            horizontal = 0;
        }

        Vector3 translations = new Vector3(horizontal * speed * Time.deltaTime, 0, 0);

        transform.Translate(translations);
    }

    private void FacingRightCheck(float horizontal)
    {
        if (horizontal > 0)
        {
            IsFacingRight = true;
        }
        else if (horizontal < 0)
        {
            IsFacingRight = false;
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && (jumpCount < 1 || isGrounded))
        {
            jumpCount++;
            rb.velocity = new Vector2(rb.velocity.x, jumpsForce);
        }

        if (rb.velocity.y >= 0)
        {
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private void CheckGrounded()
    {
        Vector2 point = new Vector2(boxCollider2D.bounds.center.x, boxCollider2D.bounds.center.y - boxCollider2D.bounds.extents.y);
        Vector2 size = new Vector2(groundCheckSize.x, groundCheckSize.y);

        Collider2D collider = Physics2D.OverlapBox(point, size, 0f, layerMask);

        isGrounded = collider != null;
    }

    private void CheckDoubleJump()
    {
        if (isGrounded && gameManager.CanDoubleJump)
        {
            jumpCount = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider2D != null)
        {
            Vector3 center = new Vector3(boxCollider2D.bounds.center.x, boxCollider2D.bounds.center.y - boxCollider2D.bounds.extents.y, boxCollider2D.bounds.center.z);
            Vector2 size = new Vector2(groundCheckSize.x, groundCheckSize.y);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, size);

            Vector2 leftCenter = new Vector2(boxCollider2D.bounds.center.x - boxCollider2D.bounds.extents.x, boxCollider2D.bounds.center.y);
            Vector2 leftSize = new Vector2(wallCheckSize.x, wallCheckSize.y);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(leftCenter, leftSize);

            Vector2 rightCenter = new Vector2(boxCollider2D.bounds.center.x + boxCollider2D.bounds.extents.x, boxCollider2D.bounds.center.y);
            Vector2 rightSize = new Vector2(wallCheckSize.x, wallCheckSize.y);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(rightCenter, rightSize);
        }
    }
}
