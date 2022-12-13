using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.transform.GetComponent<Player>();
        if (player)
        {
            player.CollectCoin();
            //play sfx
            Destroy(this.gameObject);
        }
    }
}
