using System;
using System.Runtime.InteropServices;

namespace Sedulous.Core
{
    /// <summary>
    /// Contains information relating to the current platform.
    /// </summary>
    public static class SedulousPlatformInfo
    {
        /// <summary>
        /// Initializes the <see cref="SedulousPlatformInfo"/> type.
        /// </summary>
        static SedulousPlatformInfo()
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
        public static Boolean IsRuntimeCodeGenerationSupported(SedulousPlatform platform)
        {
            if (platform == SedulousPlatform.iOS || platform == SedulousPlatform.Android)
                return false;

            return true;
        }

        /// <summary>
        /// Gets the version number associated with the executing runtime.
        /// </summary>
        public static Version CurrentRuntimeVersion { get; }

        /// <summary>
        /// Gets an <see cref="SedulousRuntime"/> value indicating which of Sedulous's supported
        /// runtimes is currently executing this application.
        /// </summary>
        public static SedulousRuntime CurrentRuntime { get; }

        /// <summary>
        /// Gets an <see cref="SedulousPlatform"/> value indicating which of Sedulous's supported
        /// platforms is currently executing this application.
        /// </summary>
        public static SedulousPlatform CurrentPlatform { get; }

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
        private static SedulousRuntime DetectCurrentRuntime()
        {
            if (Type.GetType("Mono.RuntimeStructs") != null)
                return SedulousRuntime.Mono;

            if (String.Equals("System.Private.CoreLib", typeof(Object).Assembly.GetName().Name, StringComparison.Ordinal))
                return SedulousRuntime.CoreCLR;


            return SedulousRuntime.CLR;
        }

        /// <summary>
        /// Attempts to detect the current platform.
        /// </summary>
        private static SedulousPlatform DetectCurrentPlatform(out String machineHardwareName)
        {
            machineHardwareName = Environment.OSVersion.Platform.ToString();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return SedulousPlatform.Windows;

                case PlatformID.Unix:
                    {
                        var buf = IntPtr.Zero;
                        try
                        {
                            buf = Marshal.AllocHGlobal(8192);
                            if (SedulousNative.uname(buf) == 0)
                            {
                                machineHardwareName = Marshal.PtrToStringAnsi(buf);
                                if (String.Equals("Darwin", machineHardwareName, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (Type.GetType("UIKit.UIApplicationDelegate, Xamarin.iOS") != null)
                                    {
                                        return SedulousPlatform.iOS;
                                    }
                                    else
                                    {
                                        return SedulousPlatform.macOS;
                                    }
                                }
                                else
                                {
                                    if (Type.GetType("Android.App.Activity, Mono.Android") != null)
                                    {
                                        return SedulousPlatform.Android;
                                    }
                                    else
                                    {
                                        return SedulousPlatform.Linux;
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
