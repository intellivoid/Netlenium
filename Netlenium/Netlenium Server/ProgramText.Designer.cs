﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetleniumServer {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ProgramText {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ProgramText() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NetleniumServer.ProgramText", typeof(ProgramText).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;driver-logging-level&apos; must have a value between 0-3.
        /// </summary>
        public static string DriverLoggingLevelInvalidOption {
            get {
                return ResourceManager.GetString("DriverLoggingLevelInvalidOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to  USAGE:
        ///     netlenium [OPTIONS]
        ///         Starts the Netlenium Server on the default port 6410
        ///
        ///     netlenium --update [OPTIONS]
        ///         Fetches missing Driver Resources and or updates outdated
        ///         resources then exits
        ///
        /// OPTIONS:
        ///     -h, --help
        ///         Displays the help menu and exits
        ///
        ///     --disable-stdout
        ///         Disables standard output
        ///            
        ///     --disable-file-logging
        ///         Disables logging to files
        ///            
        ///     --driver-logging-level [0-3]
        ///         Logging l [rest of string was truncated]&quot;;.
        /// </summary>
        public static string HelpMenu {
            get {
                return ResourceManager.GetString("HelpMenu", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;auth-password&apos; is invalid, the password must be greater than 6 characters.
        /// </summary>
        public static string InavlidAuthPasswordOption {
            get {
                return ResourceManager.GetString("InavlidAuthPasswordOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;max-sessions&apos; cannot have a value that&apos;s greater than 99999.
        /// </summary>
        public static string MaxSessionsInvalidOption_Greater {
            get {
                return ResourceManager.GetString("MaxSessionsInvalidOption_Greater", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;max-sessions&apos; cannot have a value that&apos;s less than 1.
        /// </summary>
        public static string MaxSessionsInvalidOption_Less {
            get {
                return ResourceManager.GetString("MaxSessionsInvalidOption_Less", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} - {1}.
        /// </summary>
        public static string ProgramServerEndpointTitle {
            get {
                return ResourceManager.GetString("ProgramServerEndpointTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Netlenium Server.
        /// </summary>
        public static string ProgramServerTitle {
            get {
                return ResourceManager.GetString("ProgramServerTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Netlenium.
        /// </summary>
        public static string ProgramTitle {
            get {
                return ResourceManager.GetString("ProgramTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;server-logging-level&apos; must have a value between 0-3.
        /// </summary>
        public static string ServerLoggingLevelInvalidOption {
            get {
                return ResourceManager.GetString("ServerLoggingLevelInvalidOption", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;server-name&apos; cannot be empty.
        /// </summary>
        public static string ServerNameInvalidOption_Empty {
            get {
                return ResourceManager.GetString("ServerNameInvalidOption_Empty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;session-inactivity-limit&apos; cannot have a value that&apos;s greater than 99999.
        /// </summary>
        public static string SessionInactivityLimitInvalidValue_Greater {
            get {
                return ResourceManager.GetString("SessionInactivityLimitInvalidValue_Greater", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The parameter &apos;session-inactivity-limit&apos; cannot have a value that&apos;s less than 0.
        /// </summary>
        public static string SessionInactivityLimitInvalidValue_Less {
            get {
                return ResourceManager.GetString("SessionInactivityLimitInvalidValue_Less", resourceCulture);
            }
        }
    }
}
