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
        private static string GetLoggingFilepath(string serviceName)
        {
            var fileName = $"{serviceName}-{DateTime.Today:dd-MM-yyyy}.log";
            return $"{ApplicationPaths.LoggingDirectory}{Path.DirectorySeparatorChar}{fileName}";
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
            var output = new StringBuilder();
            output.Append($"[{DateTime.Now:hh:mm:ss tt}]");

            switch (messageType)
            {
                case MessageType.Debugging:
                    output.Append(" {DEBUG} ");
                    break;

                case MessageType.Error:
                    output.Append(" {ERROR} ");
                    break;

                case MessageType.Information:
                    output.Append(" {INFO}  ");
                    break;

                case MessageType.Verbose:
                    output.Append(" {VERBO} ");
                    break;

                case MessageType.Warning:
                    output.Append(" {WARN}  ");
                    break;
             
                default:
                    output.Append(" {????}  ");
                    break;
            }

            output.Append($"=> {serviceName}.{moduleName} ::   {entry}{Environment.NewLine}");
            var file_name = GetLoggingFilepath(serviceName);

            try
            {
                File.AppendAllText(file_name, output.ToString());
            }
            catch(IOException io_ex)
            {
                Console.WriteLine(string.Format("Warning: Cannot write to file '{0}' due to IO error; {1}", file_name, io_ex.Message));
            }
        }

    }
}
