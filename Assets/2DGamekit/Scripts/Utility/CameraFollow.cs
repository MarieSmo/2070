using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    public float Offset;
    Transform copy_Player;

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
        transform.position = new Vector3(Player.position.x, 2.8f, -12+Offset);
        copy_Player.SetPositionAndRotation(new Vector3(Player.position.x, 2.8f, 0), new Quaternion(0, 0, 0, 0));

        transform.LookAt(copy_Player);
    }
}