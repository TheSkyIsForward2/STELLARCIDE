using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    private SpawnerManager spawnerManager;


    private void Start()
    {
        spawnerManager = SpawnerManager.Instance;
        spawnerManager.InitializeSpawner();
        StartCoroutine(SpawnCoroutine());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnCoroutine()
    {
        while (spawnerManager.ContinueSpawning()) {
            SpawnEnemy();

            yield return new WaitForSeconds(spawnerManager.spawnInterval);
            spawnerManager.elapsedTime += spawnerManager.spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        foreach(Transform child in transform)
        {
            if (spawnerManager.ContinueSpawning())
            {
                GameObject enemy = Instantiate(spawnerManager.GetNextEnemy(), child.transform.position, child.transform.rotation);
                enemy.transform.SetParent(child.transform);
                spawnerManager.enemiesAlive += 1;
                spawnerManager.numEnemiesSpawned += 1;
            }
        }
    }
}
