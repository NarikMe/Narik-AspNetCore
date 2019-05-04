using System;
using NLog;

namespace Narik.Common.Services.Core
{


    public sealed class DefaultLoggers
    {
        public const string ServerLogging = "ServerLogging";
        public const string ClinetLogging = "ClinetLogging";
    }
    public interface ILoggingService
    {
       
        bool IsDebugEnabled { get;  }
        bool IsErrorEnabled { get;  }
        bool IsFatalEnabled { get;  }
        bool IsInfoEnabled { get;  }
        bool IsTraceEnabled { get;  }
        bool IsWarnEnabled { get;  }

        void Debug(Exception exception);
        void Debug(string message, params object[] args);
        void Debug(Exception exception, string message, params object[] args);
        void Error(Exception exception);
        void Error(string message, params object[] args);
        void Error(Exception exception, string message, params object[] args);
        void Fatal(Exception exception);
        void Fatal(string message, params object[] args);
        void Fatal(Exception exception, string message, params object[] args);
        void Info(Exception exception);
        void Info(string message, params object[] args);
        void Info(Exception exception, string message, params object[] args);
        void Trace(Exception exception);
        void Trace(string message, params object[] args);
        void Trace(Exception exception, string message, params object[] args);
        void Warn(Exception exception);
        void Warn(string message, params object[] args);
        void Warn(Exception exception, string message, params object[] args);


        void Log(LogLevel level, IFormatProvider messageProvider, string message, params object[] args);
        void Log(LogLevel level,  string message);
        void Log(  string message, LogLevel level);
        void Log(LogLevel level, string message, params object[] args);


        void Log(Exception exception);
        void Log(string message);
        LogLevel MinLogLevel { get; set; }
    }

   
}
