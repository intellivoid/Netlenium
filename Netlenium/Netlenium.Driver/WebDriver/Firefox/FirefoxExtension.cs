using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;
using Newtonsoft.Json.Linq;
using Netlenium.Driver.WebDriver.Internal;



namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Provides the ability to install extensions into a <see cref="FirefoxProfile"/>.
    /// </summary>
    public class FirefoxExtension
    {
        private const string EmNamespaceUri = "http://www.mozilla.org/2004/em-rdf#";
        private const string RdfManifestFileName = "install.rdf";
        private const string JsonManifestFileName = "manifest.json";

        private string extensionFileName;
        private string extensionResourceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxExtension"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file containing the Firefox extension.</param>
        /// <remarks>WebDriver attempts to resolve the <paramref name="fileName"/> parameter
        /// by looking first for the specified file in the directory of the calling assembly,
        /// then using the full path to the file, if a full path is provided.</remarks>
        public FirefoxExtension(string fileName)
            : this(fileName, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxExtension"/> class.
        /// </summary>
        /// <param name="fileName">The name of the file containing the Firefox extension.</param>
        /// <param name="resourceId">The ID of the resource within the assembly containing the extension
        /// if the file is not present in the file system.</param>
        /// <remarks>WebDriver attempts to resolve the <paramref name="fileName"/> parameter
        /// by looking first for the specified file in the directory of the calling assembly,
        /// then using the full path to the file, if a full path is provided. If the file is
        /// not found in the file system, WebDriver attempts to locate a resource in the
        /// executing assembly with the name specified by the <paramref name="resourceId"/>
        /// parameter.</remarks>
        internal FirefoxExtension(string fileName, string resourceId)
        {
            extensionFileName = fileName;
            extensionResourceId = resourceId;
        }

        /// <summary>
        /// Installs the extension into a profile directory.
        /// </summary>
        /// <param name="profileDirectory">The Firefox profile directory into which to install the extension.</param>
        public void Install(string profileDirectory)
        {
            var info = new DirectoryInfo(profileDirectory);
            var stagingDirectoryName = Path.Combine(Path.GetTempPath(), info.Name + ".staging");
            var tempFileName = Path.Combine(stagingDirectoryName, Path.GetFileName(extensionFileName));
            if (Directory.Exists(tempFileName))
            {
                Directory.Delete(tempFileName, true);
            }

            // First, expand the .xpi archive into a temporary location.
            Directory.CreateDirectory(tempFileName);
            var zipFileStream = ResourceUtilities.GetResourceStream(extensionFileName, extensionResourceId);
            using (var extensionZipFile = ZipStorer.Open(zipFileStream, FileAccess.Read))
            {
                var entryList = extensionZipFile.ReadCentralDirectory();
                foreach (var entry in entryList)
                {
                    var localFileName = entry.FilenameInZip.Replace('/', Path.DirectorySeparatorChar);
                    var destinationFile = Path.Combine(tempFileName, localFileName);
                    extensionZipFile.ExtractFile(entry, destinationFile);
                }
            }

            // Then, copy the contents of the temporarly location into the
            // proper location in the Firefox profile directory.
            var id = GetExtensionId(tempFileName);
            var extensionDirectory = Path.Combine(Path.Combine(profileDirectory, "extensions"), id);
            if (Directory.Exists(extensionDirectory))
            {
                Directory.Delete(extensionDirectory, true);
            }

            Directory.CreateDirectory(extensionDirectory);
            FileUtilities.CopyDirectory(tempFileName, extensionDirectory);

            // By deleting the staging directory, we also delete the temporarily
            // expanded extension, which we copied into the profile.
            FileUtilities.DeleteDirectory(stagingDirectoryName);
        }

        private static string GetExtensionId(string root)
        {
            // Checks if manifest.json or install.rdf file exists and extracts
            // the addon/extenion id from the file accordingly
            var manifestJsonPath = Path.Combine(root, JsonManifestFileName);
            var installRdfPath = Path.Combine(root, RdfManifestFileName);

            if (File.Exists(installRdfPath))
            {
                return ReadIdFromInstallRdf(root);
            }

            if (File.Exists(manifestJsonPath))
            {
                return ReadIdFromManifestJson(root);
            }

            throw new WebDriverException("Extension should contain either install.rdf or manifest.json metadata file");
        }

        private static string ReadIdFromInstallRdf(string root)
        {
            string id = null;
            var installRdf = Path.Combine(root, "install.rdf");
            try
            {
                var rdfXmlDocument = new XmlDocument();
                rdfXmlDocument.Load(installRdf);

                var rdfNamespaceManager = new XmlNamespaceManager(rdfXmlDocument.NameTable);
                rdfNamespaceManager.AddNamespace("em", EmNamespaceUri);
                rdfNamespaceManager.AddNamespace("RDF", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");

                var node = rdfXmlDocument.SelectSingleNode("//em:id", rdfNamespaceManager);
                if (node == null)
                {
                    var descriptionNode = rdfXmlDocument.SelectSingleNode("//RDF:Description", rdfNamespaceManager);
                    var attribute = descriptionNode.Attributes["id", EmNamespaceUri];
                    if (attribute == null)
                    {
                        throw new WebDriverException("Cannot locate node containing extension id: " + installRdf);
                    }

                    id = attribute.Value;
                }
                else
                {
                    id = node.InnerText;
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new FileNotFoundException("Cannot install extension with ID: " + id);
                }
            }
            catch (Exception e)
            {
                throw new WebDriverException("Error installing extension", e);
            }

            return id;
        }

        private static string ReadIdFromManifestJson(string root)
        {
            string id = null;
            var manifestJsonPath = Path.Combine(root, JsonManifestFileName);
            var manifestObject = JObject.Parse(File.ReadAllText(manifestJsonPath));
            if (manifestObject["applications"] != null)
            {
                var applicationObject = manifestObject["applications"];
                if (applicationObject["gecko"] != null)
                {
                    var geckoObject = applicationObject["gecko"];
                    if (geckoObject["id"] != null)
                    {
                        id = geckoObject["id"].ToString().Trim();
                    }
                }
            }

            if (string.IsNullOrEmpty(id))
            {
                var addInName = manifestObject["name"].ToString().Replace(" ", "");
                var addInVersion = manifestObject["version"].ToString();
                id = string.Format(CultureInfo.InvariantCulture, "{0}@{1}", addInName, addInVersion);
            }

            return id;
        }
    }
}
