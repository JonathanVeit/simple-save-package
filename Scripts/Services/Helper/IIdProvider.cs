using SimpleSave.Models;

namespace SimpleSave.Services
{
    /// <summary>
    /// Provides unique identifiers of different types.
    /// </summary>
    internal interface IIdProvider
    {
        /// <summary>
        /// Creates a new unique <see cref="SaveItemId"/>
        /// </summary>
        /// <returns>The created id.</returns>
        SaveItemId GetNewItemId();

        /// <summary>
        /// Creates a new unique <see cref="ComponentId"/>.
        /// </summary>
        /// <returns>The created id.</returns>
        ComponentId GetNewComponentId();

        /// <summary>
        /// Creates a new unique <see cref="SaveVarInfo"/>.
        /// </summary>
        /// <param name="forField">SaveVarInfo to create the Id for.</param>
        /// <returns>The created Id.</returns>
        /// <remarks>The Id is based on the <see cref="SaveVarInfo"/>. The same instance will get the same id.</remarks>
        SaveVarId GetSaveVarId(SaveVarInfo forField);

        /// <summary>
        /// Creates a new unique <see cref="ReferenceId"/>.
        /// </summary>
        /// <param name="forReference">ReferenceInfo to create the Id for.</param>
        /// <returns>The created Id.</returns>
        /// <remarks>The Id is based on the <see cref="ReferenceInfo"/>. The same info will get the same id.</remarks>
        ReferenceId GetReferenceId(ReferenceInfo forReference);

        /// <summary>
        /// Creates a new unique <see cref="TagId"/>.
        /// </summary>
        /// <returns>The created id.</returns>
        TagId GetNewTagId();


        /// <summary>
        /// Creates a new unique <see cref="SaveGameId"/>.
        /// </summary>
        /// <returns>The created id.</returns>
        SaveGameId GetNewSaveGameId();
    }
}