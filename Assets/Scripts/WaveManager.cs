using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public WaveValues waveValues;
    private float spawnSpacing = 1.5f;

    void Update()
    {
        SpawnWave();
        Destroy(this);
    }

    void SpawnWave()
    {
        // enemy 1
        for (int i = 0; i < waveValues.enemy1PerWave; i++)
        {
            Vector3 spawnPosition = gameObject.transform.position + new Vector3(i * spawnSpacing, 0, 0);
            GameObject currentEnemy = Instantiate(waveValues.enemy1, spawnPosition, Quaternion.identity);
            currentEnemy.name = waveValues.enemy1.name + i;
        }

        // enemy 2
        for (int i = 0; i < waveValues.enemy2PerWave; i++)
        {
            Vector3 spawnPosition = gameObject.transform.position + new Vector3(i * spawnSpacing, 0, 0);
            GameObject currentEnemy = Instantiate(waveValues.enemy2, spawnPosition, Quaternion.identity);
            currentEnemy.name = waveValues.enemy2.name + i;
        }

    }
}
