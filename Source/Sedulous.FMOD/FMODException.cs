using System;
using Sedulous.FMOD.Native;
using static Sedulous.FMOD.Native.FMODNative;

namespace Sedulous.FMOD
{
    /// <summary>
    /// Represents an exception thrown as a result of an FMOD API error.
    /// </summary>
    [Serializable]
    public sealed class FMODException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FMODException"/> class.
        /// </summary>
        public FMODException(FMOD_RESULT result)
            : base(FMOD_ErrorString(result))
        {

        }
    }
}
