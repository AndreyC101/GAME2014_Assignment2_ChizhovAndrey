using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
{
    public float loadedLifetime;
    public float outOfCommisionTime;
    private float timeSinceLoaded;
    private bool isLoaded = false;

    private Rigidbody2D rb;
    private Vector2 startingPosition;
    private Quaternion startingRotation;

    private List<GameObject> inContact;


    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        inContact= new List<GameObject>();
    }

    public void FixedUpdate()
    {
        if (isLoaded)
        {
            timeSinceLoaded += Time.fixedDeltaTime;
            if (timeSinceLoaded > loadedLifetime)
            {
                Collapse();
            }
        }
    }

    private void Collapse()
    {
        SoundManager.Instance.PlayAudio(SoundID.Platform_Crumble);
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.None;
        StartCoroutine(Restore());
    }

    IEnumerator Restore()
    {
        yield return new WaitForSeconds(outOfCommisionTime);
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector2.zero;
        timeSinceLoaded = 0.0f;
        isLoaded= false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() || collision.gameObject.GetComponent<Enemy>()) { inContact.Add(collision.gameObject); isLoaded = true; }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        inContact.Remove(collision.gameObject);
        if (inContact.Count == 0) isLoaded = false;   
    }
}
