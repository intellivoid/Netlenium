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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ProgramText {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ProgramText() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("NetleniumServer.ProgramText", typeof(ProgramText).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static string InavlidAuthPasswordOption {
            get {
                return ResourceManager.GetString("InavlidAuthPasswordOption", resourceCulture);
            }
        }
        
        public static string SessionInactivityLimitInvalidValue_Greater {
            get {
                return ResourceManager.GetString("SessionInactivityLimitInvalidValue_Greater", resourceCulture);
            }
        }
        
        public static string SessionInactivityLimitInvalidValue_Less {
            get {
                return ResourceManager.GetString("SessionInactivityLimitInvalidValue_Less", resourceCulture);
            }
        }
        
        public static string ServerNameInvalidOption_Empty {
            get {
                return ResourceManager.GetString("ServerNameInvalidOption_Empty", resourceCulture);
            }
        }
        
        public static string ProgramTitle {
            get {
                return ResourceManager.GetString("ProgramTitle", resourceCulture);
            }
        }
        
        public static string ServerLoggingLevelInvalidOption {
            get {
                return ResourceManager.GetString("ServerLoggingLevelInvalidOption", resourceCulture);
            }
        }
        
        public static string DriverLoggingLevelInvalidOption {
            get {
                return ResourceManager.GetString("DriverLoggingLevelInvalidOption", resourceCulture);
            }
        }
        
        public static string MaxSessionsInvalidOption_Less {
            get {
                return ResourceManager.GetString("MaxSessionsInvalidOption_Less", resourceCulture);
            }
        }
        
        public static string MaxSessionsInvalidOption_Greater {
            get {
                return ResourceManager.GetString("MaxSessionsInvalidOption_Greater", resourceCulture);
            }
        }
    }
}
