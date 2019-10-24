using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    Transform copyPlayer;
    public float height = 0f;

    /*float Stop1, Stop2;

     private void Start()
      {
          Stop1 = 18;
          Stop2 = 30;
      }

      void FixedUpdate()
      {
          if(Player.position.x <= Stop1 || Player.position.x >= Stop2)
              Followplatform.position = new Vector3(Player.position.x, 2.8f, 0);
          else
              Followplatform.position = new Vector3(28, 2.8f, 0);
      }*/

    private void FixedUpdate()
    {
        transform.position = new Vector3(Player.position.x, height, -12);
        //copyPlayer.SetPositionAndRotation(new Vector3(Player.position.x, height, 0), new Quaternion(0, 0, 0, 0));
        //transform.LookAt(copyPlayer);
    }
}