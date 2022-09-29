using System.Reflection;

namespace Ultraviolet.Shims.NETCore3
{
    /// <summary>
    /// NETCore3 implementation of the Ultraviolet Framework platform compatibility shim.
    /// </summary>
    internal sealed class NETCore3UltravioletPlatformCompatibilityShim : IUltravioletPlatformCompatibilityShim
    {
        /// <inheritdoc />
        public Assembly Assembly => typeof(NETCore3UltravioletPlatformCompatibilityShim).Assembly;
    }
}
