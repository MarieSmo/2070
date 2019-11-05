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

    private int nextWave = 0;
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
            if (!EnemyIsAlive())
            {
                //Begin a new round
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

    void SpawnEnemy(Transform enemy)
    {
        Debug.Log("Spawning Enemy: " + enemy.name);
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, sp.position, sp.rotation);
    }

    IEnumerator SpawnWave(Wave wave) //IEnumerator waits until executing again
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
                //Spawn enemy
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

        yield break; //Because of IEnumerator
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed!" + nextWave + " " + waves.Length);

        nextWave++;
        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if (nextWave >= waves.Length)
        {
            Debug.Log("All waves complete");
        }
    }

}
