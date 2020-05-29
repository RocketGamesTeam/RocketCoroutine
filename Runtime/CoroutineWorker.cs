using System.Collections.Generic;
using UnityEngine;

namespace RocketCoroutine
{
    public class CoroutineWorker : MonoBehaviour
    {
        public List<string> ActiveCoroutines = new List<string>();

        protected void Awake()
        {
            CoroutineController.CoroutineStarted += OnCoroutineStarted;
            CoroutineController.CoroutineFinished += OnCoroutineFinished;
        }

        private void OnCoroutineStarted(RocCoroutine coroutine)
        {
            ActiveCoroutines.Add(coroutine.Key);
        }

        private void OnCoroutineFinished(RocCoroutine coroutine, bool isStopped)
        {
            if (ActiveCoroutines.Contains(coroutine.Key))
            {
                ActiveCoroutines.Remove(coroutine.Key);
            }
        }

        #region Singleton
        private static CoroutineWorker _instance;

        public static CoroutineWorker Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("_CoroutineWorker");
                    _instance = go.AddComponent<CoroutineWorker>();
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
        #endregion
    }
}
