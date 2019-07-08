using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Netlenium.Driver.Chrome
{
    public class Proxy : IProxy
    {
        public bool Enabled { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool AuthenticationRequried { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ProxyScheme Scheme { get; set; }

        private Driver driver;

        /// <summary>
        /// Public Constructor
        /// </summary>
        /// <param name="driver"></param>
        public Proxy(Driver driver)
        {
            this.driver = driver;
            Enabled = false;
            Host = "0.0.0.0";
            Port = 8080;
            AuthenticationRequried = false;
            Username = string.Empty;
            Password = string.Empty;
        }

        /// <summary>
        /// Builds the Chrome Extension for using this proxy
        /// </summary>
        /// <returns>The directory path of the generated extension</returns>
        public string BuildExtension()
        {
            driver.Logging.WriteEntry(Logging.MessageType.Information, "Proxy", "Creating extension for proxy");

            var ChromeExtensionTemplatePath = $"{Utilities.AssemblyDirectory}{Path.DirectorySeparatorChar}extensions{Path.DirectorySeparatorChar}chrome";
            var Files = new List<string>
            {
                $"{ChromeExtensionTemplatePath}{Path.DirectorySeparatorChar}background.js",
                $"{ChromeExtensionTemplatePath}{Path.DirectorySeparatorChar}manifest.json"
            };

            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Extension Template Path: {ChromeExtensionTemplatePath}");
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Required File: {Files[0]}");
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Required File: {Files[1]}");

            if (Directory.Exists(ChromeExtensionTemplatePath) == false)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Proxy", $"The directory '{ChromeExtensionTemplatePath}' does not exist");
                throw new DirectoryNotFoundException(ChromeExtensionTemplatePath);
            }

            if(File.Exists(Files[0]) == false)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Proxy", $"The file '{Files[0]}' does not exist");
                throw new FileNotFoundException(Files[0]);
            }

            if(File.Exists(Files[1]) == false)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Proxy", $"The file '{Files[1]}' does not exist");
                throw new FileNotFoundException(Files[1]);
            }

            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var extensionId = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Generated Extension ID: '{extensionId}'");
            
            var extensionDirectory = $"{ApplicationPaths.TemporaryDirectory}{Path.DirectorySeparatorChar}{extensionId}";
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Proxy", $"Creating directory '{extensionId}'");
            Directory.CreateDirectory(extensionDirectory);

            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Proxy", "Building Files");
            var FileContents = new List<string>
            {
                File.ReadAllText(Files[0]),
                File.ReadAllText(Files[1])
            };

            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%HOST%' with '{Host}'");
            FileContents[0] = FileContents[0].Replace("%HOST%", Host);

            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%PORT%' with '{Port}'");
            FileContents[0] = FileContents[0].Replace("%PORT%", Host);

            switch(Scheme)
            {
                case ProxyScheme.HTTP:
                    driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%SCHEME%' with 'http'");
                    FileContents[0] = FileContents[0].Replace("%SCHEME%", "http");
                    break;

                case ProxyScheme.HTTPS:
                    driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%SCHEME%' with 'https'");
                    FileContents[0] = FileContents[0].Replace("%SCHEME%", "https");
                    break;

                default:
                    driver.Logging.WriteEntry(Logging.MessageType.Error, "Proxy", $"The scheme '{Scheme.ToString()}' is not supported");
                    throw new UnsupportedSchemeException();
            }

            if (AuthenticationRequried == true)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%USERNAME%' with '{Username}'");
                FileContents[0] = FileContents[0].Replace("%USERNAME%", Username);

                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%PASSWORD%' with '{Password}'");
                FileContents[0] = FileContents[0].Replace("%PASSWORD%", Password);
            }
            else
            {
                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%USERNAME%' with 'None'");
                FileContents[0] = FileContents[0].Replace("%USERNAME%", "None");

                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%PASSWORD%' with 'None'");
                FileContents[0] = FileContents[0].Replace("%PASSWORD%", "None");
            }

            var OutputFiles = new List<string>
            {
                $"{extensionDirectory}{Path.DirectorySeparatorChar}background.js",
                $"{extensionDirectory}{Path.DirectorySeparatorChar}manifest.json"
            };

            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Proxy", $"Writing '{OutputFiles[0]}' ...");
            File.WriteAllText(OutputFiles[0], FileContents[0]);

            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Proxy", $"Writing '{OutputFiles[1]}' ...");
            File.WriteAllText(OutputFiles[1], FileContents[1]);

            driver.Logging.WriteEntry(Logging.MessageType.Information, "Proxy", "Extension created");

            return extensionDirectory;
        }
    }
}
