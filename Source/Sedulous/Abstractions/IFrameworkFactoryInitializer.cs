
namespace Sedulous
{
    /// <summary>
    /// Represents an object which injects factory methods into the Sedulous context.
    /// </summary>
    public interface IFrameworkFactoryInitializer
    {
        /// <summary>
        /// Initializes the specified factory.
        /// </summary>
        /// <param name="owner">The Sedulous context that owns the initializer.</param>
        /// <param name="factory">The <see cref="FrameworkFactory"/> to initialize.</param>
        void Initialize(FrameworkContext owner, FrameworkFactory factory);
    }
}
