using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Followplatform, Player;

    void FixedUpdate()
    {
        if(Player.position.x <= 18 || Player.position.x >= 30)
            Followplatform.position = new Vector3(Player.position.x, 2.8f, 0);
        else
            Followplatform.position = new Vector3(28, 2.8f, 0);
    }
}