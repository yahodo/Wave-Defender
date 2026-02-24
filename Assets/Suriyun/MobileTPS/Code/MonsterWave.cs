using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Suriyun.MobileTPS
{

    public class MonsterWave : MonoBehaviour
    {

        public WaveType wave_type = WaveType.MainWave;
        public GameObject mob_prefab;
        public int total_mobs_to_release;
        public float delay_before_spawn;
        public float spawn_interval = 1f;
        public List<Transform> mob_spawn_point;
        public bool is_spawning = false;

        public List<MonsterWave> sub_waves;

        [HideInInspector]
        public List<Enemy> mobs;
        [HideInInspector]
        protected int mobs_released;
        [HideInInspector]
        protected Coroutine c_spawner;

        public virtual void StartWave()
        {
            for (int i = 0; i < sub_waves.Count; i++)
            {
                if (sub_waves[i] == null)
                {
                    sub_waves.RemoveAt(i);
                    i--;
                }
            }

            foreach (MonsterWave sw in sub_waves)
            {
                sw.wave_type = WaveType.SubWave;
                sw.StartWave();
            }
            this.sub_waves.Add(this);
            c_spawner = StartCoroutine(Spawner());
            this.EventWaveStart.Invoke();
        }

        public virtual void StopWave()
        {
            this.sub_waves.Remove(this);
            this.mobs_released = 0;
            this.is_spawning = false;

            if (c_spawner != null)
            {
                this.StopCoroutine(c_spawner);
                c_spawner = null;
            }

            foreach (MonsterWave sw in sub_waves)
            {
                if (sw != this)
                {
                    sw.StopWave();
                }
            }
            this.EventWaveStop.Invoke();
        }

        public virtual void RestartWave()
        {
            foreach (MonsterWave w in sub_waves)
            {
                for (int i = 0; i < w.mobs.Count; i++)
                {
                    if (w.mobs[i] != null)
                    {
                        //Destroy(w.mobs[i].gameObject);
                        w.mobs[i].ForceDie();
                    }
                    w.mobs.RemoveAt(i);
                    i--;
                }
            }

            this.StopWave();
            this.StartWave();
        }

        protected virtual IEnumerator Spawner()
        {
            yield return new WaitForSeconds(delay_before_spawn);
            this.EventDelayEnded.Invoke();
            this.is_spawning = true;
            while (is_spawning && mobs_released < total_mobs_to_release)
            {
                int rand = UnityEngine.Random.Range(0, this.mob_spawn_point.Count - 1);
                GameObject g = (GameObject)Instantiate(mob_prefab);
                g.transform.parent = null;
                g.transform.position = this.mob_spawn_point[rand].position;

                mobs.Add(g.GetComponent<Enemy>());
                mobs_released += 1;

                yield return new WaitForSeconds(spawn_interval);
            }
            bool wave_completed = false;
            while (!wave_completed)
            {
                bool enemy_left = false;
                foreach (MonsterWave w in sub_waves)
                {
                    if (w.mobs_released >= w.total_mobs_to_release)
                    {
                        foreach (Enemy e in w.mobs)
                        {
                            if (e.hp > 0)
                            {
                                enemy_left = true;
                            }
                        }
                    }
                    else
                    {
                        enemy_left = true;
                    }
                }
                wave_completed = !enemy_left;
                this.is_spawning = false;
                yield return 0;
            }
            float complete_delay = 3f;
            while(complete_delay > 0)
            {
                complete_delay -= Time.deltaTime;
            }
            EventWaveCompleted.Invoke();
        }

        public enum WaveType
        {
            MainWave,
            SubWave
        }

        public UnityEvent EventWaveStart;
        public UnityEvent EventDelayEnded;
        public UnityEvent EventWaveStop;
        public UnityEvent EventWaveCompleted;

    }
}