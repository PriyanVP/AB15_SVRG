using NLog;
using System;

namespace AB15_GUI.WPF.NLog
{
    public class LoggingService : ILoggingService
    {
        private readonly Logger _logger;

        public LoggingService()
        {
            _logger = LogManager.Setup()
                                .SetupExtensions(ext => ext.RegisterLayoutRenderer<BuildConfigurationLayoutRenderer>("build-configuration"))
                                .SetupExtensions(ext => ext.RegisterTarget<LogMemoryRecordTarget>("MemoryRecord"))
                                .GetCurrentClassLogger();
        }

        public bool IsDebugEnabled => _logger.IsDebugEnabled;

        public bool IsErrorEnabled => _logger.IsErrorEnabled;

        public bool IsFatalEnabled => _logger.IsFatalEnabled;

        public bool IsInfoEnabled => _logger.IsInfoEnabled;

        public bool IsTraceEnabled => _logger.IsTraceEnabled;

        public bool IsWarnEnabled => _logger.IsWarnEnabled;

        public string Name => _logger.Name;

        public void Debug(Exception exception)
        {
            _logger.Debug(exception);
        }

        public void Debug(string format, params object[] args)
        {
            _logger.Debug(format, args);
        }

        public void Debug(Exception exception, string format, params object[] args)
        {
            _logger.Debug(exception, format, args);
        }

        public void Error(Exception exception)
        {
            _logger.Error(exception);
        }

        public void Error(string format, params object[] args)
        {
            _logger.Error(format, args);
        }

        public void Error(Exception exception, string format, params object[] args)
        {
            _logger.Error(exception, format, args);
        }

        public void Fatal(Exception exception)
        {
            _logger.Fatal(exception);
        }

        public void Fatal(string format, params object[] args)
        {
            _logger.Fatal(format, args);
        }

        public void Fatal(Exception exception, string format, params object[] args)
        {
            _logger.Fatal(exception, format, args);
        }

        public void Info(Exception exception)
        {
            _logger.Info(exception);
        }

        public void Info(string format, params object[] args)
        {
            _logger.Info(format, args);
        }

        public void Info(Exception exception, string format, params object[] args)
        {
            _logger.Info(exception, format, args);
        }

        public void Trace(Exception exception)
        {
            _logger.Trace(exception);
        }

        public void Trace(string format, params object[] args)
        {
            _logger.Trace(format, args);
        }

        public void Trace(Exception exception, string format, params object[] args)
        {
            _logger.Trace(exception, format, args);
        }

        public void Warn(Exception exception)
        {
            _logger.Warn(exception);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.Warn(format, args);
        }

        public void Warn(Exception exception, string format, params object[] args)
        {
            _logger.Warn(exception, format, args);
        }
    }
}