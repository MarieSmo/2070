using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGoRight : MonoBehaviour
{
    bool cameraStop;

    public float cameraScrollSpeed;

    public Vector3 startPosition;

    public float waitToScroll;


    //Usethisfor initialization
    void Start()
    {
        cameraStop = false;
        startPosition = transform.position;
    }

    //Updateiscalledonceper frame
    void Update()
    {
        if (!cameraStop)
        {
            StartCoroutine("CameraCoRoutine");
        }
        else
        {
            StopCoroutine("CameraCoRoutine");
            transform.Translate(0f, 0f, 0f);
            //if she beats all the enemies then true and starts moving the camera again ¿?
        }
    }


    public IEnumerator CameraCoRoutine()
    {
        yield return new WaitForSeconds(waitToScroll);
        transform.Translate(cameraScrollSpeed, 0f, 0f);
        yield return new WaitForSeconds(11);
        cameraStop = true;
    }
}