namespace SimpleSave.Models
{
    /// <summary>
    /// Categories for members to use the <see cref="SaveVar"/> or <see cref="SaveRef"/> attribute.
    /// </summary>
    public enum MemberCategory
    {
        /// <summary>
        /// Member is a field. 
        /// </summary>
        Field = 0,

        /// <summary>
        /// Member is a property.
        /// </summary>
        Property = 1,
    }
}
