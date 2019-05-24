namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Class containing names of common log types.
    /// </summary>
    public static class LogType
    {
        /// <summary>
        /// Log messages from the client language bindings.
        /// </summary>
        public static readonly string Client = "client";

        /// <summary>
        /// Logs from the current WebDriver instance.
        /// </summary>
        public static readonly string Driver = "driver";

        /// <summary>
        /// Logs from the browser.
        /// </summary>
        public static readonly string Browser = "browser";

        /// <summary>
        /// Logs from the server.
        /// </summary>
        public static readonly string Server = "server";

        /// <summary>
        /// Profiling logs.
        /// </summary>
        public static readonly string Profiler = "profiler";
    }
}
