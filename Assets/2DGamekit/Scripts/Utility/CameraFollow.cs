using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera camera;
    public Transform middleWave;
    public Transform levelEnd;
    public Transform Player;
    public float height = 0f;
    Transform copyPlayer;
    bool attached;

    private void Start()
    {
        transform.position = new Vector3(7, height, -12);
        attached = true;
    }

    void TryToAttach()
    {
        attached = true;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(middleWave.position, 15);
        for (int i=0; i<enemies.Length; i++)
        {
            if (enemies[i].gameObject.tag == "Enemy")
            {
                attached = false;
            }
        }
    }

    private void FixedUpdate()
    {
        float halfHeight = camera.orthographicSize;
        float halfWidth = camera.aspect * halfHeight;
        //Follow player if he goes to the right and camera is not at the end
        if (transform.position.x <= Player.position.x 
            && (transform.position.x + halfWidth/2) <= levelEnd.position.x
            && attached)
            transform.position = new Vector3(Player.position.x, height, -12);

        //Fix camera in a wave
        if (middleWave != null && middleWave.position.x - 0.5 <= transform.position.x && middleWave.position.x + 0.5 >= transform.position.x)
            attached = false;

        if (!attached)
            TryToAttach();
    }
}