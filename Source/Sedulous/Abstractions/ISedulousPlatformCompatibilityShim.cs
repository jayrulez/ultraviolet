using System.Reflection;

namespace Sedulous
{
    /// <summary>
    /// Represents a Sedulous Framework platform compatibility shim.
    /// </summary>
    public interface ISedulousPlatformCompatibilityShim
    {
        /// <summary>
        /// The assembly of the shim.
        /// </summary>
        Assembly Assembly { get; }
    }
}
