using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SatanStates { Idle, MoveTowards, AttackJump, AttackDash, Stunt }

public class Satan : Enemy
{
    [Header("AI")]
    [SerializeField]
    private int currentHealth = 8;
    [SerializeField]
    private float dashAttackRange = 8f;
    [SerializeField]
    private float dashAttackSpeed = 8f;
    [SerializeField]
    private float dashAttackChance = 0.5f;
    [SerializeField]
    private float jumpAttackRange = 4f;
    [SerializeField]
    private float jumpForce = 4f;
    [SerializeField]
    private float jumpCooldown = 5f;
    [SerializeField]
    private float gravityScale = 2f;
    [SerializeField]
    private Vector2 jumpAttackSize = new Vector2(4f, 0.2f);

    [Header("Audio")]
    [SerializeField]
    private AudioClip dashAttackAudio;
    [SerializeField]
    private AudioClip jumpAudio;
    [SerializeField]
    private AudioClip jumpLandAudio;
    [SerializeField]
    private AudioClip dashCollisionAudio;

    private SatanStates currentState;
    private Rigidbody2D rb;
    private bool canJump = true;
    private bool hasJumpAttacked = false;
    private bool hasDashed = false;
    private Coroutine dashCooldownCoroutine;
    private AudioSource audioSource;

    public event Action SatanDied;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        ChangeState(SatanStates.Idle);
    }

    private void Update()
    {
        switch (currentState)
        {
            case SatanStates.Idle:
                Idle();
                break;
            case SatanStates.MoveTowards:
                MoveTowards();
                break;
            case SatanStates.AttackJump:
                AttackJump();
                break;
            case SatanStates.AttackDash:
                AttackDash();
                break;
            case SatanStates.Stunt:
                Stunt();
                break;
            default:
                break;
        }
    }

    public void Damage()
    {
        currentHealth--;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            SatanDied?.Invoke();
        }
    }

    private void Idle()
    {
        if (IsPlayerInRange())
        {
            ChangeState(SatanStates.MoveTowards);
        }
    }

    private void MoveTowards()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        float dashChance = UnityEngine.Random.Range(0f, 1f);

        if (dashChance <= dashAttackChance && Vector3.Distance(transform.position, target.position) < dashAttackRange)
        {
            ChangeState(SatanStates.AttackDash);
        }
        else if (Vector3.Distance(transform.position, target.position) < jumpAttackRange)
        {
            ChangeState(SatanStates.AttackJump);
        }
    }

    private void AttackJump()
    {
        if (dashCooldownCoroutine != null)
        {
            StopCoroutine(dashCooldownCoroutine);
            dashCooldownCoroutine = null;
            hasDashed = false;
        }

        if (canJump && IsGroundend())
        {
            audioSource.PlayOneShot(jumpAudio);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            StartCoroutine(JumpCooldown());
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasJumpAttacked && collision.gameObject.layer == 6)
        {
            Vector2 point = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.extents.y);

            Collider2D collider = Physics2D.OverlapBox(point, jumpAttackSize, 0f, playerLayerMask);

            if (collider != null)
            {
                collider.GetComponent<Character>().Damage();
            }

            audioSource.PlayOneShot(jumpLandAudio);

            ChangeState(SatanStates.MoveTowards);
        }

        if (hasDashed && collision.transform.CompareTag("Player"))
        {
            Physics2D.IgnoreCollision(boxCollider, collision.collider, true);
            StartCoroutine(CollisionCooldown(collision.collider));
            target.GetComponent<Character>().Damage();
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

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        hasJumpAttacked = true;

        float timer = 0f;

        while (timer < jumpCooldown)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        canJump = true;
        hasJumpAttacked = false;
    }

    private void AttackDash()
    {
        if (hasDashed == false)
        {
            Vector3 direction = target.position - transform.position;
            float dot = Vector3.Dot(direction.normalized, transform.right);

            if (dot > 0)
            {
                horizontalDirection = 1;
            }
            else if (dot < 0)
            {
                horizontalDirection = -1;
            }

            audioSource.PlayOneShot(dashAttackAudio);

            dashCooldownCoroutine = StartCoroutine(DashCooldown());

            hasDashed = true;
        }
    }

    private IEnumerator DashCooldown()
    {
        hasDashed = true;

        while (IsFacingWall() == false)
        {
            Vector3 translation = new Vector3(horizontalDirection * dashAttackSpeed * Time.deltaTime, 0, 0);
            transform.Translate(translation);

            yield return null;
        }

        hasDashed = false;
        audioSource.PlayOneShot(dashCollisionAudio);
        ChangeState(SatanStates.Stunt);
    }

    private void Stunt()
    {
        StartCoroutine(StuntCooldown());
    }

    private IEnumerator StuntCooldown()
    {
        float timer = 0f;

        while (timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        ChangeState(SatanStates.MoveTowards);
    }

    private void ChangeState(SatanStates nextState)
    {
        currentState = nextState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (boxCollider != null)
        {
            Vector3 center = new Vector3(boxCollider.bounds.center.x, boxCollider.bounds.center.y - boxCollider.bounds.extents.y, boxCollider.bounds.center.z);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, jumpAttackSize);
        }
    }
}
