using System;
using System.IO;
using System.Text;

namespace Netlenium.Logging
{
    /// <summary>
    /// File Logging Class
    /// </summary>
    public static class FileLogging
    {
        /// <summary>
        /// Returns the Log's file path for the service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string GetLoggingFilepath(string serviceName)
        {
            var FileName = string.Format("{0}-{1}.log", serviceName, DateTime.Today.ToString("dd-MM-yyyy"));
            return $"{ApplicationPaths.LoggingDirectory}{Path.DirectorySeparatorChar}{FileName}";
        }

        /// <summary>
        /// Writes an entry to the logging file
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="serviceName"></param>
        /// <param name="moduleName"></param>
        /// <param name="entry"></param>
        public static void WriteEntry(MessageType messageType, string serviceName, string moduleName, string entry)
        {
            StringBuilder Output = new StringBuilder();
            Output.Append($"[{DateTime.Now.ToString("hh:mm:ss tt")}]");

            switch (messageType)
            {
                case MessageType.Debugging:
                    Output.Append(" {DEBUG} ");
                    break;

                case MessageType.Error:
                    Output.Append(" {ERROR} ");
                    break;

                case MessageType.Information:
                    Output.Append(" {INFO}  ");
                    break;

                case MessageType.Verbose:
                    Output.Append(" {VERBO} ");
                    break;

                case MessageType.Warning:
                    Output.Append(" {WARN}  ");
                    break;
            }

            Output.Append($"=> {serviceName}.{moduleName} ::   {entry}{Environment.NewLine}");
            File.AppendAllText(GetLoggingFilepath(serviceName), Output.ToString());
        }

    }
}
