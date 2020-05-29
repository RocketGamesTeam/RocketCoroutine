using System;
using System.Collections;

namespace RocketCoroutine
{
    public class RocCoroutine
    {
        public event Action<RocCoroutine, bool> Finished;
        public Action<bool> Callback;

        public string Key;
        public bool Running { get { return _running; } }
        public bool Paused { get { return _paused; } }

        private readonly IEnumerator _iEnumerator;
        private bool _running;
        private bool _paused;
        private bool _stopped;

        public RocCoroutine(IEnumerator iEnumerator, Action<bool> callback = null)
        {
            _iEnumerator = iEnumerator;
            Callback = callback;
        }

        public RocCoroutine(string key, IEnumerator iEnumerator, Action<bool> callback = null)
        {
            Key = key;
            _iEnumerator = iEnumerator;
            Callback = callback;
        }

        public void Pause()
        {
            _paused = true;
        }

        public void Resume()
        {
            _paused = false;
        }

        public void Start()
        {
            _running = true;
            CoroutineWorker.Instance.StartCoroutine(CallWrapper());
        }

        public void Stop()
        {
            _stopped = true;
            _running = false;
        }

        IEnumerator CallWrapper()
        {
            yield return null;
            IEnumerator e = _iEnumerator;
            while (_running)
            {
                if (_paused)
                    yield return null;
                else
                {
                    if (e != null && e.MoveNext())
                    {
                        yield return e.Current;
                    }
                    else
                    {
                        _running = false;
                    }
                }
            }

            if (Callback != null) Callback(_stopped);
            if (Finished != null) Finished(this, _stopped);
        }
    }
}
