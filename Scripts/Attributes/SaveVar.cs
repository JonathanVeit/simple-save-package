using System;

namespace SimpleSave
{
    /// <summary>
    /// Member with this attribute will be saved and loaded.
    /// </summary>
    /// <remarks>
    /// The member has to be declared as static.
    /// </remarks>
    /// <example>
    /// <code>
    /// public static class ExampleClass
    /// {
    ///     // the value of this field will be saved and loaded
    ///     [SaveVar] static int intField;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SaveVar : Attribute
    {
        public string[] Tags { get; }

        /// <summary>
        /// Constructor with tags.
        /// </summary>
        /// <param name="tags">Tags of the SaveVar.</param>
        public SaveVar(params string[] tags)
        {
            Tags = tags;
        }
    }
}