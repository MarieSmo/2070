using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnTrigger : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject SpawnPoint;

    private void OnTriggerEnter2D()
    {
        Prefab.SetActive(true);
        SpawnPoint.SetActive(false);
    }
}
