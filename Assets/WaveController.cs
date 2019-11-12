using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public GameObject waveObject;

    private void OnTriggerEnter2D(Collider2D waveObject)
    {
        print("Ha entrado!!!!!!!!!!!!!!");
        waveObject.GetComponent<SpawnEnemies>().enabled = true;
    }
}
