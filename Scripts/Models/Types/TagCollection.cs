using SimpleSave.Services;

namespace SimpleSave
{
    /// <summary>
    /// A collection of tags to determine which <see cref="SaveItem"/> and <see cref="SaveVar"/> should be saved or loaded.
    /// </summary>
    public class TagCollection
    {
        internal string[] Tags { get; }
        internal bool IncludeTagless { get; }
        
        private static TagCollection _taglessOnly;

        #region Services

        private static ISimpleSaveSettings Settings => ServiceWrapper.GetService<ISimpleSaveSettings>();

        #endregion

        internal TagCollection()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tags">Tags to save.</param>
        /// <remarks>
        /// The default option from the settings will be used to determine whether tagless <see cref="SaveItem"/> and <see cref="SaveVar"/> will be saved or loaded.
        /// If there are no tags given, all <see cref="SaveItem"/> and <see cref="SaveVar"/> will be saved or loaded.
        /// </remarks>
        public TagCollection(params string[] tags) : this (Settings.AlwaysSaveTagless, tags)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="includeTagless">Should <see cref="SaveItem"/> and <see cref="SaveVar"/> that do not have any tags defined be saved or loaded as well? </param>
        /// <param name="tags">Tags to save.</param>
        /// <remarks>
        /// If there are no tags given, all <see cref="SaveItem"/> and <see cref="SaveVar"/> will be saved or loaded.
        /// </remarks>
        public TagCollection(bool includeTagless, params string[] tags)
        {
            Tags = tags;
            IncludeTagless = includeTagless;
        }

        /// <summary>
        /// This will only save or load <see cref="SaveItem"/> and <see cref="SaveVar"/> that do not have any tag defined.
        /// </summary>
        public static TagCollection TaglessOnly
        {
            get
            {
                if (_taglessOnly != null)
                {
                    return _taglessOnly;
                }

                _taglessOnly = new TagCollection();
                return _taglessOnly;
            }
        }
    }
}