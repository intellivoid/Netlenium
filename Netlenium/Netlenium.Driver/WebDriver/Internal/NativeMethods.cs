using System;
using System.Runtime.InteropServices;

namespace Netlenium.Driver.WebDriver
{
    /// <summary>
    /// Provides entry points into needed unmanaged APIs.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Values for flags for setting information about a native operating system handle.
        /// </summary>
        [Flags]
        internal enum HandleInformation
        {
            /// <summary>
            /// No flags are to be set for the handle.
            /// </summary>
            None = 0,

            /// <summary>
            /// If this flag is set, a child process created with the bInheritHandles
            /// parameter of CreateProcess set to TRUE will inherit the object handle.
            /// </summary>
            Inherit = 1,

            /// <summary>
            /// If this flag is set, calling the CloseHandle function will not close the
            /// object handle.
            /// </summary>
            ProtectFromClose = 2
        }

        /// <summary>
        /// Sets the handle information for a Windows object.
        /// </summary>
        /// <param name="hObject">Handle to the object.</param>
        /// <param name="dwMask">The handle information to set.</param>
        /// <param name="dwFlags">The flags for the handle.</param>
        /// <returns><see langword="true"/> if the information is set; otherwise <see langword="false"/>.</returns>
        [DllImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetHandleInformation(IntPtr hObject, HandleInformation dwMask, HandleInformation dwFlags);
    }
}
