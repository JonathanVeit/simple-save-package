using System;
using SimpleSave.Models;
using SimpleSave.Services;
using UnityEngine;

namespace SimpleSave.Extensions
{
    /// <summary>
    /// <see cref="ILogger"/> extensions.
    /// </summary>
    internal static class LoggerExtensions
    {
        #region Services

        private static ISimpleSaveSettings Settings => ServiceWrapper.GetService<ISimpleSaveSettings>();

        #endregion

        /// <summary>
        /// Log based on the <see cref="InternalLogType"/> defined in the <see cref="ISimpleSaveSettings"/>.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="message">Message to log.</param>
        public static void LogInternal(this ILogger logger, string message)
        {
            switch (Settings.LogType)
            {
                case InternalLogType.Log:
                    logger.Log(LogType.Log, message);
                    break;
                case InternalLogType.Warning:
                    logger.Log(LogType.Warning, message);
                    break;
                case InternalLogType.Error:
                    logger.Log(LogType.Error, message);
                    break;
                case InternalLogType.Exception:
                    logger.Log(LogType.Exception, message);
                    break;
                case InternalLogType.Ignore:
                    return;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Debug logs. Can be disabled in the <see cref="ISimpleSaveSettings"/>.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="message">Message to log.</param>
        public static void LogDebug(this ILogger logger, string message)
        {
            if (Settings.DebugLogging)
            {
                logger.Log(LogType.Log, message);
            }
        }
    }
}
