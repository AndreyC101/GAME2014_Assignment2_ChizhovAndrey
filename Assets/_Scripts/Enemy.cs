using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;
    protected EnemySenses sense;
    [SerializeField]
    protected Transform groundPoint;
    [SerializeField]
    protected Transform forwardGroundPoint;
    [SerializeField]
    protected Transform forwardPoint;

    [Header("Health")]
    public float health;
    public float hitRecoveryTime;

    [Header("Navigation")]
    public bool isMoving;
    public float maxMoveSpeed;
    public float moveAcceleration;
    public Vector2 direction;

    public LayerMask groundLayerMask;
    public LayerMask hazardLayerMask;
    public float groundDetectionRadius;
    public float forwardGroundDetectionRadius;
    public float forwardDetectionRadius;

    private bool isGrounded;
    private bool isForwardGroundDetected;
    private bool isForwardObjectDetected;

    private bool hitRecovering;
    private bool hasDied;

    public bool attackAvailable;
    public bool isAttacking;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sense = GetComponentInChildren<EnemySenses>();
        direction = new Vector2(-1.0f, 0.0f);
        anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("SkeletonAnimationController");
        hitRecovering = false;
        hasDied = false;
        attackAvailable = true;
        isAttacking = false;
}

    protected virtual void FixedUpdate()
    {
        if (hasDied) return;
        if (isMoving) Move();
        if (isMoving && sense.playerBehind) Flip();
        UpdateAnimationMovementProperties();
    }

    public virtual void ApplyDamage(float damage, float playerXposition)
    {
        if (hasDied) return;
        StopAllCoroutines();
        health -= damage;
        float incomingDamageDirectionX = playerXposition - transform.position.x;
        rb.AddForce(Vector2.up + new Vector2(-incomingDamageDirectionX, 0.0f).normalized * 5.0f, ForceMode2D.Impulse);
        if (health > 0)
        {
            GetComponent<Animator>().SetTrigger("Hit");
            hitRecovering = true;
            StartCoroutine(OnHit());
        }
        else
        {
            Die();
        }
        
    }

    protected void Die()
    {
        SoundManager.Instance.PlayAudio(SoundID.Enemy_Death);
        hasDied = true;
        StopAllCoroutines();
        GetComponent<Animator>().SetTrigger("Death");
        StartCoroutine(OnDied());
    }

    protected void SetupMovementComponents()
    {
        groundPoint = transform.GetChild(0).GetComponent<Transform>();
        forwardGroundPoint = transform.GetChild(1).GetComponent<Transform>();
        forwardPoint = transform.GetChild(2).GetComponent<Transform>();
    }

    protected void UpdateMovementComponents()
    {
        var groundHit = Physics2D.OverlapCircle(groundPoint.position, groundDetectionRadius, groundLayerMask);
        isGrounded = groundHit;

        var killboxHit = Physics2D.OverlapCircle(groundPoint.position, groundDetectionRadius, hazardLayerMask);
        if (killboxHit) Die();

        var forwardGroundHit = Physics2D.OverlapCircle(forwardGroundPoint.position, forwardGroundDetectionRadius, groundLayerMask);
        isForwardGroundDetected = forwardGroundHit;

        var forwardHit = Physics2D.OverlapCircle(forwardPoint.position, forwardDetectionRadius, groundLayerMask);
        isForwardObjectDetected = forwardHit;
    }

    protected void Move()
    {
        UpdateMovementComponents();

        if (!IsActionReady()) return;

        if (isForwardGroundDetected)
        {
            rb.AddForce(new Vector2(-transform.localScale.x * moveAcceleration, 0.0f), ForceMode2D.Impulse);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxMoveSpeed, maxMoveSpeed), rb.velocity.y);
        }
        
        if ((!isForwardGroundDetected || isForwardObjectDetected))
        {
            if (!sense.lineOfSight) Flip();
        }
    }

    protected virtual void UpdateAnimationMovementProperties()
    {
        anim.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
    }

    protected void Flip()
    {
        direction *= -1.0f;
        transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
    }

    protected bool IsActionReady()
    {
        return (!hitRecovering && !hasDied && !isAttacking && isGrounded);
    }

    IEnumerator OnHit()
    {
        yield return new WaitForSeconds(hitRecoveryTime);
        hitRecovering = false;
        isMoving = true;
        isAttacking = false;
        attackAvailable = true;
        anim.SetTrigger("HitRecover");
    }

    IEnumerator OnDied()
    {
        yield return new WaitForSeconds(5.0f);
        Destroy(this.gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundPoint.transform.position, groundDetectionRadius);
        Gizmos.DrawWireSphere(forwardGroundPoint.transform.position, forwardGroundDetectionRadius);
        Gizmos.DrawWireSphere(forwardPoint.transform.position, forwardDetectionRadius);
    }
}
