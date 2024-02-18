using System;
using System.Runtime.InteropServices;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// Small helper for determining the current platform
    /// </summary>
    public class PlatformHelper
    {
        /// <summary>
        /// Returns true if running on Windows, false otherwise
        /// </summary>
        public static bool IsWindows
        {
            get => Environment.OSVersion.Platform == PlatformID.Win32NT;
        }

        /// <summary>
        /// Returns true if running on Linux, false otherwise
        /// </summary>
        public static bool IsLinux
        {
            get => Environment.OSVersion.Platform == PlatformID.Unix;
        }

        /// <summary>
        /// Returns true if running on Linux desktop, false otherwise
        /// </summary>
        public static bool IsLinuxDesktop
        {
            get => IsLinux;
        }

        /// <summary>
        /// Returns true if running on macOS, false otherwise
        /// </summary>
        public static bool IsMac
        {
            get => false;
        }

        /// <summary>
        /// Returns true if running in 64bit process, false otherwise
        /// </summary>
        public static bool IsX64BitProcess => IntPtr.Size == 8;
    }
}
