using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public bool circular;
    public bool reverse;
    public GameObject pathParent;

    public float moveSpeed;

    private Rigidbody2D rb;
    private Vector3[] path;
    private int pathLength;

    private int indexOfNextPathPoint;
    private Vector3 distanceToNextPathPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathLength = pathParent.transform.childCount;
        path = new Vector3[pathLength];
        for (int i = 0; i < pathLength; i++)
        {
            path[i] = pathParent.transform.GetChild(i).transform.position;
        }
        indexOfNextPathPoint = 1;
    }

    private void FixedUpdate()
    {
        distanceToNextPathPoint = path[indexOfNextPathPoint] - transform.position;
        if (distanceToNextPathPoint.magnitude < (moveSpeed * Time.fixedDeltaTime))
        {
            if (reverse)
            {
                if (indexOfNextPathPoint == 0)
                {
                    indexOfNextPathPoint = circular ? pathLength - 1 : 1;
                    reverse = circular ? true : false;
                }
                else indexOfNextPathPoint--;
            }
            else 
            {
                if (indexOfNextPathPoint + 1 == pathLength)
                {
                    indexOfNextPathPoint = (circular ? 0 : indexOfNextPathPoint - 1);
                    reverse = circular ? false : true;
                }
                else indexOfNextPathPoint++;
            }
        }
        rb.velocity = distanceToNextPathPoint.normalized * moveSpeed;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.transform.SetParent(transform);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        collision.gameObject.transform.SetParent(null);
    }
}
