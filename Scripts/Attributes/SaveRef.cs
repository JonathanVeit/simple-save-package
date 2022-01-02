using System;
using UnityEngine;

namespace SimpleSave
{
    /// <summary>
    /// Member with this attribute can have their references be saved and loaded. <see cref="GameObject"/> or <see cref="Component"/>
    /// Use this property to save and load references of type .
    /// </summary>
    /// <remarks>
    /// The type of the member has to be <see cref="GameObject"/>, <see cref="Component"/> or any type derived from <see cref="Component"/>.
    /// The member has to be saved by a <see cref="SaveItem"/> or marked as a <see cref="SaveVar"/> in order to save the reference.
    /// </remarks>
    /// <example>
    /// <code>
    /// public class ExampleClass : MonoBehaviour
    /// {
    ///     // the references of these fields will be saved and loaded together with the component
    ///     [SerializeField] [SaveRef] GameObject gameObjectField;
    ///     [SerializeField] [SaveRef] Transform transformField;
    /// }
    /// </code>
    /// 
    /// <code>
    /// public static class MyGlobalVariables
    /// {
    ///     // the references of this field will be saved and loaded
    ///     [SaveVar] [SaveRef] static GameObject gameObject;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SaveRef : Attribute
    {
    }
}