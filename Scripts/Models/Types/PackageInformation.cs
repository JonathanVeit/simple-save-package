namespace SimpleSave.Models
{
    /// <summary>
    /// Information about a unity package.
    /// </summary>
    [System.Serializable]
    internal struct PackageInformation
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unity;
        public string unityRelease;
        public string documentationUrl;
        public string changelogUrl;
        public string licensesUrl;
        public string[] keywords;
        public PackageAuthorInformation author;
    }

    [System.Serializable]
    internal struct PackageAuthorInformation
    {
        public string name;
        public string email;
        public string url;
    }
}