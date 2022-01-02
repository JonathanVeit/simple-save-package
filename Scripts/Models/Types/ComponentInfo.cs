using UnityEngine;
using System;

namespace SimpleSave.Models
{
	/// <summary>
    /// Information about a component to be saved by a <see cref="SaveItem"/>.
    /// </summary>
    /// <remarks>This type is serializable in the inspector.</remarks>
    [Serializable]
    internal class ComponentInfo
    {
        [SerializeField] private ComponentId _id;
        [SerializeField] private Component _component;

        public ComponentId Id => _id;
        public Component Component => _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <param name="component">Component to save.</param>
        public ComponentInfo(ComponentId id, Component component)
        {
            if (id == ComponentId.Invalid)
            {
                throw new ArgumentException("Identifier of ComponentInfo is invalid.");
            }

            _id = id;
            _component = component;
        }

        /// <summary>
        /// Sets the saved component.
        /// </summary>
        /// <param name="component"></param>
        public void SetComponent(Component component)
        {
            _component = component;
        }
    }
}