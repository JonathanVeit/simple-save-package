using SimpleSave.Services;
using UnityEngine;

namespace SimpleSave
{
    /// <summary>
    /// Base class for services.
    /// </summary>
    internal class BaseService
    {
        protected static ISimpleSaveSettings Settings => ServiceWrapper.GetService<ISimpleSaveSettings>();
        protected static ILogger Logger => ServiceWrapper.GetService<ILogger>();
        protected static IDebugHelper DebugHelper => ServiceWrapper.GetService<IDebugHelper>();
        protected static ICoroutineHelper CoroutineHelper => ServiceWrapper.GetService<ICoroutineHelper>();
    }
}