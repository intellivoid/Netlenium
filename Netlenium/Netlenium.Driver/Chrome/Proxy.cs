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
        
        public bool AuthenticationRequired { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ProxyScheme Scheme { get; set; }

        private readonly Driver driver;

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
            AuthenticationRequired = false;
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

            var chromeExtensionTemplatePath = $"{Utilities.AssemblyDirectory}{Path.DirectorySeparatorChar}extensions{Path.DirectorySeparatorChar}chrome";
            var files = new List<string>
            {
                $"{chromeExtensionTemplatePath}{Path.DirectorySeparatorChar}background.js",
                $"{chromeExtensionTemplatePath}{Path.DirectorySeparatorChar}manifest.json"
            };

            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Extension Template Path: {chromeExtensionTemplatePath}");
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Required File: {files[0]}");
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Required File: {files[1]}");

            if (Directory.Exists(chromeExtensionTemplatePath) == false)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Proxy", $"The directory '{chromeExtensionTemplatePath}' does not exist");
                throw new DirectoryNotFoundException(chromeExtensionTemplatePath);
            }

            if(File.Exists(files[0]) == false)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Proxy", $"The file '{files[0]}' does not exist");
                throw new FileNotFoundException(files[0]);
            }

            if(File.Exists(files[1]) == false)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Error, "Proxy", $"The file '{files[1]}' does not exist");
                throw new FileNotFoundException(files[1]);
            }

            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var extensionId = new string(Enumerable.Repeat(chars, 16).Select(s => s[random.Next(s.Length)]).ToArray());
            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Generated Extension ID: '{extensionId}'");
            
            var extensionDirectory = $"{ApplicationPaths.TemporaryDirectory}{Path.DirectorySeparatorChar}{extensionId}";
            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Proxy", $"Creating directory '{extensionId}'");
            Directory.CreateDirectory(extensionDirectory);

            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Proxy", "Building Files");
            var fileContents = new List<string>
            {
                File.ReadAllText(files[0]),
                File.ReadAllText(files[1])
            };

            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%HOST%' with '{Host}'");
            fileContents[0] = fileContents[0].Replace("%HOST%", Host);

            driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%PORT%' with '{Port}'");
            fileContents[0] = fileContents[0].Replace("%PORT%", Port.ToString());

            switch(Scheme)
            {
                case ProxyScheme.HTTP:
                    driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%SCHEME%' with 'http'");
                    fileContents[0] = fileContents[0].Replace("%SCHEME%", "http");
                    break;

                case ProxyScheme.HTTPS:
                    driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%SCHEME%' with 'https'");
                    fileContents[0] = fileContents[0].Replace("%SCHEME%", "https");
                    break;

                default:
                    driver.Logging.WriteEntry(Logging.MessageType.Error, "Proxy", $"The scheme '{Scheme.ToString()}' is not supported");
                    throw new UnsupportedSchemeException();
            }

            if (AuthenticationRequired)
            {
                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%USERNAME%' with '{Username}'");
                fileContents[0] = fileContents[0].Replace("%USERNAME%", Username);

                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%PASSWORD%' with '{Password}'");
                fileContents[0] = fileContents[0].Replace("%PASSWORD%", Password);
            }
            else
            {
                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%USERNAME%' with 'None'");
                fileContents[0] = fileContents[0].Replace("%USERNAME%", "None");

                driver.Logging.WriteEntry(Logging.MessageType.Debugging, "Proxy", $"Replacing '%PASSWORD%' with 'None'");
                fileContents[0] = fileContents[0].Replace("%PASSWORD%", "None");
            }

            var outputFiles = new List<string>
            {
                $"{extensionDirectory}{Path.DirectorySeparatorChar}background.js",
                $"{extensionDirectory}{Path.DirectorySeparatorChar}manifest.json"
            };

            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Proxy", $"Writing '{outputFiles[0]}' ...");
            File.WriteAllText(outputFiles[0], fileContents[0]);

            driver.Logging.WriteEntry(Logging.MessageType.Verbose, "Proxy", $"Writing '{outputFiles[1]}' ...");
            File.WriteAllText(outputFiles[1], fileContents[1]);

            driver.Logging.WriteEntry(Logging.MessageType.Information, "Proxy", "Extension created");

            return extensionDirectory;
        }
    }
}
