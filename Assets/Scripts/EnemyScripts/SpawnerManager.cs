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

public enum RoundType
{
    CLEAR,
    SURVIVAL
}

public class SpawnerManager
{
    static SpawnerManager thisInstance;

    public static SpawnerManager Instance
    {
        get { return thisInstance ??= new SpawnerManager(); }
    }

    public RoundType roundType = RoundType.SURVIVAL;

    public int initialRedGiantWeight = 0;
    public int initialYellowDwarfWeight = 0;
    public int initialRedDwarfWeight = 0;

    // If numEnemiesSpawned == totalEnemies, stop spawning enemies & clear remaining enemies
    private int totalEnemies = 2; // Will be calclulated based on difficulty
    public int numEnemiesSpawned = 0;

    public int enemiesAlive = 0;
    public int enemiesKilled = 0;

    // For making the win condition based on time.  If roundTime > elapsedTime, stop spawning enemies & clear remaining enemies
    public float elapsedTime = 0;
    public int roundTime = 10;

    public float spawnInterval = 10f;

    private List<EnemySpawnData> enemyTypes; // Stores enemy spawn weights + corresponding prefab

    public int healthMult = 1;


    // Call this when you transfer the variable information from .json, but for now we're doing this in enemyspawner
    public void InitializeSpawner()
    {
        EventBus.Instance.OnEnemyDead += EnemyDead;
        EventBus.Instance.OnRoundEnd += RoundEnd;
        ResetVariables();
        CalculateDifficulty();
        UpdateUI();
    }

    // Since this is a public class and will always persist, we need to manually reset the values
    private void ResetVariables()
    {
        initialRedDwarfWeight = 0;
        initialRedGiantWeight = 0;
        initialYellowDwarfWeight = 0;
        totalEnemies = 2;
        numEnemiesSpawned = 0;
        enemiesAlive = 0;
        enemiesKilled = 0;
        elapsedTime = 0;
        roundTime = 10;
        spawnInterval = 10f;
        healthMult = 1;
        roundType = RoundType.SURVIVAL;
    }

    private void CalculateDifficulty()
    {
        healthMult = 1;
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

        enemyTypes = new List<EnemySpawnData>();

        enemyTypes.Add(new EnemySpawnData { prefab = prefabs[0], spawnWeight = initialRedDwarfWeight });
        enemyTypes.Add(new EnemySpawnData { prefab = prefabs[1], spawnWeight = initialRedGiantWeight });
        enemyTypes.Add(new EnemySpawnData { prefab = prefabs[2], spawnWeight = initialYellowDwarfWeight });
    }

    // Returns true if number of enemies spawned is less than total enemies
    // Returns true if elapsed time is 
    public bool ContinueSpawning()
    {
        switch (roundType)
        {
            case RoundType.SURVIVAL:
                return (elapsedTime < roundTime);
            case RoundType.CLEAR:
                return (numEnemiesSpawned < totalEnemies);
        }
        return false;
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
        enemiesKilled += 1;
        UpdateUI();
        if (enemiesAlive == 0)
        {
            if ((totalEnemies == numEnemiesSpawned))
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

    public void UpdateUI()
    {
        switch (roundType)
        {
            case RoundType.SURVIVAL:
                GameManager.Instance.MissionGoalUI.objective.text = "OBJECTIVE: SURVIVE";
                GameManager.Instance.MissionGoalUI.counter.text = ($"Time left: {(roundTime - elapsedTime):F2}");
                break;
            case RoundType.CLEAR:
                GameManager.Instance.MissionGoalUI.objective.text = "OBJECTIVE: CLEAR ALL ENEMIES";
                GameManager.Instance.MissionGoalUI.counter.text = ($"Enemies left: {totalEnemies - enemiesKilled}");
                break;
        };
    }



}

