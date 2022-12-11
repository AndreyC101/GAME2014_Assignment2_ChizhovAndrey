using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CheckpointSystem cs;
    [SerializeField]
    private PlayerUI ui;

    [Header("Gameplay")]
    public int lives, coins;
    public float currentHP, maxHP;

    public LayerMask killboxLayerMask;

    [Header("Movement")]
    public float horizontalForce;
    public float horizontalSpeed;

    public float verticalForce;
    public float airControl;
    public Transform groundPoint;
    public float groundRadius;
    public LayerMask groundLayerMask;

    public float doubleJumpDelay;

    private float xInput = 0.0f;
    private bool isGrounded;
    private bool doubleJumpAvailable;
    private bool jumpInputReleased;
    private float currentJumpMaxVelocityX;

    [Header("Combat")]
    public float attackRecovery;
    public float attackCooldown;

    public Transform attackPoint;
    public float attackRadius;
    public LayerMask enemyLayerMask;
    public float attackExecuteDelay;
    public float attackLungeForce;
    public float attackLungeForceMoving;

    private bool attackAvailable = true;
    private bool attacking = false;

    [Header("Controls")]
    [SerializeField]
    private MobileController controller;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cs = GameObject.Find("CheckpointSystem").GetComponent<CheckpointSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GroundCheck();
        CheckMoveInput();
        CheckJumpInput();
        CheckAttackInput();
        UpdateAnimProperties();
        UpdateUIProperties();
    }

    private void CheckMoveInput()
    {
        float x = Input.GetAxisRaw("Horizontal") + (controller ? controller.GetAxis(ControllerAxis.Horizontal) : 0.0f);
        
        xInput = x;
        if (x!= 0f)
        {
            if (attacking) return;
            Flip(!isGrounded);
            rb.AddForce(Vector2.right * x * horizontalForce * (isGrounded ? 1.0f : airControl));

            var clampedVelocityX = Mathf.Clamp(rb.velocity.x, -horizontalSpeed, horizontalSpeed);
            rb.velocity = new Vector2(isGrounded ? clampedVelocityX : rb.velocity.x, rb.velocity.y);
        }
    }

    private void CheckJumpInput()
    {
        var y = Input.GetAxis("Jump") + (controller ? controller.GetAxis(ControllerAxis.Jump) : 0.0f);
        if (y > 0.0f && !attacking)
        {
            if (isGrounded)
            {
                jumpInputReleased = false;
                Jump(false, 0.0f);
                StartCoroutine(EnableDoubleJump());
            }
            else if (doubleJumpAvailable && jumpInputReleased)
            {
                doubleJumpAvailable = false;
                Jump(true, 0.0f);
            }
        }
        else jumpInputReleased = true;
        if (!isGrounded && !attacking) rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -currentJumpMaxVelocityX, currentJumpMaxVelocityX), rb.velocity.y);
    }

    private void CheckAttackInput()
    {
        var attack = Input.GetAxis("Attack") + (controller ? controller.GetAxis(ControllerAxis.Attack) : 0.0f);
        if (attack > 0.0f && attackAvailable)
        {
            attacking = true;
            attackAvailable = false;
            doubleJumpAvailable = false;
            StartCoroutine(ProcessAttack());
            StartCoroutine(AttackRecovery());
            StartCoroutine(AttackCooldown());
            Flip(false);
            anim.SetTrigger("Attack");
            if (Mathf.Abs(xInput) > 0) { rb.AddForce(Vector2.right * attackLungeForceMoving * xInput, ForceMode2D.Impulse); }
            else 
            { 
                bool facingRight = transform.localScale.x > 0.0f;
                rb.AddForce((facingRight? Vector2.right : Vector2.left) * attackLungeForce, ForceMode2D.Impulse);
            }
        }
    }

    public void Jump(bool second, float launchForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0.0f);
        rb.AddForce((Vector2.up * 10 + (second && (Mathf.Abs(xInput)) > 0.0f ? (xInput * Vector2.right) : Vector3.zero)).normalized * (launchForce == 0.0f ? verticalForce : launchForce), ForceMode2D.Impulse);
        currentJumpMaxVelocityX = Mathf.Abs(rb.velocity.x) + airControl;
        Flip(false);
        anim.SetTrigger("Jump");
    }

    private void GroundCheck()
    {
        var hit = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);
        isGrounded = hit;

        var hazard = Physics2D.OverlapCircle(groundPoint.position, groundRadius, killboxLayerMask);
        if (hazard)
        {
            OnPlayerLifeLost();
        }
    }

    private void Flip(bool restrict)
    {
        if (xInput != 0.0f && !restrict)
        {
            transform.localScale = new Vector3((xInput > 0.0f ? 4.0f : -4.0f), transform.localScale.y, transform.localScale.z);
        }
    }

    private void UpdateAnimProperties()
    {
        anim.SetFloat("XVelocity", Mathf.Abs(rb.velocity.x) == 0 ? 0.0f : (Mathf.Abs(rb.velocity.x) > 0 ? 1.0f : -1.0f)); //bug fix with anim receiving strange numbers
        anim.SetBool("IsGrounded", isGrounded);
    }

    private void UpdateUIProperties()
    {
        if (ui)
        {
            ui.lives = lives;
            ui.coins = coins;
            ui.currentHP = currentHP;
            ui.maxHP = maxHP;
            ui.OnUpdated();
        }
    }

    private void OnPlayerLifeLost()
    {
        lives--;
        if (lives == 0)
        {
            Debug.Log("You have met a bitter end");
        }
        else
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        transform.position = cs.GetCurrentPlayerCheckpointPosition();
    }

    IEnumerator EnableDoubleJump()
    {
        yield return new WaitForSeconds(doubleJumpDelay);
        doubleJumpAvailable = true;
    }

    IEnumerator ProcessAttack()
    {
        yield return new WaitForSeconds(attackExecuteDelay);
        var hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayerMask);
        if (hit)
        {
            hit.gameObject.GetComponent<Enemy>().ApplyDamage(30.0f, transform.position.x);
        }
    }

    IEnumerator AttackRecovery()
    {
        yield return new WaitForSeconds(attackRecovery);
        anim.SetTrigger("AttackRecover");
        attacking = false;
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        attackAvailable = true;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
