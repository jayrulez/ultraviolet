using Sedulous.Graphics.Graphics2D;

namespace Sedulous
{
    /// <summary>
    /// Initializes factory methods for the Framework core.
    /// </summary>
    internal sealed class SedulousFactoryInitializer : ISedulousFactoryInitializer
    {
        /// <summary>
        /// Initializes the specified factory.
        /// </summary>
        /// <param name="owner">The Sedulous context that owns the initializer.</param>
        /// <param name="factory">The <see cref="SedulousFactory"/> to initialize.</param>
        public void Initialize(SedulousContext owner, SedulousFactory factory)
        {
            factory.SetFactoryMethod(owner.IsRunningInServiceMode ? 
                new SpriteBatchFactory((uv) => null) : 
                new SpriteBatchFactory((uv) => new SpriteBatch(uv)));
        }
    }
}
