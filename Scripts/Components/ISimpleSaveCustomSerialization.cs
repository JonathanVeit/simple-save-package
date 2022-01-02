using UnityEngine;

namespace SimpleSave
{
    /// <summary>
    /// Provides methods for the custom serialization of a specific <see cref="Component"/>.
    /// </summary>
    /// <remarks>
    /// The component has to be saved by a <see cref="SaveItem"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class ExampleClass : MonoBehaviour, ISimpleSaveCustomSerialization
    /// {
    ///     // these fields should not be saved
    ///     [SerializeField] private int intField1;
    ///     [SerializeField] private int intField2;
    ///     [SerializeField] private int intField3;
    ///     [SerializeField] private int stringField1;
    ///     [SerializeField] private int stringField2;
    ///     [SerializeField] private int stringField3;
    ///
    ///     // these fields should be saved
    ///     [SerializeField] private bool boolField;
    ///     [SerializeField] private float floatField;
    /// 
    ///     // this create a SaveState and stores the relevant data
    ///     public string Serialize()
    ///     {
    ///         var saveState = new SaveState
    ///         {
    ///             BoolField = boolField
    ///             FloatField = floatField
    ///         };
    ///
    ///         return JsonUtility.ToJson(saveState);
    ///     }
    ///
    ///     // this restores the relevant data from the SaveState
    ///     public void Populate(string serialized)
    ///     {
    ///         var saveState = JsonUtility.FromJson<SaveState>(serialized);
    ///
    ///         boolField = saveState.BoolField;
    ///         floatField = saveState.FloatField;
    ///     }
    ///
    ///     // this struct stores the relevant data
    ///     [Serializable] 
    ///     private struct SaveState
    ///     {
    ///         public bool BoolField;
    ///         public float FloatField;
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface ISimpleSaveCustomSerialization
    {
        /// <summary>
        /// Serializes the <see cref="Component"/>.
        /// </summary>
        string Serialize();

        /// <summary>
        /// Populates on the <see cref="Component"/> instance.
        /// </summary>
        void Populate(string serialized);
    }
}