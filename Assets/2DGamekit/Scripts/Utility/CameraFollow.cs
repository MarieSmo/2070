using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    public Transform middleWave;
    Transform copyPlayer;
    public float height = 0f;
    bool attached;

    private void Start()
    {
        transform.position = new Vector3(7, height, -12);
        attached = true;
    }

    void TryToAttached()
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
        if(transform.position.x <= Player.position.x && attached)
            transform.position = new Vector3(Player.position.x, height, -12);

        if (middleWave.position.x - 0.5 <= transform.position.x && middleWave.position.x + 0.5 >= transform.position.x)
            attached = false;

        if (!attached)
            TryToAttached();
    }
}