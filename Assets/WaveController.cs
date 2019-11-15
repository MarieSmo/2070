using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    private GameObject character;
    private GameObject spawnLeft;
    private GameObject spawnRight;
    public GameObject waveObject;

    private bool completed = false;
    public bool finalWave = false;

    public void Start()
    {
        spawnLeft = GameObject.Find("SpawnLeft");
        spawnRight = GameObject.Find("SpawnRight");
        character = GameObject.Find("Our_guy");
}

    public void Update()
    {
        //Disable spawns and enable wave
        if (waveObject.transform.position.x <= character.transform.position.x && !completed)
        {
            GameObject.Find("MainCamera").GetComponent<SpawnController>().setInWave(true);
            waveObject.GetComponent<SpawnEnemies>().enabled = true;
        }

        //If wave is completed enable spawns unless it is the final wave
        if(completed && finalWave)
        {
            GameObject.Find("MainCamera").GetComponent<SpawnController>().setInWave(true);
        }

    }

    public bool levelEnded()
    {
        if (finalWave && completed)
            return true;
        else
            return false;
    }

    public void setCompleted(bool val)
    {
        print(this.name);
        completed = val;
        if(val==true)
        {
            GameObject.Find("MainCamera").GetComponent<SpawnController>().setInWave(false);
            if(finalWave)
            {
                spawnRight.SetActive(false);
            }
        }
    }

}
