using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    public float bounceForce;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            if (player.GetVelocityY() < 2.0f)
            {
                SoundManager.Instance.PlayAudio(SoundID.Platform_Bounce);
                player.Jump(false, bounceForce);
            }
        }
    }
}
