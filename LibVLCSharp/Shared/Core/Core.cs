using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using LibVLCSharp.Shared.Helpers;

namespace LibVLCSharp.Shared
{
    /// <summary>
    /// The Core class handles libvlc loading intricacies on various platforms as well as
    /// the libvlc/libvlcsharp version match check.
    /// </summary>
    public static partial class Core
    {
        partial struct Native
        {
#if !UWP10_0 && !NETSTANDARD1_1
            [DllImport(Constants.LibraryName, CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "libvlc_get_version")]
            internal static extern IntPtr LibVLCVersion();
#endif
            [DllImport(Constants.Kernel32, SetLastError = true, CharSet = CharSet.Unicode)]
            internal static extern IntPtr LoadLibraryW(string dllToLoad);

            [DllImport(Constants.LibSystem, EntryPoint = "dlopen")]
            internal static extern IntPtr Dlopen(string libraryPath, int mode = 1);
        }

#if !UWP10_0 && !NETSTANDARD1_1
        /// <summary>
        /// Checks whether the major version of LibVLC and LibVLCSharp match <para/>
        /// Throws a VLCException if the major versions mismatch
        /// </summary>
        static void EnsureVersionsMatch()
        {
            var libvlcMajorVersion = int.Parse(Native.LibVLCVersion().FromUtf8()?.Split('.').FirstOrDefault() ?? "0");
            var libvlcsharpMajorVersion = Assembly.GetExecutingAssembly().GetName().Version?.Major;
            if (libvlcMajorVersion != libvlcsharpMajorVersion)
                throw new VLCException($"Version mismatch between LibVLC {libvlcMajorVersion} and LibVLCSharp {libvlcsharpMajorVersion}. " +
                    $"They must share the same major version number");
        }

#endif
        static string LibraryExtension => PlatformHelper.IsWindows ? Constants.WindowsLibraryExtension : Constants.MacLibraryExtension;
        static void Log(string message)
        {
            Trace.WriteLine(message);
        }

        static bool _libvlcLoaded;
        static internal bool LibVLCLoaded
        {
            get => _libvlcLoaded || LibvlcHandle != IntPtr.Zero;
        }

        static IntPtr LibvlccoreHandle;
        static IntPtr LibvlcHandle;

        static void LoadLibVLC()
        {
            // full path to directory location of libvlc and libvlccore has been provided
            bool loadResult;
            var libvlccorePath = "libvlccore.dll";
            loadResult = LoadNativeLibrary(libvlccorePath, out LibvlccoreHandle);
            if (!loadResult)
            {
                Log($"Failed to load required native libraries at {libvlccorePath}");
                return;
            }

            var libvlcPath = "libvlc.dll";
            loadResult = LoadNativeLibrary(libvlcPath, out LibvlcHandle);
            if (!loadResult)
            {
                Log($"Failed to load required native libraries at {libvlcPath}");
                return;
            }

            if (!LibVLCLoaded)
            {
                throw new VLCException("Failed to load required native libraries. " +
                    $"{Environment.NewLine}Have you installed the latest LibVLC package from nuget for your target platform?");
            }

            _libvlcLoaded = true;
        }

        internal static void EnsureLoaded()
        {
            if (LibVLCLoaded)
            {
                return;
            }
            else
            {
                LoadLibVLC();
            }

        }

        static bool LoadNativeLibrary(string nativeLibraryPath, out IntPtr handle)
        {
            handle = IntPtr.Zero;
            Log($"Loading {nativeLibraryPath}");

            if (!File.Exists(nativeLibraryPath))
            {
                Log($"Cannot find {nativeLibraryPath}");
                return false;
            }

            return handle != IntPtr.Zero;
        }
    }
}
