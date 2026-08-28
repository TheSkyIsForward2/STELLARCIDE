using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using System.Linq;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject prefab;
    public int spawnWeight;
}

public class SpawnerManager : MonoBehaviour
{
    // Number of enemies that will be spawned
    private int totalEnemies = 10;
    private int numEnemiesSpawned = 0;

    private int enemiesAlive = 0;
    private float elapsedTime = 0;
    private int roundTime = 10; // How much time needs to pass to win (secondary win condition)

    private List<EnemySpawnData> enemyTypes;

    //private float spawnTimer = 0f;
    private float spawnInterval = 3f; // Spawn an enemy every x seconds

    private int maxSpawnPerPoint = 1; // How many enemies can be spawned on a spawner at a time

    void Awake()
    {
        GameManager.Instance.SpawnerManager = this;
    }

    private void Start()
    {
        LoadInfoFromJSON();
        StartCoroutine(StartSpawning());
        EventBus.Instance.OnEnemyDead += EnemyDead;
    }

    private void OnDestroy()
    {
        EventBus.Instance.OnEnemyDead -= EnemyDead;
    }

    // Coroutine to start spawning enemies
    IEnumerator StartSpawning()
    {
        SpawnEnemies();
        yield return new WaitForSeconds(spawnInterval);
        elapsedTime += spawnInterval; // Lazy way of increasing time

        if ((numEnemiesSpawned >= totalEnemies) || (elapsedTime >= roundTime))
        {
            yield break;
        }

        StartCoroutine(StartSpawning());

    }

    // This will load in the proper spawning information from the JSON (sets all of the variables above)
    void LoadInfoFromJSON()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/Enemy Prefabs");

        enemyTypes = new List<EnemySpawnData>();

        foreach (GameObject prefab in prefabs)
        {
            // Spawn weights will be read from JSON
            enemyTypes.Add(new EnemySpawnData 
            { prefab= prefab, spawnWeight= 1 
            });
        }
    }
    // This will call the spawn enemy function on every spawner
    private void SpawnEnemies()
    {
        foreach (Transform child in transform)
        {
            if (child.transform.childCount <= maxSpawnPerPoint)
            {
                GameObject selectedEnemy = enemyTypes[GetRandomWeightedIndex()].prefab; // Need to apply some weight randomness here
                GameObject enemy = Instantiate(selectedEnemy, child.transform.position, child.transform.rotation);
                enemy.transform.SetParent(child.transform);
                numEnemiesSpawned += 1;
            }
            if (numEnemiesSpawned >= totalEnemies)
            {
                break;
            }
            
        }
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

}
