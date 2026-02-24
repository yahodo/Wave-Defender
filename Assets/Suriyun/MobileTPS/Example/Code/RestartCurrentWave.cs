using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Suriyun.MobileTPS;

public class RestartCurrentWave : MonoBehaviour
{
    public MonsterWave current_wave;

    public void SetCurrentWave(MonsterWave wave_spawner)
    {
        current_wave = wave_spawner;
    }

    public void Do()
    {
        current_wave.RestartWave();
    }
}
