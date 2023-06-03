using System;
using Sedulous.Core;

namespace Sedulous.FreeType2.Native
{
#pragma warning disable 1591
    /// <summary>
    /// Contains FreeType2 helper methods.
    /// </summary>
    unsafe static partial class FreeTypeNative
    {
        public static Boolean Use64BitInterface => FrameworkPlatformInfo.CurrentPlatform != FrameworkPlatform.Windows && Environment.Is64BitProcess;
    }
#pragma warning restore 1591
}
