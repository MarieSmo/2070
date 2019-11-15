using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera camera;
    public Transform Player;
    public Transform[] waves;
    public Transform initialPositionOfCamera, endPositionOfCamera;
    public float height = 0f;
    bool attached, cameraNeedSmooth = false, smoothCompleted = false;
    float smoothSpeed = 4f;
    int currentWave;

    private void Start()
    {
        transform.position = new Vector3(initialPositionOfCamera.position.x, height, -12);
        currentWave = 0;
    }

    // This function counts the number of enemies that are in an radius of 20 from the middle of the wave screen
    // It's just temporary, when we have a way of know if the wave is completed, we can change it.
    int CountEnemiesInArea(Vector3 wavePosition)
    {
        int enemyCounter = 0;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(wavePosition, 20);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].gameObject.tag == "Enemy")
            {
                enemyCounter++;
            }
        }

        return enemyCounter;
    }

    bool WaveCompleted()
    {
        bool waveIsCompleted = false;
        if (waves[currentWave].GetComponent<SpawnEnemies>().nextWave >= waves[currentWave].GetComponent<SpawnEnemies>().waves.Length)
        {
            waveIsCompleted = true;
            Debug.Log("Wave completed: " + currentWave);
        }
        return waveIsCompleted;
    }

    bool UpdatedAttached()
    {
        bool attachCamera = false;

        // the camera won't move until the player reaches the camera initial position
        if (Player != null && Player.position.x <= initialPositionOfCamera.position.x)
        {
            attachCamera = false;
        }
        else if (Player != null && Player.position.x <= endPositionOfCamera.position.x)
        {
            float halfHeight = camera.orthographicSize;
            float halfWidth = camera.aspect * halfHeight;
            // area between the initial position of the camera and the end of the wave
            attachCamera = true;
            // area between the middle of the wave and the limit of it (this need a fix)
            if (Player != null && Player.position.x >= waves[currentWave].position.x && Player.position.x <= waves[currentWave].position.x + halfWidth/2) // change this 10 for an object or whatever thing to state the limit of the screen
            {
                attachCamera = false;
                int enemyCounter = CountEnemiesInArea(waves[currentWave].position);
                // if there are no enemies and the player is at the right of the camera
                if (WaveCompleted())
                {
                    attachCamera = true;
                    if (transform.position.x <= Player.position.x)
                    {
                        // the camera needs to move smoothly towards the player
                        cameraNeedSmooth = true;
                        smoothCompleted = false;
                    }

                    //If wave is completed set it on the WaveController script
                    GameObject.Find(waves[currentWave].name).GetComponent<WaveController>().setCompleted(true);
                    currentWave++;
                }
            }
        }
        else
        {
            attachCamera = false;

            // if there are no enemies and the player is at the right of the camera
            if (WaveCompleted())
            {
                //If wave is completed set it on the WaveController script
                GameObject.Find(waves[currentWave].name).GetComponent<WaveController>().setCompleted(true);
            }
        }

        return attachCamera;
    }


    private void FixedUpdate()
    {
        // check if the camera needs to be attached
        attached = UpdatedAttached();

        // after a wave, if the player is in the right side of the camera, it will need to move smoothly
        if (cameraNeedSmooth && !smoothCompleted)
        {
            Vector3 desiredPosition = new Vector3(Player.position.x, transform.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // the camera moves smoothly until it reaches the player position (margin of 0.3)
            if (transform.position.x >= Player.position.x - 0.3)
            {
                cameraNeedSmooth = false;
                smoothCompleted = true;
            }
        }
        // if attached the camera moves, else it stops
        else if (Player != null && transform.position.x <= Player.position.x && attached)
            transform.position = new Vector3(Player.position.x, height, -12);
    }
}

/*  Previous document
 public class CameraFollow : MonoBehaviour
{
    public Camera camera;
    public Transform Player;
    public Transform middleWave;
    public Transform initialPositionOfCamera;
    public Transform endPositionOfCamera;
    Transform copyPlayer;
    public float height = 0f;
    bool attached, cameraNeedSmooth = false, smoothCompleted = false;
    float smoothSpeed = 4f;

    private void Start()
    {
        transform.position = new Vector3(initialPositionOfCamera.position.x, height, -12);
    }

    // This function counts the number of enemies that are in an radius of 20 from the middle of the wave screen
    // It's just temporary, when we have a way of know if the wave is completed, we can change it.
    int CountEnemiesInArea(Vector3 wavePosition)
    {
        int enemyCounter = 0;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(wavePosition, 20);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].gameObject.tag == "Enemy")
            {
                enemyCounter++;
            }
        }

        return enemyCounter;
    }


    bool UpdatedAttached()
    {
        bool attachCamera = false;

        // the camera won't move until the player reaches the camera initial position
        if (Player != null && Player.position.x <= initialPositionOfCamera.position.x)
        {
            attachCamera = false;
        }
        else if (Player != null && Player.position.x <= endPositionOfCamera.position.x)
        {
            float halfHeight = camera.orthographicSize;
            float halfWidth = camera.aspect * halfHeight;
            // area between the initial position of the camera and the end of the wave
            attachCamera = true;
            // area between the middle of the wave and the limit of it (this need a fix)
            if (Player != null && Player.position.x >= middleWave.position.x && Player.position.x <= middleWave.position.x + halfWidth/2) // change this 10 for an object or whatever thing to state the limit of the screen
            {
                attachCamera = false;
                int enemyCounter = CountEnemiesInArea(middleWave.position);
                // if there are no enemies and the player is at the right of the camera
                if (enemyCounter == 0 && transform.position.x <= Player.position.x)
                {
                    // the camera needs to move smoothly towards the player
                    cameraNeedSmooth = true;
                    attachCamera = true;
                    //disable camera collision
                }
                else
                {
                    // enable camera collision
                }
            }
        }
        else
        {
            attachCamera = false;
        }

        return attachCamera;
    }


    private void FixedUpdate()
    {
        // check if the camera needs to be attached
        attached = UpdatedAttached();

        // after a wave, if the player is in the right side of the camera, it will need to move smoothly
        if (cameraNeedSmooth && !smoothCompleted)
        {
            Vector3 desiredPosition = new Vector3(Player.position.x, transform.position.y, transform.position.z);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            // the camera moves smoothly until it reaches the player position (margin of 0.3)
            if (transform.position.x >= Player.position.x - 0.3)
            {
                cameraNeedSmooth = false;
                smoothCompleted = true;
            }
        }
        // if attached the camera moves, else it stops
        else if (Player != null && transform.position.x <= Player.position.x && attached)
            transform.position = new Vector3(Player.position.x, height, -12);
    }
}
*/
