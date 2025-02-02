﻿using System;
using System.Runtime.InteropServices;

namespace Sedulous.Graphics.PackedVector
{
    /// <summary>
    /// Represents an 8-bit packed vector consisting of 1 unsigned 8-bit value.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 0, Size = sizeof(Byte))]
    public partial struct Byte1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Byte1"/> structure from the specified vector components.
        /// </summary>
        /// <param name="x">The x-component from  which to create the packed instance.</param>
        public Byte1(Single x)
        {
            this.X = (Byte)PackedVectorUtils.PackUnsigned(PackingMask, x);
        }

        /// <inheritdoc/>
        public override String ToString() => X.ToString("X");

        /// <summary>
        /// Converts the <see cref="Byte1"/> instance to a <see cref="Single"/> instance.
        /// </summary>
        /// <returns>The <see cref="Single"/> instance which was created.</returns>
        public Single ToSingle()
        {
            return PackedVectorUtils.UnpackUnsigned(PackingMask, X);
        }

        /// <summary>
        /// The vector's X component.
        /// </summary>
        [CLSCompliant(false)]
        [FieldOffset(0)]
        public Byte X;

        // Packing mask for this vector type.
        private const UInt32 PackingMask = 0xFF;
    }
}
