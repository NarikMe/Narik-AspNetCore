using System;
using Narik.Common.Services.Core;
using NLog;

namespace Narik.Common.Infrastructure.Service
{
    public class NarikLoggingService :  ILoggingService
    {
        private readonly ILogger _logger;

        public NarikLoggingService(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }
        public void Debug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        public void Debug(Exception exception, string format, params object[] args)
        {
            if (IsDebugEnabled || LogLevel.Debug>=MinLogLevel) return;
            _logger.Debug(exception);
        }

        public void Error(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void Error(Exception exception, string format, params object[] args)
        {
            _logger.Error(exception,format,args);
        }

        public void Fatal(string format, params object[] args)
        {
            _logger.Fatal(format,args);
        }

        public void Fatal(Exception exception, string format, params object[] args)
        {
            if (!IsFatalEnabled || LogLevel.Fatal >= MinLogLevel) return;
            _logger.Fatal(exception,format,args);
        }

        public void Info(string format, params object[] args)
        {
            _logger.Info(format, args);
        }

        public void Info(Exception exception, string format, params object[] args)
        {
            if (!IsInfoEnabled || LogLevel.Info >= MinLogLevel) return;
            _logger.Info(exception,format,args);
        }

        public void Trace(string format, params object[] args)
        {
            _logger.Trace(format, args);
        }

        public void Trace(Exception exception, string format, params object[] args)
        {
            if (!IsTraceEnabled || LogLevel.Trace >= MinLogLevel) return;
            
            _logger.Trace(exception,format,args);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.Warn(format, args);
        }

        public void Warn(Exception exception, string format, params object[] args)
        {
            if (!IsWarnEnabled || LogLevel.Warn >= MinLogLevel) return;
            
            _logger.Warn(exception ,format,args);
        }

        public void Log(LogLevel level, IFormatProvider formatProvider, string message, params object[] args)
        {
            _logger.Log(level,formatProvider,message,args);
        }

        public void Log(LogLevel level, string message)
        {
            _logger.Log(level,  message);
        }

        public void Log(string message, LogLevel level)
        {
            _logger.Log(level, message);
        }

        public void Log(LogLevel level, string message, params object[] args)
        {
            _logger.Log(level,  message, args);
        }

        public bool IsDebugEnabled => _logger.IsDebugEnabled;
        public bool IsErrorEnabled => _logger.IsErrorEnabled;
        public bool IsFatalEnabled => _logger.IsFatalEnabled;
        public bool IsInfoEnabled => _logger.IsInfoEnabled;
        public bool IsTraceEnabled => _logger.IsTraceEnabled;
        public bool IsWarnEnabled => _logger.IsWarnEnabled;

        public void Debug(Exception exception)
        {
            Debug(exception, string.Empty);
        }

        public void Error(Exception exception)
        {
            Error(exception, string.Empty);
        }

        public void Fatal(Exception exception)
        {
            Fatal(exception, string.Empty);
        }

        public void Info(Exception exception)
        {
            Info(exception, string.Empty);
        }

        public void Trace(Exception exception)
        {
            Trace(exception, string.Empty);
        }

        public void Warn(Exception exception)
        {
            Warn(exception, string.Empty);
        }

        public void Log(Exception exception)
        {
            Error(exception);
        }

        public void Log(string message)
        {
            Log(LogLevel.Info,message);
            
        }

        public LogLevel MinLogLevel { get; set; }

    }
}
