using System.Collections;
using System.Collections.Generic;
using Gamekit2D;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public GameObject spawnLeft;
    public GameObject spawnRight;
    public Transform transformSpawnLeft;
    public Transform transformSpawnRight;
    public Transform levelIni;
    public Transform levelEnd;
    public Transform Player;
    public float DistanceToPlayer = 12f;

    private void Start()
    {
        checkSpawn();
        moveSpawns();
    }

    // Update is called once per frame
    void Update()
    {
        moveSpawns();
        checkSpawn();

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.parent = null;
            //enemies[i].GetComponent<EnemyBehavior2D>().player = GameObject.Find("Our_guy");
        }
    }

    private void checkSpawn()
    {
        if(spawnLeft.transform.position.x <= levelIni.position.x)
        {
            spawnLeft.GetComponent<EnemySpawner>().enabled = false;
        }
        else
        {
            spawnLeft.GetComponent<EnemySpawner>().enabled = true;
        }

        if (spawnRight.transform.position.x >= levelEnd.position.x)
        {
            spawnRight.SetActive(false);
        }
        else
        {
            spawnRight.SetActive(true);
        }
    }

    private void moveSpawns()
    {
        transformSpawnLeft.transform.position = new Vector3(Player.position.x - DistanceToPlayer, 2, 0);
        transformSpawnRight.transform.position = new Vector3(Player.position.x + DistanceToPlayer, 2, 0);
    }
}