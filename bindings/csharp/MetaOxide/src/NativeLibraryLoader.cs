using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

namespace MetaOxide
{
    /// <summary>
    /// Handles dynamic loading of the native MetaOxide library.
    /// </summary>
    /// <remarks>
    /// This class automatically detects the platform and loads the appropriate
    /// native library (DLL on Windows, SO on Linux, DYLIB on macOS).
    /// It supports both NuGet package deployment and local development scenarios.
    /// </remarks>
    internal static class NativeLibraryLoader
    {
        private static bool _isLoaded;
        private static readonly object _lock = new object();

        /// <summary>
        /// Ensures the native library is loaded. This method is idempotent.
        /// </summary>
        /// <exception cref="DllNotFoundException">Thrown when the native library cannot be found or loaded</exception>
        /// <exception cref="PlatformNotSupportedException">Thrown when the current platform is not supported</exception>
        public static void EnsureLoaded()
        {
            if (_isLoaded)
                return;

            lock (_lock)
            {
                if (_isLoaded)
                    return;

                try
                {
                    LoadNativeLibrary();
                    _isLoaded = true;
                }
                catch (Exception ex)
                {
                    throw new DllNotFoundException(
                        $"Failed to load native MetaOxide library. {ex.Message}\n" +
                        $"Platform: {GetRuntimeIdentifier()}\n" +
                        $"Library name: {GetLibraryName()}\n" +
                        "Please ensure the native library is present in the appropriate runtime directory.",
                        ex);
                }
            }
        }

        private static void LoadNativeLibrary()
        {
            string rid = GetRuntimeIdentifier();
            string libraryName = GetLibraryName();

            // Try to load from multiple locations in order of preference
            string? libraryPath = null;

            // 1. Try NuGet package runtime directory (most common for deployed apps)
            string nugetPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                "runtimes",
                rid,
                "native",
                libraryName);

            if (File.Exists(nugetPath))
            {
                libraryPath = nugetPath;
            }

            // 2. Try local native directory (for development)
            if (libraryPath == null)
            {
                string localPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                    "native",
                    rid,
                    libraryName);

                if (File.Exists(localPath))
                {
                    libraryPath = localPath;
                }
            }

            // 3. Try assembly directory directly (for simple deployments)
            if (libraryPath == null)
            {
                string directPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                    libraryName);

                if (File.Exists(directPath))
                {
                    libraryPath = directPath;
                }
            }

            // 4. Try system library paths (Linux/macOS)
            if (libraryPath == null && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Let the system loader find it in standard locations
                libraryPath = libraryName;
            }

            if (libraryPath == null)
            {
                throw new FileNotFoundException(
                    $"Native library '{libraryName}' not found in any expected location.\n" +
                    $"Searched paths:\n" +
                    $"  - {nugetPath}\n" +
                    $"  - {Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "native", rid, libraryName)}\n" +
                    $"  - {Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", libraryName)}");
            }

            // Load the library
            if (RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
            {
                // .NET Framework: Use LoadLibrary
                LoadLibraryWindows(libraryPath);
            }
            else
            {
                // .NET Core/.NET 5+: Use NativeLibrary.Load
                LoadLibraryModern(libraryPath);
            }
        }

        private static void LoadLibraryModern(string libraryPath)
        {
#if NET5_0_OR_GREATER
            IntPtr handle = NativeLibrary.Load(libraryPath);
            if (handle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Failed to load library from: {libraryPath}");
            }
#else
            // For .NET Standard 2.0, we rely on DllImport's default loading mechanism
            // The library must be in the same directory as the assembly or in system paths
            if (!File.Exists(libraryPath))
            {
                throw new FileNotFoundException($"Library not found: {libraryPath}");
            }
#endif
        }

        private static void LoadLibraryWindows(string libraryPath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IntPtr handle = WindowsLoadLibrary(libraryPath);
                if (handle == IntPtr.Zero)
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new DllNotFoundException(
                        $"Failed to load library from: {libraryPath}. " +
                        $"Windows error code: {errorCode}");
                }
            }
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        private static IntPtr WindowsLoadLibrary(string libraryPath)
        {
            return LoadLibrary(libraryPath);
        }

        /// <summary>
        /// Gets the runtime identifier (RID) for the current platform.
        /// </summary>
        private static string GetRuntimeIdentifier()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => "win-x64",
                    Architecture.X86 => "win-x86",
                    Architecture.Arm64 => "win-arm64",
                    _ => throw new PlatformNotSupportedException(
                        $"Unsupported Windows architecture: {RuntimeInformation.ProcessArchitecture}")
                };
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => "linux-x64",
                    Architecture.Arm64 => "linux-arm64",
                    Architecture.Arm => "linux-arm",
                    _ => throw new PlatformNotSupportedException(
                        $"Unsupported Linux architecture: {RuntimeInformation.ProcessArchitecture}")
                };
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X64 => "osx-x64",
                    Architecture.Arm64 => "osx-arm64",
                    _ => throw new PlatformNotSupportedException(
                        $"Unsupported macOS architecture: {RuntimeInformation.ProcessArchitecture}")
                };
            }

            throw new PlatformNotSupportedException(
                $"Unsupported operating system: {RuntimeInformation.OSDescription}");
        }

        /// <summary>
        /// Gets the platform-specific library file name.
        /// </summary>
        private static string GetLibraryName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "meta_oxide.dll";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "libmeta_oxide.so";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "libmeta_oxide.dylib";
            }

            throw new PlatformNotSupportedException(
                $"Unsupported operating system: {RuntimeInformation.OSDescription}");
        }

        /// <summary>
        /// Gets diagnostic information about the current platform and library loading.
        /// </summary>
        public static string GetDiagnosticInfo()
        {
            return $"Platform: {RuntimeInformation.OSDescription}\n" +
                   $"Architecture: {RuntimeInformation.ProcessArchitecture}\n" +
                   $"Framework: {RuntimeInformation.FrameworkDescription}\n" +
                   $"Runtime Identifier: {GetRuntimeIdentifier()}\n" +
                   $"Library Name: {GetLibraryName()}\n" +
                   $"Assembly Location: {Assembly.GetExecutingAssembly().Location}\n" +
                   $"Library Loaded: {_isLoaded}";
        }
    }
}
