
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer
{
    public static class TryCatch
    {
        private static ILogger _logger = Logger.Register(typeof(TryCatch));

        public static Exception Run(Action action)
        {
            var ex = _run(action);
            if(ex != null)
            {
                _logger.LogError(ex.ToString());
                _logger.LogException(ex);
            }
            return ex;
        }

        public static Exception Run(Func<string> func, Action action)
        {
            var ex = _run(action);
            if (ex != null)
            {
                _logger.LogError(func() + "\r\n" + ex.ToString());
                _logger.LogException(ex);
            }
            return ex;
        }

        public static Exception RunSilent(Action action)
        {
            return _run(action);
        }

        public static void RunThrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                _logger.LogException(ex);
                throw;
            }
        }

        public static void RunThrow(Func<string> func, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.LogError(func() +"\r\n"+ ex.ToString());
                _logger.LogException(ex);
                throw;
            }
        }

        static Exception _run(Action action)
        {
            Exception _ex = null;
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _ex = ex;
            }
            return _ex;
        }
    }

    public static class TryTime
    {
        public static long RunStopWatch(Action action)
        {
            return RunStopWatch(action, null);
        }

        public static long RunStopWatch(Action action, Action<long> final = null)
        {
            var sw = new Stopwatch();
            long ms = 0;

            sw.Start();
            try {
                action();
            }
            finally
            {
                sw.Stop();
                ms = sw.ElapsedMilliseconds;
                sw = null;
                final(ms);
            }
            return ms;
        }


    }
}
