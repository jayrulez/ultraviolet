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
            switch (FrameworkPlatformInfo.CurrentPlatform)
            {
                default:
                    libpng = new NativeLibrary("libpng16");
                    break;
            }
            switch (FrameworkPlatformInfo.CurrentPlatform)
            {
                case FrameworkPlatform.Linux:
                    libharfbuzz = new NativeLibrary("libharfbuzz");
                    break;
                case FrameworkPlatform.macOS:
                    libharfbuzz = new NativeLibrary("libharfbuzz");
                    break;
                default:
                    libharfbuzz = new NativeLibrary("harfbuzz");
                    break;
            }
            switch (FrameworkPlatformInfo.CurrentPlatform)
            {
                case FrameworkPlatform.Linux:
                    libfreetype = new NativeLibrary("libfreetype");
                    break;
                case FrameworkPlatform.macOS:
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
