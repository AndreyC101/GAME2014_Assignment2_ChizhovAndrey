using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointID;
    private CheckpointSystem cs;
    private SpriteRenderer sr;

    [SerializeField]
    Vector3 playerSpawnPoint;

    public void Register(int ID, CheckpointSystem cs)
    {
        checkpointID = ID;
        this.cs = cs;
        sr = GetComponent<SpriteRenderer>();
        playerSpawnPoint = transform.GetChild(0).position;
    }

    public void SetAsActiveCheckpoint(bool active)
    {
        if (sr)
        {
            sr.sprite = active ? cs.ActiveCheckpointSprite : cs.InactiveCheckpointSprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<Player>();
        if (player)
        {
            cs.SetCurrentPlayerCheckpoint(checkpointID);
        }
    }

    public Vector3 GetSpawnPosition()
    {
        return playerSpawnPoint;
    }
}
