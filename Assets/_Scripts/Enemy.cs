using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class Enemy : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;
    [SerializeField]
    protected Transform groundPoint;
    [SerializeField]
    protected Transform forwardGroundPoint;
    [SerializeField]
    protected Transform forwardPoint;

    public float health;
    public float hitRecoveryTime;
    public float attackRecoveryTime;

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

    private bool recovering = false;
    private bool hasDied = false;

    private IEnumerator hitCoroutine;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hitCoroutine = OnHit();
        direction = new Vector2(1.0f, 0.0f);
    }

    protected virtual void FixedUpdate()
    {
        Move();
        UpdateAnimationMovementProperties();
    }

    public void ApplyDamage(float damage, float playerXposition)
    {
        if (hasDied) return;
        StopCoroutine(hitCoroutine);
        health -= damage;
        float incomingDamageDirectionX = playerXposition - transform.position.x;
        rb.AddForce(Vector2.up + new Vector2(-incomingDamageDirectionX, 0.0f).normalized, ForceMode2D.Impulse);
        if (health > 0)
        {
            GetComponent<Animator>().SetTrigger("Hit");
            recovering = true;
            StartCoroutine(OnHit());
        }
        else
        {
            Die();
        }
        
    }

    protected void Die()
    {
        GetComponent<Animator>().SetTrigger("Death");
        hasDied = true;
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
        if (hasDied || recovering) return;
        UpdateMovementComponents();
        if (!isMoving || !isGrounded) return;
        
        if (isForwardGroundDetected)
        {
            rb.AddForce(new Vector2(-transform.localScale.x * moveAcceleration, 0.0f), ForceMode2D.Impulse);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxMoveSpeed, maxMoveSpeed), rb.velocity.y);
        }
        else if (!isForwardGroundDetected || isForwardObjectDetected)
        {
            Flip();
        }
    }

    protected virtual void UpdateAnimationMovementProperties()
    {
        anim.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
    }

    protected void Flip()
    {
        direction *= -1.0f;
        transform.localScale = new Vector3(transform.localScale.x * direction.x, transform.localScale.y, transform.localScale.z);
        
    }

    IEnumerator OnHit()
    {
        yield return new WaitForSeconds(hitRecoveryTime);
        recovering = false;
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
