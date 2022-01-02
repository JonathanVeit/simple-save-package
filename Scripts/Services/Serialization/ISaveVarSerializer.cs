using System;

namespace SimpleSave.Services
{
    /// <summary>
    /// Serializer for member with the <see cref="SaveVar"/> attribute. 
    /// </summary>
    /// <remarks>
    /// You can create your own implementations of this interface.<br/>
    /// If you want to use your own implementation, you need to add it to the <see cref="SaveGameSettings"/> when creating or loading save games from <see cref="Simple"/>.
    /// </remarks>
    public interface ISaveVarSerializer
    {
        /// <summary>
        /// Serializes the value of the member.
        /// </summary>
        /// <param name="saveVar">Value to serialize.</param>
        /// <returns>Serialized value.</returns>
        string Serialize(object saveVar);

        /// <summary>
        /// Deserializes the value of the member.
        /// </summary>
        /// <param name="serializedVar">Serialized value.</param>
        /// <param name="type">Type of the member.</param>
        /// <returns>The deserialized value.</returns>
        object Deserialize(string serializedVar, Type type);
    }
}