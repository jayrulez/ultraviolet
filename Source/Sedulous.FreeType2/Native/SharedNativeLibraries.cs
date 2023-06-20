using Sedulous.Core;
using Sedulous.Core.Native;

namespace Sedulous.FreeType2.Native
{
#pragma warning disable 1591
    internal sealed class SharedNativeLibraries
    {
        public static readonly Sedulous.Core.Native.NativeLibrary libpng;
        public static readonly Sedulous.Core.Native.NativeLibrary libharfbuzz;
        public static readonly Sedulous.Core.Native.NativeLibrary libfreetype;
        
        static SharedNativeLibraries()
        {
            switch (FrameworkPlatformInfo.CurrentPlatform)
            {
                default:
                    libpng = new Sedulous.Core.Native.NativeLibrary("libpng16");
                    break;
            }
            switch (FrameworkPlatformInfo.CurrentPlatform)
            {
                case FrameworkPlatform.Linux:
                    libharfbuzz = new Sedulous.Core.Native.NativeLibrary("libharfbuzz");
                    break;
                case FrameworkPlatform.macOS:
                    libharfbuzz = new Sedulous.Core.Native.NativeLibrary("libharfbuzz");
                    break;
                default:
                    libharfbuzz = new Sedulous.Core.Native.NativeLibrary("harfbuzz");
                    break;
            }
            switch (FrameworkPlatformInfo.CurrentPlatform)
            {
                case FrameworkPlatform.Linux:
                    libfreetype = new Sedulous.Core.Native.NativeLibrary("libfreetype");
                    break;
                case FrameworkPlatform.macOS:
                    libfreetype = new Sedulous.Core.Native.NativeLibrary("libfreetype");
                    break;
                default:
                    libfreetype = new Sedulous.Core.Native.NativeLibrary("freetype");
                    break;
            }
        }
    }
#pragma warning restore 1591
}
