using System.Reflection;

namespace Sedulous.Shims.NETCore
{
    /// <summary>
    /// NETCore implementation of the Sedulous Framework platform compatibility shim.
    /// </summary>
    internal sealed class NETCoreSedulousPlatformCompatibilityShim : ISedulousPlatformCompatibilityShim
    {
        /// <inheritdoc />
        public Assembly Assembly => typeof(NETCoreSedulousPlatformCompatibilityShim).Assembly;
    }
}
