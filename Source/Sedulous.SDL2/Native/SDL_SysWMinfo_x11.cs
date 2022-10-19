using System;
using System.Runtime.InteropServices;

namespace Sedulous.SDL2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_SysWMinfo_x11
    {
        public IntPtr display;
        public IntPtr window;
    }
#pragma warning restore 1591
}
