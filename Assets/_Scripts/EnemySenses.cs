using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySenses : MonoBehaviour
{
    public bool playerDetected;
    public bool lineOfSight;
    public bool playerBehind;
    public LayerMask visualCollisionLayerMask;
    private Transform playerTransform;
    private float playerDirection;
    private float enemyDirection;
    private float distanceToEnemy;

    private Vector2 playerDirectionVector;

    // Start is called before the first frame update
    void Start()
    {
        playerDetected = false;
        lineOfSight= false;
        
        playerDirection = 0.0f;
        playerDirectionVector = Vector2.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerDetected && playerTransform)
        {
            distanceToEnemy = (playerTransform.position - transform.position).magnitude;
            playerDirectionVector = (playerTransform.position - transform.position).normalized;
            playerDirection = (playerDirectionVector.x > 0 ? 1.0f : -1.0f);
            enemyDirection = GetComponentInParent<Enemy>().direction.x;
            var hits = Physics2D.Linecast(transform.position, playerTransform.position + (Vector3.up), visualCollisionLayerMask);
            if (hits.transform == playerTransform)
            {
                if (playerDirection == enemyDirection)
                {
                    lineOfSight = true;
                    playerBehind = false;
                }
                else
                {
                    lineOfSight = false;
                    playerBehind = true;
                }

            }
            else
            {
                lineOfSight = false;
                playerBehind = false;
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            playerDetected = true;
            playerTransform = collision.transform;

        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == playerTransform)
        {
            playerDetected = false;
            lineOfSight = false;
            playerBehind = false;
            playerTransform = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (IsPlayerInView())
        {
            Gizmos.DrawLine(transform.position, playerTransform.position + (Vector3.up));
        }
    }

    public bool IsPlayerInView()
    {
        return (playerDetected && lineOfSight && playerTransform);
    }

    public float GetDistanceToPlayer()
    {
        if (IsPlayerInView())
        {
            return distanceToEnemy;
        }
        return -1.0f;
    }
}
