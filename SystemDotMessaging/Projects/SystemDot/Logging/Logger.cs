using System;

namespace SystemDot.Logging
{
    public class Logger
    {
        public static ILoggingMechanism LoggingMechanism { get; set; }

        public static void Info(string message)
        {
            if (!LoggingMechanism.ShowInfo) return;
            if (LoggingMechanism != null)
                LoggingMechanism.Info(message);
        }

        public static void Info(string message, params object[] args)
        {
            if (!LoggingMechanism.ShowInfo) return;
            if (LoggingMechanism != null)
                LoggingMechanism.Info(String.Format(message, args));
        }

        public static void Error(string message)
        {
            if (LoggingMechanism != null)
                LoggingMechanism.Error(message);
        }
    }
}