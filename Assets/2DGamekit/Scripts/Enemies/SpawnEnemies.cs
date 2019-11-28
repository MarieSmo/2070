using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public bool waveZone = false;
    public float timeBetweenWaves = 2.0f;
    public int nextWave = 0;
    public SpawnState state = SpawnState.COUNTING;
    public Transform[] spawnPoints;
    public Wave[] waves;
    private bool arrowToRight = true;
    private bool canFade = false;
    private bool enteredZone = false;
    private bool moveArrow = false;
    private float searchCountdown = 1.0f;
    private float waveCountDown = 0.0f;
    private float arrowIni, arrowEnd;
    private GameObject waveNumber;
    private GameObject waveText;
    private GameObject wavesArrow;
    private GameObject wavesCleared;

    //Methods

    private void Start()
    {
        waveNumber = GameObject.Find("WaveNumber");
        waveText = GameObject.Find("WaveText");
        wavesArrow = GameObject.Find("WavesArrow");
        wavesCleared = GameObject.Find("WavesCleared");

        arrowIni = wavesArrow.transform.position.x;
        arrowEnd = arrowIni + 50;

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
                StartCoroutine(WaveCompleted());
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
                //Start a wave zone if needed

                if (waveZone && !enteredZone)
                {
                    enteredZone = true;
                    waveNumber.GetComponent<CanvasRenderer>().SetAlpha(0f);
                    waveNumber.GetComponent<Text>().text = nextWave+"/"+waves.Length;
                    waveNumber.GetComponent<Text>().CrossFadeAlpha(1f, 1f, false);
                    waveText.GetComponent<Text>().text = "Entered wave zone";
                    StartCoroutine(FadeUI(waveText));
                }

                StartCoroutine(SpawnWave(waves[nextWave])); 
            }
        }
        else
        {
            waveCountDown -= Time.deltaTime;
        }

        //Move UI arrow if needed
        
        if (moveArrow)
        {
            if (arrowToRight && wavesArrow.transform.position.x >= arrowEnd)
            {
                arrowToRight = false;
            }
            else if (!arrowToRight && wavesArrow.transform.position.x <= arrowIni)
            {
                arrowToRight = true;
            }

            if (arrowToRight)
                wavesArrow.transform.position = new Vector3(wavesArrow.transform.position.x + 1f, wavesArrow.transform.position.y, wavesArrow.transform.position.z);
            else
                wavesArrow.transform.position = new Vector3(wavesArrow.transform.position.x - 1f, wavesArrow.transform.position.y, wavesArrow.transform.position.z);
        }

        if ((Input.GetKey("right") || Input.GetKey("d")) && canFade && wavesArrow.transform.position.x == arrowIni)
        {
            wavesCleared.GetComponent<Text>().CrossFadeAlpha(0f, 1f, false);
            wavesArrow.GetComponent<Text>().CrossFadeAlpha(0f, 1f, false);
            waveNumber.GetComponent<Text>().CrossFadeAlpha(0f, 1f, false);
            moveArrow = false;
            canFade = false;
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

    public IEnumerator WaveCompleted()
    {
        nextWave++;
        state = SpawnState.COUNTING;
        waveCountDown = timeBetweenWaves;

        if (waveZone)
        {
            waveNumber.GetComponent<Text>().text = nextWave + "/" + waves.Length;

            if (nextWave >= waves.Length)
            {
                waveText.GetComponent<Text>().text = "";
                wavesArrow.GetComponent<CanvasRenderer>().SetAlpha(0f);
                wavesCleared.GetComponent<CanvasRenderer>().SetAlpha(0f);
                wavesArrow.GetComponent<Text>().enabled = true;
                wavesCleared.GetComponent<Text>().enabled = true;
                wavesArrow.GetComponent<Text>().CrossFadeAlpha(1f, 1f, false);
                wavesCleared.GetComponent<Text>().CrossFadeAlpha(1f, 1f, false);
                yield return new WaitForSeconds(1f);
                moveArrow = true;
                canFade = true;
                yield return null;
            }
            else
            {
                waveText.GetComponent<CanvasRenderer>().SetAlpha(0f);
                waveText.GetComponent<Text>().text = "Wave cleared";
                StartCoroutine(FadeUI(waveText));
                yield return null;
            }
        }
    }

    //Fades UI in and out

    private IEnumerator FadeUI(GameObject obj)
    {
        obj.GetComponent<CanvasRenderer>().SetAlpha(0f);
        obj.GetComponent<Text>().CrossFadeAlpha(1f, 1f, false);
        yield return new WaitForSeconds(2f);
        obj.GetComponent<Text>().CrossFadeAlpha(0f, 1f, false);
        yield return null;
    }

}
