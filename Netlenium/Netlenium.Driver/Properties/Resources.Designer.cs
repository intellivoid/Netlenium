﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Netlenium.Driver.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Netlenium.Driver.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot delete &apos;{0}&apos;, {1}.
        /// </summary>
        internal static string ErrorCannotDeleteFile {
            get {
                return ResourceManager.GetString("ErrorCannotDeleteFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function(){return function(){var k=this;function l(a){return void 0!==a}function m(a){return&quot;string&quot;==typeof a}function aa(a,b){a=a.split(&quot;.&quot;);var c=k;a[0]in c||!c.execScript||c.execScript(&quot;var &quot;+a[0]);for(var d;a.length&amp;&amp;(d=a.shift());)!a.length&amp;&amp;l(b)?c[d]=b:c[d]&amp;&amp;c[d]!==Object.prototype[d]?c=c[d]:c=c[d]={}}
        ///function ba(a){var b=typeof a;if(&quot;object&quot;==b)if(a){if(a instanceof Array)return&quot;array&quot;;if(a instanceof Object)return b;var c=Object.prototype.toString.call(a);if(&quot;[object Window]&quot;==c)return&quot;object&quot;;if( [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string isDisplayed {
            get {
                return ResourceManager.GetString("isDisplayed", resourceCulture);
            }
        }
    }
}
