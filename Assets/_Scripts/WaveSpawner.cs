using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject minesPickup;
    [SerializeField] private GameObject healthPickup;
    [SerializeField] private GameObject stickyBombPickup;
    [SerializeField] private GameObject multiBombPickup;
    [SerializeField] private float zBound;
    [SerializeField] private float xBound;

    private int waveNo = 0;

    private void Update()
    {
        Enemy[] aliveEnemies = GameObject.FindObjectsOfType<Enemy>();

        if (aliveEnemies.Length < 1)
        {
            waveNo++;
            SpawnEnemyWave();
        }
    }

    private void SpawnEnemyWave()
    {
        for (int index = 0; index < waveNo; index++)
        {
            SpawnEnemy();
            SpawnHealth();
            SpawnStickyGernades();
            SpawnMultiBombPickup();
            SpawnMinesPickup();
        }
    }

    private void SpawnHealth()
    {
        Vector3 spawningPosition = GetRandomPosition();
        Instantiate(healthPickup, spawningPosition, healthPickup.transform.rotation);
    }

    private void SpawnStickyGernades()
    {
        Vector3 spawningPosition = GetRandomPosition();
        Instantiate(stickyBombPickup, spawningPosition, stickyBombPickup.transform.rotation);
    }

    private void SpawnMultiBombPickup()
    {
        Vector3 spawningPosition = GetRandomPosition();
        Instantiate(multiBombPickup, spawningPosition, multiBombPickup.transform.rotation);
    }

    private void SpawnMinesPickup()
    {
        Vector3 spawningPosition = GetRandomPosition();
        Instantiate(minesPickup, spawningPosition, minesPickup.transform.rotation);
    }

    private void SpawnEnemy()
    {
        Vector3 spawningPosition = GetRandomPosition();
        Instantiate(enemyPrefab, spawningPosition, enemyPrefab.transform.rotation);
    }

    private Vector3 GetRandomPosition() => new Vector3(Random.Range(-xBound, xBound), 0.3f, Random.Range(-zBound, zBound));
}
