using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Suriyun.MobileTPS
{
    public class Game : MonoBehaviour
    {

        public static Game instance;
        [HideInInspector]
        public LevelSetting level_setting;

        public bool is_pause;
        [HideInInspector]
        public Coroutine c_handle_hotkey;
        public KeyCode hotkey_pause;

        public UnityEvent EventGameStart;
        public UnityEvent EventGamePause;
        public UnityEvent EventGameResume;
        public UnityEvent EventGameRestart;
        public UnityEvent EventGameOver;

        protected virtual void Awake()
        {
            instance = this;
            Application.targetFrameRate = 60;

            if (level_setting == null)
            {
                level_setting = GameObject.FindObjectOfType<LevelSetting>();
            }
        }

        public void StartHandleHotkey()
        {
            c_handle_hotkey = StartCoroutine(HandleHotkey());
        }

        public void StopHandleHotkey()
        {
            StopCoroutine(c_handle_hotkey);
        }

        IEnumerator HandleHotkey()
        {
            while (true)
            {
                if (Input.GetKeyDown(hotkey_pause))
                {
                    if (is_pause)
                    {
                        this.GameResume();
                    }
                    else
                    {
                        this.GamePause();
                    }
                }
                yield return 0;
            }
        }

        public virtual void GameStart()
        {
            EventGameStart.Invoke();
        }

        public virtual void GamePause()
        {
            is_pause = true;
            Time.timeScale = 0;
            EventGamePause.Invoke();
        }

        public virtual void GameResume()
        {
            is_pause = false;
            Time.timeScale = 1;
            EventGameResume.Invoke();
        }

        public virtual void GameRestart()
        {
            Time.timeScale = 1;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public virtual void GameOver()
        {
            EventGameOver.Invoke();
            Debug.Log("Game Over");
        }

		public virtual void LoadLevel(string level){
            StartCoroutine(LoadLevelDelay(level, 3f));
		}

        protected IEnumerator LoadLevelDelay(string level, float sec)
        {
            yield return new WaitForSeconds(sec);
            SceneManager.LoadScene(level);
        }

    }
}