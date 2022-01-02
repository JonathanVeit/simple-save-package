namespace SimpleSave.Models.Serializables
{
    /// <summary>
    /// Serializable entry in the <see cref="SaveGameInfo.CustomData"/>.
    /// </summary>
    [System.Serializable]
    public struct SerializableCustomDataEntry
    {
        /// <summary>
        /// Key of the entry.
        /// </summary>
        public string Key;

        /// <summary>
        /// Value of the Entry.
        /// </summary>
        public string Value;
    }
}