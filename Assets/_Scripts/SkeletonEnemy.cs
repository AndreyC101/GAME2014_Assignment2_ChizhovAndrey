using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkeletonEnemy : Enemy
{
    [Header("Combat")]
    public float attackRange;
    public float attackChargeTime;
    public float attackRadius;
    public float attackRecoveryTime;
    public LayerMask playerLayerMask;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (sense.IsPlayerInView() && IsActionReady() && attackAvailable)
        {
            float distanceToPlayer = sense.GetDistanceToPlayer();
            if (distanceToPlayer != -1.0f && distanceToPlayer <= attackRange)
            {
                Debug.Log("Enemy begin attack");
                anim.SetTrigger("Attack");
                isMoving = false;
                isAttacking = true;
                attackAvailable = false;
                StartCoroutine(Attack());
            }
        }
    }

    protected override void UpdateAnimationMovementProperties()
    {
        base.UpdateAnimationMovementProperties();
    }

    public override void ApplyDamage(float damage, float playerXposition)
    {
        base.ApplyDamage(damage, playerXposition);
    }

    private void ProcessAttack()
    {
        var playerHit = Physics2D.OverlapCircle(forwardPoint.position, attackRadius, playerLayerMask);
        if (playerHit)
        {
            Player hitTarget = playerHit.transform.GetComponent<Player>();
            if (hitTarget)
            {
                hitTarget.ApplyDamage(20.0f, transform.position.x);
            }
        }
        Debug.Log("Attack logic processed, starting recovery");
        StartCoroutine(AttackRecovery());
    }

    private void AttackRecover()
    {
        Debug.Log("Recovered from Attack");
        isAttacking = false;
        attackAvailable = true;
        isMoving = true;
        anim.SetTrigger("AttackRecover");
        if (sense.playerBehind) Flip();
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackChargeTime);
        ProcessAttack();
    }

    IEnumerator AttackRecovery()
    {
        yield return new WaitForSeconds(attackRecoveryTime);
        AttackRecover();
    }
}
