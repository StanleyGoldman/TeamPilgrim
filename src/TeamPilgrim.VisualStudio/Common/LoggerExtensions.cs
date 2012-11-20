using System;
using NLog;

namespace JustAProgrammer.TeamPilgrim.VisualStudio.Common
{
    public static class LoggerExtensions
    {
        public static void DebugException(this Logger logger, Exception ex)
        {
            logger.DebugException(String.Empty, ex);
        }

        public static void TraceException(this Logger logger, Exception ex)
        {
            logger.TraceException(String.Empty, ex);
        }

        public static void ErrorException(this Logger logger, Exception ex)
        {
            logger.ErrorException(String.Empty, ex);
        }

        public static void FatalException(this Logger logger, Exception ex)
        {
            logger.FatalException(String.Empty, ex);
        }

        public static void InfoException(this Logger logger, Exception ex)
        {
            logger.InfoException(String.Empty, ex);
        }

        public static void WarnException(this Logger logger, Exception ex)
        {
            logger.WarnException(String.Empty, ex);
        }
    }
}
