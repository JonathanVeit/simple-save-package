using System;

namespace SimpleSave.Models
{
    /// <summary>
    /// Information about a member with the <see cref="SaveVar"/> attribute.
    /// </summary>
    public struct ReferenceInfo
    {
        /// <summary>
        /// Unique id of the reference.
        /// </summary>
        public ReferenceId Id;

        /// <summary>
        /// Category of the member.
        /// </summary>
        public MemberCategory MemberCategory;

        /// <summary>
        /// Unique name of the member (identifier).
        /// </summary>
        public string MemberName;

        /// <summary>
        /// Type of the member
        /// </summary>
        public Type MemberType;

        /// <summary>
        /// Type of the class declaring the member.
        /// </summary>
        /// <remarks>This could also be some base class of the <see cref="OwningType"/>.</remarks>
        public Type DeclaringType;

        /// <summary>
        /// Type of the class owning the member.
        /// </summary>
        public Type OwningType;
    }
}