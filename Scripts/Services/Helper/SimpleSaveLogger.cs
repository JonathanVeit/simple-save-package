using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SimpleSave.Services
{
    /// <inheritdoc/>
    internal class SimpleSaveLogger : ILogger
    {
        /// <inheritdoc/>
        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            Debug.unityLogger.LogFormat(logType, context, AddPrefix(format), args);
        }

        /// <inheritdoc/>
        public void LogException(Exception exception, Object context)
        {
            Debug.unityLogger.LogException(exception, context);
        }

        /// <inheritdoc/>
        public bool IsLogTypeAllowed(LogType logType)
        {
            return Debug.unityLogger.IsLogTypeAllowed(logType);
        }

        /// <inheritdoc/>
        public void Log(LogType logType, object message)
        {
            if (message is string asString)
            {
                Debug.unityLogger.Log(logType,AddPrefix(asString));
                return;
            }

            Debug.unityLogger.Log(logType, message);
        }

        /// <inheritdoc/>
        public void Log(LogType logType, object message, Object context)
        {
            if (message is string asString)
            {
                Debug.unityLogger.Log(logType, (object) AddPrefix(asString), context);
                return;
            }

            Debug.unityLogger.Log(logType, message, context);
        }

        /// <inheritdoc/>
        public void Log(LogType logType, string tag, object message)
        {
            Debug.unityLogger.Log(logType, AddPrefix(tag), message);
        }

        /// <inheritdoc/>
        public void Log(LogType logType, string tag, object message, Object context)
        {
            Debug.unityLogger.Log(logType, AddPrefix(tag), message, context);
        }

        /// <inheritdoc/>
        public void Log(object message)
        {
            if (message is string asString)
            {
                Debug.unityLogger.Log(AddPrefix(asString));
                return;
            }

            Debug.unityLogger.Log(message);
        }

        /// <inheritdoc/>
        public void Log(string tag, object message)
        {
            Debug.unityLogger.Log(AddPrefix(tag), message);
        }

        /// <inheritdoc/>
        public void Log(string tag, object message, Object context)
        {
            Debug.unityLogger.Log(AddPrefix(tag), message, context);
        }

        /// <inheritdoc/>
        public void LogWarning(string tag, object message)
        {
            Debug.unityLogger.LogWarning(AddPrefix(tag), message);
        }

        /// <inheritdoc/>
        public void LogWarning(string tag, object message, Object context)
        {
            Debug.unityLogger.LogWarning(AddPrefix(tag), message, context);
        }

        /// <inheritdoc/>
        public void LogError(string tag, object message)
        {
            Debug.unityLogger.LogError(AddPrefix(tag), message);
        }

        /// <inheritdoc/>
        public void LogError(string tag, object message, Object context)
        {
            Debug.unityLogger.LogError(AddPrefix(tag), message, context);
        }

        /// <inheritdoc/>
        public void LogFormat(LogType logType, string format, params object[] args)
        {
            Debug.unityLogger.LogFormat(logType, AddPrefix(format), args);
            return;
        }

        /// <inheritdoc/>
        public void LogException(Exception exception)
        {
            Debug.unityLogger.LogException(exception);
        }

        private string AddPrefix (string input)
        {
            if (Application.isEditor)
            {
                input = "<color=#fbff00>[SimpleSave]</color> " + input;
            }

            return input;
        }

        /// <inheritdoc/>
        public ILogHandler logHandler 
        { 
            get => Debug.unityLogger.logHandler;
            set => Debug.unityLogger.logHandler = value;
        }

        /// <inheritdoc/>
        public bool logEnabled
        {
            get => Debug.unityLogger.logEnabled;
            set => Debug.unityLogger.logEnabled = value;
        }

        /// <inheritdoc/>
        public LogType filterLogType
        {
            get => Debug.unityLogger.filterLogType;
            set => Debug.unityLogger.filterLogType = value;
        }
    }
}