using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public Sprite InactiveCheckpointSprite;
    public Sprite ActiveCheckpointSprite;

    private int currentPlayerCheckpointID;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();

    public void InitCheckpoints()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Checkpoint checkpoint = transform.GetChild(i).GetComponent<Checkpoint>();
            if (checkpoint)
            {
                checkpoints.Add(checkpoint);
                checkpoint.Register(i, this);
            }
        }
        SetCurrentPlayerCheckpoint(0);
    }

    public void SetCurrentPlayerCheckpoint(int newID)
    {
        currentPlayerCheckpointID = newID;
        for (int i = 0; i < checkpoints.Count; i++)
        {
            checkpoints[i].SetAsActiveCheckpoint((currentPlayerCheckpointID == i));
        }
    }

    public Vector3 GetCurrentPlayerCheckpointPosition()
    {
        return checkpoints[currentPlayerCheckpointID].GetSpawnPosition();
    }
}
