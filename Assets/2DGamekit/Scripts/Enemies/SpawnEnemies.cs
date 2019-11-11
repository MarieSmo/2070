using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    //Enums

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    //Classes

    [System.Serializable]
    public class Enemy
    {
        public Transform enemy;
        public int weight;
    }

    [System.Serializable]
    public class Wave
    {
        public int count;
        public float rate;
        public Enemy[] enemies;
    }

    //Attributes

    public int nextWave = 0;
    private float searchCountdown = 1.0f;
    public float timeBetweenWaves = 5.0f;
    private float waveCountDown = 0.0f;
    public SpawnState state = SpawnState.COUNTING;
    public Transform[] spawnPoints;
    public Wave[] waves;

    //Methods

    private void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points referenced.");
        }

        waveCountDown = timeBetweenWaves;
    }

    private void Update()
    {
        if (state == SpawnState.WAITING)
        {
            //Mark current wave as completed
            if (!EnemyIsAlive())
            {
                WaveCompleted();
                return;
            }
            else
            {
                return;
            }
        }

        if (waveCountDown <= 0)
        {
            //Start a new wave if possible
            if (state != SpawnState.SPAWNING && nextWave != waves.Length)
            {
                StartCoroutine(SpawnWave(waves[nextWave])); 
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }
    }

    //Checks if there are any enemies alive in the current level

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;

        if (searchCountdown <= 0.0f)
        {
            searchCountdown = 1.0f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    //Spawns a given enemy with randomized values

    void SpawnEnemy(Transform enemy)
    {
        //Instanciate enemy

        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Object enemyObject = Instantiate(enemy, sp.position, sp.rotation);
        enemyObject.name = enemy.name + enemyObject.GetInstanceID().ToString();
        GameObject enemyValues = GameObject.Find(enemyObject.name);

        //Modify health and damage values by adding or substracting a random value

        int randomHealth = 0;
        int randomDamage = 0;

        //Health

        if(enemyValues.GetComponent<EnemyBehavior2D>().health >= 14)
        {
            randomHealth = Mathf.RoundToInt(Random.Range(-enemyValues.GetComponent<EnemyBehavior2D>().health / 5, enemyValues.GetComponent<EnemyBehavior2D>().health / 5));
        }
        else
        {
            randomHealth = Mathf.RoundToInt(Random.Range(-enemyValues.GetComponent<EnemyBehavior2D>().health / 3, (enemyValues.GetComponent<EnemyBehavior2D>().health / 3) + 1));
        }

        //Damage

        if (enemyValues.GetComponent<EnemyBehavior2D>().health >= 7)
        {
            randomDamage = Mathf.RoundToInt(Random.Range(-enemyValues.GetComponent<EnemyBehavior2D>().damage / 3, enemyValues.GetComponent<EnemyBehavior2D>().damage / 3));
        }
        else
        {
            randomDamage = Mathf.RoundToInt(Random.Range(-enemyValues.GetComponent<EnemyBehavior2D>().damage / 2, (enemyValues.GetComponent<EnemyBehavior2D>().damage / 2) + 1));
        }

        //Modify enemy values

        enemyValues.GetComponent<EnemyBehavior2D>().health += randomHealth;
        enemyValues.GetComponent<EnemyBehavior2D>().damage += randomDamage;
    }

    //Initiates a wave and spawns enemies

    IEnumerator SpawnWave(Wave wave) 
    {
        Debug.Log("Spawning wave " + nextWave);
        state = SpawnState.SPAWNING;

        //Spawn a random enemy 

        int totalWeight = 0;

        for (int i = 0; i < wave.enemies.Length; i++)
        {
            if(wave.enemies[i].weight > 0)
                totalWeight += wave.enemies[i].weight;
        }

        int count = 0;
        while (count < wave.count)
        {
            int randomWeight = Random.Range(0, totalWeight);
            int randomEnemy = Random.Range(0, wave.enemies.Length);
            if (randomWeight < wave.enemies[randomEnemy].weight)
            {
                //Spawns enemy

                SpawnEnemy(wave.enemies[randomEnemy].enemy);

                yield return new WaitForSeconds(wave.rate);

                //Modify weight values

                wave.enemies[randomEnemy].weight -= 30 / wave.enemies.Length;

                for (int j = 0; j < wave.enemies.Length; j++)
                {
                    if (randomEnemy != j)
                    {
                        wave.enemies[j].weight += 30 / wave.enemies.Length;
                    }
                }

                //Enemy has spawned, count increases
                
                count++;
            }
        }

        state = SpawnState.WAITING;

        yield break;
    }

    //Marks current wave as completed and sets the next wave's index

    public void WaveCompleted()
    {
        Debug.Log("Wave Completed!");

        nextWave++;
        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if (nextWave >= waves.Length)
        {
            Debug.Log("All waves complete");
        }
    }

}
