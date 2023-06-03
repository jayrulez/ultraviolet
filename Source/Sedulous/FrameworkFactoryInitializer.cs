using Sedulous.Graphics.Graphics2D;

namespace Sedulous
{
    /// <summary>
    /// Initializes factory methods for the Framework core.
    /// </summary>
    internal sealed class FrameworkFactoryInitializer : IFrameworkFactoryInitializer
    {
        /// <summary>
        /// Initializes the specified factory.
        /// </summary>
        /// <param name="owner">The Sedulous context that owns the initializer.</param>
        /// <param name="factory">The <see cref="FrameworkFactory"/> to initialize.</param>
        public void Initialize(FrameworkContext owner, FrameworkFactory factory)
        {
            factory.SetFactoryMethod(owner.IsRunningInServiceMode ? 
                new SpriteBatchFactory((uv) => null) : 
                new SpriteBatchFactory((uv) => new SpriteBatch(uv)));
        }
    }
}
