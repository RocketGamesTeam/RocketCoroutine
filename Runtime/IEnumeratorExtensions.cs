using System;
using System.Collections;

namespace RocketCoroutine
{
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// Start Coroutine [Single Instance]
        /// Use with a key if you require multiple instances of coroutine at the same time
        /// </summary>
        /// <param name="iEnumerator">Coroutine</param>
        /// <param name="overrideIfExists">Stop first if same coroutine is already working</param>
        /// <param name="onFinished"></param>
        public static void StartCoroutine(this IEnumerator iEnumerator, bool overrideIfExists = false, Action<bool> onFinished = null)
        {
            CoroutineController.StartCoroutine(iEnumerator, overrideIfExists, onFinished);
        }

        /// <summary>
        /// Start Coroutine [Multiple Instance]
        /// !! Has to be stopped with the key !!
        /// </summary>
        /// <param name="iEnumerator">Coroutine</param>
        /// <param name="key">Unique key</param>
        /// <param name="overrideIfExists">Stop first if coroutine with same key is already working</param>
        /// <param name="onFinished"></param>
        public static void StartCoroutine(this IEnumerator iEnumerator, string key, bool overrideIfExists = false, Action<bool> onFinished = null)
        {
            CoroutineController.StartCoroutine(iEnumerator, key, overrideIfExists, onFinished);
        }

        /// <summary>
        /// Stops coroutine started without a key
        /// </summary>
        public static void StopCoroutine(this IEnumerator iEnumerator)
        {
            CoroutineController.StopCoroutine(iEnumerator);
        }

        /// <summary>
        /// Stops coroutine started with a key
        /// </summary>
        public static void StopCoroutine(this IEnumerator iEnumerator, string key)
        {
            CoroutineController.StopCoroutine(key);
        }
    }
}
