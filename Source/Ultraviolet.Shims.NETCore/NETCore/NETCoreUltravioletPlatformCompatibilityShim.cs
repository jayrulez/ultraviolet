using System.Reflection;

namespace Ultraviolet.Shims.NETCore
{
    /// <summary>
    /// NETCore implementation of the Ultraviolet Framework platform compatibility shim.
    /// </summary>
    internal sealed class NETCoreUltravioletPlatformCompatibilityShim : IUltravioletPlatformCompatibilityShim
    {
        /// <inheritdoc />
        public Assembly Assembly => typeof(NETCoreUltravioletPlatformCompatibilityShim).Assembly;
    }
}
