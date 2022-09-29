using System.Reflection;

namespace Ultraviolet
{
    /// <summary>
    /// Represents a Ultraviolet Framework platform compatibility shim.
    /// </summary>
    public interface IUltravioletPlatformCompatibilityShim
    {
        /// <summary>
        /// The assembly of the shim.
        /// </summary>
        Assembly Assembly { get; }
    }
}
