using System.Text;
using SimpleSave.Models;

namespace SimpleSave.Services
{
    /// <inheritdoc/>
    internal class IdProvider : IIdProvider
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        /// <inheritdoc/>
        public SaveItemId GetNewItemId()
        {
            var id = new SaveItemId(System.Guid.NewGuid().ToString());
            return id;
        }

        /// <inheritdoc/>
        public ComponentId GetNewComponentId()
        {
            var id = new ComponentId(System.Guid.NewGuid().ToString());
            return id;
        }

        /// <inheritdoc/>
        public SaveVarId GetSaveVarId(SaveVarInfo forField)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(forField.DeclaringType.AssemblyQualifiedName);
            _stringBuilder.Append(forField.MemberName);
            return new SaveVarId(_stringBuilder.ToString().GetHashCode());
        }

        /// <inheritdoc/>
        public ReferenceId GetReferenceId(ReferenceInfo forReference)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(forReference.DeclaringType.AssemblyQualifiedName);
            _stringBuilder.Append(forReference.MemberName);
            return new ReferenceId(_stringBuilder.ToString().GetHashCode());
        }

        /// <inheritdoc/>
        public TagId GetNewTagId()
        {
            var id = new TagId(System.Guid.NewGuid().ToString());
            return id;
        }

        /// <inheritdoc/>
        public SaveGameId GetNewSaveGameId()
        {
            var id = new SaveGameId(System.Guid.NewGuid().ToString());
            return id;
        }
    }
}