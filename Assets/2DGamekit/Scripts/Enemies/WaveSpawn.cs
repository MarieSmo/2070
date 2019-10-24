using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawn : MonoBehaviour
{

    //Enums

    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    //Classes

    [System.Serializable]
    public class Wave
    {
        public int count;
        public Transform enemy;
        public string name;
        public float rate;
    }

    //Attributes

    private int nextWave = 0;
    private float searchCountdown = 1.0f;
    public Transform[] spawnPoints;
    public SpawnState state = SpawnState.COUNTING;
    public float timeBetweenWaves = 5.0f;
    private float waveCountDown = 0.0f;
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
        if(state == SpawnState.WAITING)
        {
            if(!EnemyIsAlive())
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

        if(waveCountDown <= 0)
        {
            if(state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave])); //IEnumerator -> StartCoroutine
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed!" + nextWave + " " + waves.Length);

        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if(nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("All waves complete");
        }
        else
        {
            nextWave++;
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

    IEnumerator SpawnWave(Wave wave) //IEnumerator waits until executing again
    {
        Debug.Log("Spawning wave" + wave.name);
        state = SpawnState.SPAWNING;

        for(int i = 0; i < wave.count; i++)
        {
            SpawnEnemy(wave.enemy);
            yield return new WaitForSeconds(1.0f/wave.rate);
        }

        state = SpawnState.WAITING;

        yield break; //Because of IEnumerator
    } 

    void SpawnEnemy(Transform enemy)
    {
        Debug.Log("Spawning Enemy: " + enemy.name);
        Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemy, sp.position, sp.rotation);
    }


}
