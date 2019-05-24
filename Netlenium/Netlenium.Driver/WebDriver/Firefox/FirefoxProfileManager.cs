using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Netlenium.Driver.WebDriver.Firefox.Internal;

namespace Netlenium.Driver.WebDriver.Firefox
{
    /// <summary>
    /// Allows the user to enumerate and access existing named Firefox profiles.
    /// </summary>
    public class FirefoxProfileManager
    {
        private Dictionary<string, string> profiles = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FirefoxProfileManager"/> class.
        /// </summary>
        public FirefoxProfileManager()
        {
            var appDataDirectory = GetApplicationDataDirectory();
            ReadProfiles(appDataDirectory);
        }

        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> containing <see cref="FirefoxProfile">FirefoxProfiles</see>
        /// representing the existing named profiles for Firefox.
        /// </summary>
        public ReadOnlyCollection<string> ExistingProfiles
        {
            get
            {
                var profileList = new List<string>(profiles.Keys);
                return profileList.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a <see cref="FirefoxProfile"/> with a given name.
        /// </summary>
        /// <param name="profileName">The name of the profile to get.</param>
        /// <returns>A <see cref="FirefoxProfile"/> with a given name.
        /// Returns <see langword="null"/> if no profile with the given name exists.</returns>
        public FirefoxProfile GetProfile(string profileName)
        {
            FirefoxProfile profile = null;
            if (!string.IsNullOrEmpty(profileName))
            {
                if (profiles.ContainsKey(profileName))
                {
                    profile = new FirefoxProfile(profiles[profileName]);
                    if (profile.Port == 0)
                    {
                        profile.Port = FirefoxDriver.DefaultPort;
                    }
                }
            }

            return profile;
        }

        private static string GetApplicationDataDirectory()
        {
            var appDataDirectory = string.Empty;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    appDataDirectory = Path.Combine(".mozilla", "firefox");
                    break;

                case PlatformID.MacOSX:
                    appDataDirectory = Path.Combine("Library", Path.Combine("Application Support", "Firefox"));
                    break;

                default:
                    appDataDirectory = Path.Combine("Mozilla", "Firefox");
                    break;
            }

            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appDataDirectory);
        }

        private void ReadProfiles(string appDataDirectory)
        {
            var profilesIniFile = Path.Combine(appDataDirectory, "profiles.ini");
            if (File.Exists(profilesIniFile))
            {
                var reader = new IniFileReader(profilesIniFile);
                var sectionNames = reader.SectionNames;
                foreach (var sectionName in sectionNames)
                {
                    if (sectionName.StartsWith("profile", StringComparison.OrdinalIgnoreCase))
                    {
                        var name = reader.GetValue(sectionName, "name");
                        var isRelative = reader.GetValue(sectionName, "isrelative") == "1";
                        var profilePath = reader.GetValue(sectionName, "path");
                        var fullPath = string.Empty;
                        if (isRelative)
                        {
                            fullPath = Path.Combine(appDataDirectory, profilePath);
                        }
                        else
                        {
                            fullPath = profilePath;
                        }

                        profiles.Add(name, fullPath);
                    }
                }
            }
        }
    }
}
