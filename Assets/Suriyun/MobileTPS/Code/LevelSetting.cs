using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Suriyun.MobileTPS
{
    public class LevelSetting : MonoBehaviour
    {
        public List<Transform> player_move_pos;

        public UnityEvent EventOnStart;

        protected virtual void Start()
        {
            EventOnStart.Invoke();
        }
    }
}