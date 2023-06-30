
namespace Sedulous
{
    /// <summary>
    /// Represents an application component which participates in an Framework context.
    /// </summary>
    public interface IFrameworkComponent
    {
        /// <summary>
        /// Gets the Framework context.
        /// </summary>
        FrameworkContext FrameworkContext { get; }
    }
}
