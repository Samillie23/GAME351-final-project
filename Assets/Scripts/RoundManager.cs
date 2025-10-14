using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public GameObject spawner;
    public GameObject[] spawnLocation;
    public WaveValues[] waveValues;

    void Awake()
    {
        if (spawnLocation.Length != waveValues.Length)
        {
            Debug.Log(this.name + "is missing wave values and/or spawn locations");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        SpawnSpawner();
        Destroy(this);
    }

    void SpawnSpawner()
    {
        for (int i = 0; i < spawnLocation.Length; i++)
        {
            GameObject spawner1 = Instantiate(spawner, spawnLocation[i].transform.position, Quaternion.identity);
            spawner1.GetComponent<WaveManager>().waveValues = waveValues[i];
        }
    }
}
