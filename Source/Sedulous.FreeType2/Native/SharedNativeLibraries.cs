using Sedulous.Core;
using Sedulous.Core.Native;

namespace Sedulous.FreeType2.Native
{
#pragma warning disable 1591
    internal sealed class SharedNativeLibraries
    {
        public static readonly NativeLibrary libpng;
        public static readonly NativeLibrary libharfbuzz;
        public static readonly NativeLibrary libfreetype;
        
        static SharedNativeLibraries()
        {
            switch (SedulousPlatformInfo.CurrentPlatform)
            {
                default:
                    libpng = new NativeLibrary("libpng16");
                    break;
            }
            switch (SedulousPlatformInfo.CurrentPlatform)
            {
                case SedulousPlatform.Linux:
                    libharfbuzz = new NativeLibrary("libharfbuzz");
                    break;
                case SedulousPlatform.macOS:
                    libharfbuzz = new NativeLibrary("libharfbuzz");
                    break;
                default:
                    libharfbuzz = new NativeLibrary("harfbuzz");
                    break;
            }
            switch (SedulousPlatformInfo.CurrentPlatform)
            {
                case SedulousPlatform.Linux:
                    libfreetype = new NativeLibrary("libfreetype");
                    break;
                case SedulousPlatform.macOS:
                    libfreetype = new NativeLibrary("libfreetype");
                    break;
                default:
                    libfreetype = new NativeLibrary("freetype");
                    break;
            }
        }
    }
#pragma warning restore 1591
}
