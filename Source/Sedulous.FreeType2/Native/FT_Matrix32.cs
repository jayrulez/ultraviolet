﻿using System;
using System.Runtime.InteropServices;

namespace Sedulous.FreeType2.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FT_Matrix32
    {
        public Int32 xx, xy;
        public Int32 yx, yy;
    }
#pragma warning restore 1591
}
