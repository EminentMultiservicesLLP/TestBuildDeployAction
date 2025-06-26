using System;
using System.IO;
using log4net;
using System.Globalization;
using System.Threading;
using System.Web;

namespace CommonLayer
{
    public sealed class Logger
    {
        public const long DefaultThresholds = 3000;
        public static readonly string DefaultThresholdMessage = "[*> " + DefaultThresholds.ToString("#,###") + " ms] ";
        public static readonly Func<long, string> DefaultThresholdMessageFunc = x => "[*> " + x.ToString("#,###") + " ms] ";
        public static string LoggedInUser
        {
            get
            {
                if (HttpContext.Current != null && HttpContext.Current.ApplicationInstance.Session.Count > 0)
                    return Convert.ToString(HttpContext.Current.ApplicationInstance.Session["AppUserId"]);
                else
                return "0";
            }
        }

        public static ILogger Register(Type type)
        {
            return new LoggerImpl(type);
        }

        class LoggerImpl : ILogger
        {
            static ILog _logger;

            public string TypeName { get; private set; }

            static LoggerImpl()
            {
                var log4NetConfigDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

                var log4NetConfigFilePath = Path.Combine(log4NetConfigDirectory, "log4net.config");
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(log4NetConfigFilePath));
            }

            public LoggerImpl(Type logClass)
            {
                _logger = LogManager.GetLogger(logClass);
                TypeName = logClass.ToString();
            }


            public void LogError(string errorMessage)
            {
                _logger.Error(BuildMessage(errorMessage, "Error"));
            }

            public void LogError(string errorMessage, Exception ex)
            {
                var msg = errorMessage + ex.Message + Environment.NewLine + ex.StackTrace;
                if (ex.InnerException != null)
                    msg = msg + Environment.NewLine + " **** Inner Exception detail **** " + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.InnerException.StackTrace;
                LogError(msg);
            }

            public void LogError(Func<string> errorMessage)
            {
                _logger.Error(BuildMessage(errorMessage(), "Error"));
            }

            public void LogErrorFormat(string mask, params string[] values)
            {
                _logger.Error(BuildMessage(string.Format(mask, values), "Error"));
            }

            public void LogDebug(string errorMessage)
            {
                _logger.Debug(BuildMessage(errorMessage, "Debug"));
            }

            public void LogDebug(Func<string> errorMessage)
            {
                _logger.Debug(BuildMessage(errorMessage(), "Debug"));
            }

            public void LogDebugFormat(string mask, params string[] values)
            {
                _logger.Debug(BuildMessage(string.Format(mask, values), "Debug"));
            }

            public void LogInfo(string errorMessage)
            {
                _logger.Info(BuildMessage(errorMessage, "Info"));
            }

            public void LogInfo(Func<string> errorMessage)
            {
                _logger.Info(BuildMessage(errorMessage(), "Info"));
            }

            public void LogInfoFormat(string mask, params object[] values)
            {
                _logger.Info(BuildMessage(string.Format(mask, values), "Info"));
            }

            public void LogException(Exception ex)
            {
                var msg = ex.Message + Environment.NewLine + ex.StackTrace;
                if (ex.InnerException != null)
                    msg = msg + Environment.NewLine + " **** Inner Exception detail **** " + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.InnerException.StackTrace;

                LogError(msg);
            }

            public long TimeLog(Func<string> tag, Action action)
            {
                long ms = 0;
                try
                {
                    TryCatch.RunThrow(tag, () => TryTime.RunStopWatch(action));
                }
                finally
                {
                    LogInfoFormat("{[1:#,##0} ms] {2}{0}", tag(), ms, (ms >= DefaultThresholds ? DefaultThresholdMessage : string.Empty));
                }
                return ms;
            }

            public long TimeLogBeginEnd(Func<string> tag, Action action)
            {
                var uid = Guid.NewGuid().ToString();
                long ms = 0;
                LogInfoFormat("Begin: {0} - (guid: {1}", tag, uid);
                try
                {
                    TryCatch.RunThrow(tag, () => ms = TryTime.RunStopWatch(action));
                }
                finally
                {
                    LogInfoFormat("{[1:#,##0} ms] {2}{0}", tag(), ms, uid, (ms >= DefaultThresholds ? DefaultThresholdMessage : string.Empty));
                }
                return ms;
            }

            public long WatchTime(Action action, long timems = DefaultThresholds)
            {
                throw new NotImplementedException();
            }

            public long WatchTime(Func<string> tag, Action action, long timems = DefaultThresholds)
            {
                var ms = TryTime.RunStopWatch(action, ml =>
                    {
                        if (ml > timems)
                            LogInfoFormat("Operation Time look longer than expected : [{1:#,###} ms] {2} '{0}' ", tag(), ml, DefaultThresholdMessageFunc(timems));
                    });
                return ms;
            }

            public string BuildMessage(string msg, string logType = "")
            {
                var appUser = "for Logged-in UserId: " + LoggedInUser;
                //return string.Format("{5} {1} [{2}] {3} ({0})\r\n", msg, DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture), Thread.CurrentThread.ManagedThreadId, TypeName, logType, appUser);
                return string.Format("{5} {3} ({0})\r\n", msg, DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture), Thread.CurrentThread.ManagedThreadId, TypeName, logType, appUser);
            }


        }

    }

    public interface ILogger
    {
        void LogError(string message);
        void LogError(string message, Exception ex);

        void LogError(Func<string> message);
        void LogErrorFormat(string mask, params string[] values);

        void LogDebug(string message);
        void LogDebug(Func<string> message);
        void LogDebugFormat(string mask, params string[] values);

        void LogInfo(string message);
        void LogInfo(Func<string> message);
        void LogInfoFormat(string mask, params object[] values);

        void LogException(Exception ex);

        long TimeLog(Func<string> tag, Action action);
        long TimeLogBeginEnd(Func<string> tag, Action action);

        long WatchTime(Action action, long timems = Logger.DefaultThresholds);
        long WatchTime(Func<string> tag, Action action, long timems = Logger.DefaultThresholds);
    }

}
