using System;
using System.Runtime.InteropServices;

namespace Sedulous.Core
{
    /// <summary>
    /// Contains information relating to the current platform.
    /// </summary>
    public static class FrameworkPlatformInfo
    {
        /// <summary>
        /// Initializes the <see cref="FrameworkPlatformInfo"/> type.
        /// </summary>
        static FrameworkPlatformInfo()
        {
            CurrentPlatform = DetectCurrentPlatform(out var machineHardwareName);
            CurrentPlatformMachineHardwareName = machineHardwareName;
            CurrentPlatformVersion = Environment.OSVersion.VersionString;
            CurrentRuntime = DetectCurrentRuntime();
            CurrentRuntimeVersion = DetectCurrentRuntimeVersion();
        }
        
        /// <summary>
        /// Gets a value indicating whether runtime code generation is supported on the current platform.
        /// </summary>
        /// <returns><see langword="true"/> if the current platform supports runtime code generation; otherwise, <see langword="false"/>.</returns>
        public static Boolean IsRuntimeCodeGenerationSupported()
        {
            return IsRuntimeCodeGenerationSupported(CurrentPlatform);
        }

        /// <summary>
        /// Gets a value indicating whether runtime code generation is supported on the specified platform.
        /// </summary>
        /// <param name="platform">The platform to evaluate.</param>
        /// <returns><see langword="true"/> if the specified platform supports runtime code generation; otherwise, <see langword="false"/>.</returns>
        public static Boolean IsRuntimeCodeGenerationSupported(FrameworkPlatform platform)
        {
            if (platform == FrameworkPlatform.iOS || platform == FrameworkPlatform.Android)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the version number associated with the executing runtime.
        /// </summary>
        public static Version CurrentRuntimeVersion { get; }

        /// <summary>
        /// Gets an <see cref="FrameworkRuntime"/> value indicating which of Sedulous's supported
        /// runtimes is currently executing this application.
        /// </summary>
        public static FrameworkRuntime CurrentRuntime { get; }

        /// <summary>
        /// Gets an <see cref="FrameworkPlatform"/> value indicating which of Sedulous's supported
        /// platforms is currently executing this application.
        /// </summary>
        public static FrameworkPlatform CurrentPlatform { get; }

        /// <summary>
        /// Gets the string which contains the machine hardware name for the current platform.
        /// </summary>
        public static String CurrentPlatformMachineHardwareName { get; }

        /// <summary>
        /// Gets the string which contains the version information for the current platform.
        /// </summary>
        public static String CurrentPlatformVersion { get; }

        /// <summary>
        /// Attempts to detect the version number of the current runtime.
        /// </summary>
        private static Version DetectCurrentRuntimeVersion()
        {
            return Environment.Version;
        }

        /// <summary>
        /// Attempts to detect the current runtime.
        /// </summary>
        private static FrameworkRuntime DetectCurrentRuntime()
        {
            if (Type.GetType("Mono.RuntimeStructs") != null)
                return FrameworkRuntime.Mono;

            if (String.Equals("System.Private.CoreLib", typeof(Object).Assembly.GetName().Name, StringComparison.Ordinal))
                return FrameworkRuntime.CoreCLR;


            return FrameworkRuntime.CLR;
        }

        /// <summary>
        /// Attempts to detect the current platform.
        /// </summary>
        private static FrameworkPlatform DetectCurrentPlatform(out String machineHardwareName)
        {
            machineHardwareName = Environment.OSVersion.Platform.ToString();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return FrameworkPlatform.Windows;

                case PlatformID.Unix:
                    {
                        var buf = IntPtr.Zero;
                        try
                        {
                            buf = Marshal.AllocHGlobal(8192);
                            if (FrameworkNative.uname(buf) == 0)
                            {
                                machineHardwareName = Marshal.PtrToStringAnsi(buf);
                                if (String.Equals("Darwin", machineHardwareName, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (Type.GetType("UIKit.UIApplicationDelegate, Xamarin.iOS") != null)
                                    {
                                        return FrameworkPlatform.iOS;
                                    }
                                    else
                                    {
                                        return FrameworkPlatform.macOS;
                                    }
                                }
                                else
                                {
                                    if (Type.GetType("Android.App.Activity, Mono.Android") != null)
                                    {
                                        return FrameworkPlatform.Android;
                                    }
                                    else
                                    {
                                        return FrameworkPlatform.Linux;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            if (buf != IntPtr.Zero)
                                Marshal.FreeHGlobal(buf);
                        }
                    }
                    break;                    
            }

            throw new PlatformNotSupportedException();
        }
    }
}
