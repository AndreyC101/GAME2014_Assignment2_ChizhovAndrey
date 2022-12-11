using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySenses : MonoBehaviour
{
    public bool playerDetected;
    public bool lineOfSight;
    public LayerMask visualCollisionLayerMask;
    private Transform playerTransform;
    private float playerDirection;
    private float enemyDirection;

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
            playerDirectionVector = (playerTransform.localPosition - transform.position).normalized;
            playerDirection = (playerDirectionVector.x > 0 ? 1.0f : -1.0f);
            enemyDirection = GetComponentInParent<Enemy>().direction.x;
            var hits = Physics2D.Linecast(transform.position, playerTransform.position, visualCollisionLayerMask);
            if (hits.transform == playerTransform && playerDirection == enemyDirection)
            {
                lineOfSight = true;
            }
            else lineOfSight = false;
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
            playerTransform = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (lineOfSight)
        {
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }

        if (playerDetected)
        {
            Gizmos.DrawWireSphere(transform.position, 15.0f);
        }
    }
}
