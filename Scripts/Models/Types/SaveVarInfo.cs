using System;

namespace SimpleSave.Models
{
    /// <summary>
    /// Information about a member with the <see cref="SaveVar"/> attribute.
    /// </summary>
    public struct SaveVarInfo
    {
        /// <summary>
        /// Unique id of the SaveVar.
        /// </summary>
        public SaveVarId Id;

        /// <summary>
        /// Type of the SaveVar.
        /// </summary>
        public MemberCategory MemberCategory;

        /// <summary>
        /// Name of the member.
        /// </summary>
        public string MemberName;

        /// <summary>
        /// Declaring  type of the member.
        /// </summary>
        public Type DeclaringType;

        /// <summary>
        /// Type of the member.
        /// </summary>
        public Type MemberType;

        /// <summary>
        /// Tags of the SaveVar.
        /// </summary>
        public string[] Tags;
    }
}