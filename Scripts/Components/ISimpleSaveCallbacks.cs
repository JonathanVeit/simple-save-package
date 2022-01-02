using UnityEngine;

namespace SimpleSave
{
    /// <summary>
    /// Provides methods to be called during the process of saving and loading a <see cref="Component"/>.
    /// </summary>
    /// <remarks>
    /// The component has to be saved by a <see cref="SaveItem"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class ExampleClass : MonoBehaviour, ISimpleSaveCallbacks
    /// {
    ///     // not serializable
    ///     private DateTime dateTime;
    /// 
    ///     // serializable
    ///     [SerializeField] private string dateTimeString;
    ///
    ///     // this will store the string value of the DateTime before saving
    ///     public void OnBeforeSaved()
    ///     {
    ///         dateTimeString = dateTime.ToString();
    ///     }
    ///
    ///     // this will restore the DateTime after loading
    ///     public void OnAfterLoaded()
    ///     {
    ///         dateTime = DateTime.Parse(dateTimeString);
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface ISimpleSaveCallbacks
    {
        /// <summary>
        /// Will be called right before the component is saved.
        /// </summary>
        void OnBeforeSaved();

        /// <summary>
        /// Will be called right after the component has been loaded.
        /// </summary>
        void OnAfterLoaded();
    }

    
}