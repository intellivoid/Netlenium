using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Netlenium.Driver.WebDriver.Firefox.Internal
{
    /// <summary>
    /// Parses and reads an INI file.
    /// </summary>
    internal class IniFileReader
    {
        private Dictionary<string, Dictionary<string, string>> iniFileStore = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IniFileReader"/> class.
        /// </summary>
        /// <param name="fileName">The full path to the .INI file to be read.</param>
        public IniFileReader(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", "File name must not be null or empty");
            }

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("INI file not found", fileName);
            }

            var section = new Dictionary<string, string>();
            var sectionName = string.Empty;

            var iniFileContent = File.ReadAllLines(fileName);
            foreach (var iniFileLine in iniFileContent)
            {
                if (!string.IsNullOrEmpty(iniFileLine.Trim()) && !iniFileLine.StartsWith(";", StringComparison.OrdinalIgnoreCase))
                {
                    if (iniFileLine.StartsWith("[", StringComparison.OrdinalIgnoreCase) && iniFileLine.EndsWith("]", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.IsNullOrEmpty(sectionName))
                        {
                            iniFileStore.Add(sectionName, section);
                        }

                        sectionName = iniFileLine.Substring(1, iniFileLine.Length - 2).ToUpperInvariant();
                        section = new Dictionary<string, string>();
                    }
                    else
                    {
                        var entryParts = iniFileLine.Split(new char[] { '=' }, 2);
                        var name = entryParts[0].ToUpperInvariant();
                        var value = string.Empty;
                        if (entryParts.Length > 1)
                        {
                            value = entryParts[1];
                        }

                        section.Add(name, value);
                    }
                }
            }

            iniFileStore.Add(sectionName, section);
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> containing the names of the sections in the .INI file.
        /// </summary>
        public ReadOnlyCollection<string> SectionNames
        {
            get
            {
                var keyList = new List<string>(iniFileStore.Keys);
                return new ReadOnlyCollection<string>(keyList);
            }
        }

        /// <summary>
        /// Gets a value from the .INI file.
        /// </summary>
        /// <param name="sectionName">The section in which to find the key-value pair.</param>
        /// <param name="valueName">The key of the key-value pair.</param>
        /// <returns>The value associated with the given section and key.</returns>
        public string GetValue(string sectionName, string valueName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException("sectionName", "Section name cannot be null or empty");
            }

            var lowerCaseSectionName = sectionName.ToUpperInvariant();

            if (string.IsNullOrEmpty(valueName))
            {
                throw new ArgumentNullException("valueName", "Value name cannot be null or empty");
            }

            var lowerCaseValueName = valueName.ToUpperInvariant();

            if (!iniFileStore.ContainsKey(lowerCaseSectionName))
            {
                throw new ArgumentException("Section does not exist: " + sectionName, "sectionName");
            }

            var section = iniFileStore[lowerCaseSectionName];

            if (!section.ContainsKey(lowerCaseValueName))
            {
                throw new ArgumentException("Value does not exist: " + valueName, "valueName");
            }

            return section[lowerCaseValueName];
        }
    }
}
