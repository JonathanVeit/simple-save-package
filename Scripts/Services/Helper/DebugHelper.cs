using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleSave.Services
{
    /// <inheritdoc/>
    internal class DebugHelper : IDebugHelper
    {
        private readonly Dictionary<string, Stopwatch> _stopwatches = new Dictionary<string, Stopwatch>();

        /// <inheritdoc/>
        public void StartTimer(string id)
        {
            if (!_stopwatches.ContainsKey(id))
            {
                _stopwatches.Add(id, new Stopwatch());
            }

            _stopwatches[id].Start();
        }

        /// <inheritdoc/>
        public long GetTimer(string id)
        {
            if (!_stopwatches.ContainsKey(id))
            {
                _stopwatches.Add(id, new Stopwatch());
            }

            return _stopwatches[id].ElapsedMilliseconds;
        }

        /// <inheritdoc/>
        public long StopTimer(string id)
        {
            if (!_stopwatches.ContainsKey(id))
            {
                _stopwatches.Add(id, new Stopwatch());
            }

            _stopwatches[id].Stop();
            long elapsedMilliseconds = _stopwatches[id].ElapsedMilliseconds;
            _stopwatches[id].Reset();
            return elapsedMilliseconds;
        }
    }
}