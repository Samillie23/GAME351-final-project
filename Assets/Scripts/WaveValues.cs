using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "WaveValues")]
public class WaveValues : ScriptableObject
{
    public GameObject enemy1;
    public GameObject enemy2;
    public int enemy1PerWave;
    public int enemy2PerWave;
}
