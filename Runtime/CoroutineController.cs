using System;
using System.Collections;
using System.Collections.Generic;
using RocketLog;
using UnityEngine;

namespace RocketCoroutine
{
    public static class CoroutineController
    {
        public static event Action<RocCoroutine> CoroutineStarted;
        public static event Action<RocCoroutine, bool> CoroutineFinished;

        private static readonly Dictionary<string, RocCoroutine> Coroutines = new Dictionary<string, RocCoroutine>();

        private static readonly RocLog Log = new RocLog(typeof(CoroutineController).Name, DebugLevels.Warning);

        /// <summary>
        /// Start Coroutine [Single Instance]
        /// Use with a key if you require multiple instances of coroutine at the same time
        /// </summary>
        /// <param name="iEnumerator">Coroutine</param>
        /// <param name="overrideIfExists">Stop first if same coroutine is already working</param>
        /// <param name="onFinished"></param>
        public static void StartCoroutine(IEnumerator iEnumerator, bool overrideIfExists = false, Action<bool> onFinished = null)
        {
            StartCoroutine(iEnumerator, null, overrideIfExists, onFinished);
        }

        /// <summary>
        /// Start Coroutine [Multiple Instance]
        /// !! Has to be stopped with the key !!
        /// </summary>
        /// <param name="iEnumerator">Coroutine</param>
        /// <param name="key">Unique key</param>
        /// <param name="overrideIfExists">Stop first if coroutine with same key is already working</param>
        /// <param name="onFinished"></param>
        public static void StartCoroutine(IEnumerator iEnumerator, string key, bool overrideIfExists = false, Action<bool> onFinished = null)
        {
            if (key == null)
            {
	            key = GenerateKey(iEnumerator);
            }
            
            if (Coroutines.ContainsKey(key))
            {
                if (!overrideIfExists)
                {
                    Log.Warning(string.Format("Coroutine ALREADY exists! Key: {0}\nHint: You can override it or if you want multiple instances then give this coroutine a key!", key));
                    return;
                }

                Log.Info(string.Format("Coroutine already exists! It will be OVERRIDEN! Key: {0}", key));
                StopCoroutine(key);
                StartCoroutine(key, iEnumerator, onFinished);
                return;
            }

            StartCoroutine(key, iEnumerator, onFinished);
        }

        private static void StartCoroutine(string key, IEnumerator iEnumerator, Action<bool> onFinished = null)
        {
            Log.Debug("Coroutine will be started. Key: " + key);

            RocCoroutine coroutine = new RocCoroutine(key, iEnumerator, onFinished);
            coroutine.Finished += OnCoroutineFinished;
            coroutine.Start();

            Coroutines.Add(key, coroutine);

            if (CoroutineStarted != null) CoroutineStarted(coroutine);
        }

        private static void OnCoroutineFinished(RocCoroutine rocCoroutine, bool isStopped)
        {
            rocCoroutine.Finished -= OnCoroutineFinished;

            if (isStopped) return;

            //Coroutine died by itself!

            if (!Coroutines.ContainsKey(rocCoroutine.Key))
            {
                Log.Warning("Coroutine does NOT exist! Key: " + rocCoroutine.Key);
                return;
            }
	        
            Coroutines.Remove(rocCoroutine.Key);

            if (CoroutineFinished != null) CoroutineFinished(rocCoroutine, false);
        }

        /// <summary>
        /// Stops coroutine started without a key
        /// </summary>
        public static void StopCoroutine(IEnumerator iEnumerator)
        {
            if (iEnumerator != null)
            {
                StopCoroutine(GenerateKey(iEnumerator));
            }
        }

        /// <summary>
        /// Stops coroutine started with a key
        /// </summary>
        public static void StopCoroutine(string key)
        {
            if (!Coroutines.ContainsKey(key))
            {
                Log.Warning("Coroutine does NOT exist! Key: " + key);
                return;
            }

            RocCoroutine targetCoroutine = Coroutines[key];
            targetCoroutine.Stop();

            Coroutines.Remove(key);

            if (CoroutineFinished != null) CoroutineFinished(targetCoroutine, true);

            Log.Debug("Coroutine is STOPPED! Key: " + key);
        }

        public static void DoAfterCondition(Func<bool> predicate, Action actionToInvoke, string key = null)
        {
            if (key == null)
            {
                CoroutineWorker.Instance.StartCoroutine(WaitUntil(predicate, actionToInvoke));
            }
            else
            {
                StartCoroutine(key, WaitUntil(predicate, actionToInvoke));
            }
        }

        public static void DoAfterFixedUpdate(Action actionToInvoke, string key = null, bool ignoreTimeScale = true)
        {
            if (key == null)
            {
	            CoroutineWorker.Instance.StartCoroutine(ignoreTimeScale
		            ? WaitInRealtime(Time.fixedDeltaTime, actionToInvoke)
		            : Wait(Time.fixedDeltaTime, actionToInvoke));
            }
            else
            {
                StartCoroutine(key,  ignoreTimeScale
	                ? WaitInRealtime(Time.fixedDeltaTime, actionToInvoke)
	                : Wait(Time.fixedDeltaTime, actionToInvoke));
            }
        }

        public static void DoAfterGivenTime(float time, Action actionToInvoke, string key = null, bool overrideIfExists = false, bool ignoreTimeScale = true)
        {
            if (key == null)
            {
	            CoroutineWorker.Instance.StartCoroutine(ignoreTimeScale
		            ? WaitInRealtime(time, actionToInvoke)
		            : Wait(time, actionToInvoke));
            }
            else
            {
	            StartCoroutine(ignoreTimeScale
		            ? WaitInRealtime(time, actionToInvoke)
		            : Wait(time, actionToInvoke), key, overrideIfExists);
            }
        }

        private static IEnumerator WaitUntil(Func<bool> predicate, Action actionToInvoke)
        {
            yield return new WaitUntil(predicate);
            if (actionToInvoke != null) actionToInvoke();
        }

        private static IEnumerator Wait(float time, Action actionToInvoke)
        {
            yield return new WaitForSeconds(time);

            actionToInvoke.Invoke();
        }

        private static IEnumerator WaitInRealtime(float time, Action actionToInvoke)
        {
            yield return new WaitForSecondsRealtime(time);

            actionToInvoke.Invoke();
        }

	    private static string GenerateKey(IEnumerator iEnumerator)
	    {
	        if (iEnumerator != null)
            {
                Type type = iEnumerator.GetType();
                if (type.DeclaringType != null) return type.DeclaringType.Name + type.Name;
            }

            return "";
        }

        /// <summary>
        /// Returns TRUE if coroutine is running
        /// </summary>
        /// <param name="iEnumerator"></param>
        /// <returns></returns>
        public static bool IsCoroutineRunning(IEnumerator iEnumerator)
	    {
		    return IsCoroutineRunning(GenerateKey(iEnumerator));
	    }

	    /// <summary>
		/// Returns TRUE if coroutine is running
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
	    public static bool IsCoroutineRunning(string key)
	    {
		    if (!Coroutines.ContainsKey(key))
		    {
			    return false;
		    }

		    return Coroutines[key].Running;
	    }
	}
}