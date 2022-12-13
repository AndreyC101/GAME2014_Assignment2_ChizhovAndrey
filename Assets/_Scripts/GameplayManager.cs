using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject coinPrefab;
    public GameObject healthPickupPrefab;

    public GameObject enemiesParent;
    public int enemiesCount;
    public List<Vector2> enemydefaultSpawnPositions;

    public GameObject coinsParent;
    public int coinsCount;
    public List<Vector2> coinSpawnPositions;

    public GameObject healthPickupsParent;
    public int healthPickupsCount;
    public List<Vector2> healthPickupsSpawnPositions;

    public Player player;
    public CheckpointSystem cs;

    private void Awake()
    {
        enemydefaultSpawnPositions = new List<Vector2>();
        coinSpawnPositions = new List<Vector2>();
        healthPickupsSpawnPositions= new List<Vector2>();
        InitializeSpawnPositions();
        cs.InitCheckpoints();
        cs.SetCurrentPlayerCheckpoint(0);
    }

    private void InitializeSpawnPositions()
    {
        enemiesCount = enemiesParent.transform.childCount;
        for (int i = 0; i < enemiesCount; i++)
        {
            enemydefaultSpawnPositions.Add(enemiesParent.transform.GetChild(i).position);
        }
        coinsCount = coinsParent.transform.childCount;
        for (int i = 0; i < coinsCount; i++)
        {
            coinSpawnPositions.Add(coinsParent.transform.GetChild(i).position);
        }
        healthPickupsCount = healthPickupsParent.transform.childCount;
        for (int i = 0; i < healthPickupsCount; i++)
        {
            healthPickupsSpawnPositions.Add(healthPickupsParent.transform.GetChild(i).position);
        }
    }

    private void ClearLevel()
    {
        for (int i = 0; i < enemiesParent.transform.childCount; i++) 
        {
            Destroy(enemiesParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < coinsParent.transform.childCount; i++)
        {
            Destroy(coinsParent.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < healthPickupsParent.transform.childCount; i++)
        {
            Destroy(healthPickupsParent.transform.GetChild(i).gameObject);
        }
    }

    private void RespawnObjects()
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, enemydefaultSpawnPositions[i], Quaternion.identity);
            newEnemy.transform.parent = enemiesParent.transform;
        }
        for (int i = 0; i < coinsCount; i++)
        {
            GameObject newCoin = Instantiate(coinPrefab, coinSpawnPositions[i], Quaternion.identity);
            newCoin.transform.parent = coinsParent.transform;
        }
        for (int i = 0; i < healthPickupsCount; i++)
        {
            GameObject newHealthDrop = Instantiate(healthPickupPrefab, healthPickupsSpawnPositions[i], Quaternion.identity);
            newHealthDrop.transform.parent = healthPickupsParent.transform;
        }
    }

    public void ResetLevel()
    {
        ClearLevel();
        RespawnObjects();
        cs.SetCurrentPlayerCheckpoint(0);
        player.ResetPlayer();
    }
}
