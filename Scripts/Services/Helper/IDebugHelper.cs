namespace SimpleSave.Services
{
    /// <summary>
    /// helper class for SimpleSave.
    /// </summary>
    internal interface IDebugHelper
    {
        /// <summary>
        /// Starts a new timer with the given id
        /// </summary>
        /// <param name="id">Unique id of the timer.</param>
        void StartTimer(string id);

        /// <summary>
        /// Returns the time of the timer with the given id.
        /// </summary>
        /// <param name="id">Unique id of the timer.</param>
        /// <returns>The milliseconds since the timer was started.</returns>
        long GetTimer(string id);

        /// <summary>
        /// Stops the timer with the given id.
        /// </summary>
        /// <param name="id">Unique id of the timer.</param>
        /// <returns>The milliseconds v the timer was started.</returns>
        long StopTimer(string id);
    }
}