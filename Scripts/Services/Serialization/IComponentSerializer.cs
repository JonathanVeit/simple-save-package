using UnityEngine;


namespace SimpleSave.Services
{
    /// <summary>
    /// Serializer for <see cref="Component"/>s.
    /// </summary>
    /// <remarks>
    /// You can create your own implementations of this interface.<br/>
    /// If you want to use your own implementation, you need to add it to the <see cref="SaveGameSettings"/> when creating or loading save games from <see cref="Simple"/>.
    /// </remarks>
    public interface IComponentSerializer
    {
        /// <summary>
        /// Serializes the component.
        /// </summary>
        /// <param name="component">Component to serialize.</param>
        /// <returns>Serialized component.</returns>
        public string Serialize(Component component);

        /// <summary>
        /// Populates the serialized component to the given instance.
        /// </summary>
        /// <param name="serializedComponent">Serialized component.</param>
        /// <param name="targetComponent">Component to populate to.</param>
        public void PopulateTo(string serializedComponent, Component targetComponent);
    }
}