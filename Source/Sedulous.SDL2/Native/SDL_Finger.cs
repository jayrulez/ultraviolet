﻿using System;
using System.Runtime.InteropServices;

namespace Sedulous.SDL2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_Finger
    {
        public Int64 id;
        public Single x;
        public Single y;
        public Single pressure;
    }
#pragma warning restore 1591
}