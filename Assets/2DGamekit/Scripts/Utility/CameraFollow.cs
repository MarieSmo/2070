using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    Transform copyPlayer;
    public float height = 0f;


    private void FixedUpdate()
    {
        if(transform.position.x <= Player.position.x)
            transform.position = new Vector3(Player.position.x, height, -12);
    }
}