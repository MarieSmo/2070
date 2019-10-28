using System.Collections;
using System.Collections.Generic;
using Gamekit2D;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public Camera camera;
    public GameObject spawnLeft;
    public GameObject spawnRight;
    public Transform transformSpawnLeft;
    public Transform transformSpawnRight;
    public Transform levelIni;
    public Transform levelEnd;
    public Transform Player;
    public float SpawnDistance = 20f;

    private void Start()
    {
        checkCollision();
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
            enemies[i].GetComponent<EnemyBehavior2D>().player = GameObject.Find("Our_guy");
        }
    }

    //Modiy collision box if distance changes
    private void checkCollision()
    {
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;
        spawnLeft.GetComponent<BoxCollider2D>().size = new Vector2(2*(SpawnDistance-halfWidth/2),100);
        spawnRight.GetComponent<BoxCollider2D>().size = new Vector2(2*(SpawnDistance-halfWidth/2),100);
    }

    //Enable or disable spawns
    private void checkSpawn()
    {

        if(spawnLeft.transform.position.x <= levelIni.position.x)
        {
            spawnLeft.GetComponent<SpawnEnemies>().enabled = false;
        }
        else
        {
            spawnLeft.GetComponent<SpawnEnemies>().enabled = true;
        }

        if (spawnRight.transform.position.x >= levelEnd.position.x)
        {
            spawnRight.GetComponent<SpawnEnemies>().enabled = false;
        }
        else
        {
            spawnRight.GetComponent<SpawnEnemies>().enabled = true;
        }

    }

    //Move spawns to the desired position
    private void moveSpawns()
    {
        transformSpawnLeft.transform.position = new Vector3(camera.gameObject.transform.position.x - SpawnDistance, 2, 0);
        transformSpawnRight.transform.position = new Vector3(camera.gameObject.transform.position.x + SpawnDistance, 2, 0);
    }
}