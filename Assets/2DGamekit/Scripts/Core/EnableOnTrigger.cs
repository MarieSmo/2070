using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnTrigger : MonoBehaviour
{
    public GameObject Prefab;

    private void OnTriggerEnter2D()
    {
        Prefab.SetActive(true);
    }
}
