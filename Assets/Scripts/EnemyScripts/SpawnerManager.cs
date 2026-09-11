using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject prefab;
    public int spawnWeight;
}

public class SpawnerManager
{
    static SpawnerManager thisInstance;

    public static SpawnerManager Instance
    {
        get { return thisInstance ??= new SpawnerManager(); }
    }

    public int initialRedGiantWeight;
    public int initialYellowDwarfWeight;
    public int initialRedDwarfWeight;

    // If numEnemiesSpawned == totalEnemies, stop spawning enemies & clear remaining enemies
    private int totalEnemies = 2; // Will be calclulated based on difficulty
    public int numEnemiesSpawned = 0;

    public int enemiesAlive = 0;

    // For making the win condition based on time.  If roundTime > elapsedTime, stop spawning enemies & clear remaining enemies
    public float elapsedTime = 0;
    public int roundTime = 100;

    public float spawnInterval = 10f;

    private List<EnemySpawnData> enemyTypes; // Stores enemy spawn weights + corresponding prefab


    // Call this when you transfer the variable information from .json, but for now we're doing this in enemyspawner
    public void InitializeSpawner()
    {
        EventBus.Instance.OnEnemyDead += EnemyDead;
        EventBus.Instance.OnRoundEnd += RoundEnd;
        CalculateDifficulty();
    }

    private void CalculateDifficulty()
    {
        totalEnemies = 1;
        numEnemiesSpawned = 0;
        enemiesAlive = 0;
        elapsedTime = 0;
        roundTime = 60;
        spawnInterval = 10f;
        int healthMult = 10;
        if (GameManager.Instance.difficultySum >= 7)
        {
            initialRedDwarfWeight += (initialRedDwarfWeight == 0) ? 0 : 10;
            initialRedGiantWeight += (initialRedGiantWeight == 0) ? 0 : 10;
            initialYellowDwarfWeight += (initialYellowDwarfWeight == 0) ? 0 : 10;
        }
        if (GameManager.Instance.difficultySum >= 14)
        {
            spawnInterval = 8f;
        }
        if (GameManager.Instance.difficultySum >= 21)
        {
            healthMult = 2;
        }
        if (GameManager.Instance.difficultySum >= 28)
        {
            initialYellowDwarfWeight += 10;
        }
        // Need to add what happens if the number goes over 28
        // Very hardcoded.... ;-;
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy Prefabs");

        prefabs[0].GetComponent<EnemyHealth>().maxHealth *= healthMult;
        prefabs[2].GetComponent<EnemyHealth>().maxHealth *= healthMult;

        enemyTypes = new List<EnemySpawnData>();

        enemyTypes.Add(new EnemySpawnData { prefab = prefabs[0], spawnWeight = initialRedDwarfWeight });
        enemyTypes.Add(new EnemySpawnData { prefab = prefabs[1], spawnWeight = initialRedGiantWeight });
        enemyTypes.Add(new EnemySpawnData { prefab = prefabs[2], spawnWeight = initialYellowDwarfWeight });
    }

    // Returns true if number of enemies spawned is less than total enemies
    // Returns true if elapsed time is 
    public bool ContinueSpawning()
    {
        return (numEnemiesSpawned < totalEnemies) && (elapsedTime < roundTime);
    }
    

    public GameObject GetNextEnemy()
    {
        return enemyTypes[GetRandomWeightedIndex()].prefab;
    }

    private int GetRandomWeightedIndex()
    {
        int totalWeight = enemyTypes.Sum(enemy => enemy.spawnWeight);

        int randomValue = Random.Range(0, totalWeight);

        for (int i = 0; i < enemyTypes.Count; i++)
        {
            randomValue -= enemyTypes[i].spawnWeight;

            if (randomValue < 0)
            {
                return i;
            }
        }

        return 0;
    }

    // "Signals"
    private void EnemyDead()
    {
        Debug.Log("we killed something right?");
        enemiesAlive -= 1;
        if (enemiesAlive == 0)
        {
            if ((totalEnemies == numEnemiesSpawned) || (elapsedTime >= roundTime))
            {
                Debug.Log("all enemies are dead and the conditions for winning are satisfied");
                EventBus.Instance.RoundEnd();
            }
        }
    }

    private void RoundEnd()
    {
        EventBus.Instance.OnEnemyDead -= EnemyDead;
        EventBus.Instance.OnRoundEnd -= RoundEnd;
        Debug.Log("beep beep");
        SceneManager.LoadScene("Scenes/UpgradeSelectorTesting");
    }



}

