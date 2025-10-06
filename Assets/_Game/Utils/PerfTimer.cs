using System.Diagnostics;
using _Game.Core._Logger;

namespace _Game.Utils
{
    public class PerfTimer
    {
        private Stopwatch _stopwatch;
        private readonly string _label;
        private readonly IMyLogger _logger;

        public PerfTimer(IMyLogger logger, string label = "[PerfTimer]")
        {
            _label = label;
            _stopwatch = new Stopwatch();
            _logger = logger;
        }

        public void Start()
        {
            _stopwatch.Restart();
        }

        public void Stop()
        {
            _stopwatch.Stop();
            _logger.Log($"{_label} Elapsed: {_stopwatch.ElapsedMilliseconds} ms", DebugStatus.Fault);
        }

        public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;
    }
}