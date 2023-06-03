
namespace Sedulous
{
    /// <summary>
    /// Represents an application component which participates in an Sedulous context.
    /// </summary>
    public interface ISedulousComponent
    {
        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        SedulousContext Sedulous { get; }
    }
}
