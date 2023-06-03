
namespace Sedulous
{
    /// <summary>
    /// Represents an application component which participates in an Sedulous context.
    /// </summary>
    public interface IFrameworkComponent
    {
        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        FrameworkContext Sedulous { get; }
    }
}
