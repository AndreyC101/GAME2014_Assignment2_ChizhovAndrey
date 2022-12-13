using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.transform.GetComponent<Player>();
        if (player)
        {
            player.Heal(5.0f);
            //play sfx
            Destroy(this.gameObject);
        }
    }
}
