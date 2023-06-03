using System;

namespace Sedulous
{
    /// <summary>
    /// Represents a plugin which extends the functionality of the Sedulous Framework.
    /// </summary>
    public abstract class FrameworkPlugin
    {
        /// <summary>
        /// Registers the plugin and allows it to modify the Sedulous configuration prior to context creation.
        /// </summary>
        /// <param name="configuration">The Sedulous configuration.</param>
        public virtual void Register(FrameworkConfiguration configuration) { }

        /// <summary>
        /// Initializes the plugin for the specified Sedulous context.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="factory">The Sedulous factory.</param>
        public virtual void Initialize(FrameworkContext context, FrameworkFactory factory) { }

        /// <summary>
        /// Gets a value indicating whether this plugin has been configured.
        /// </summary>
        public Boolean Configured { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this plugin has been initialized.
        /// </summary>
        public Boolean Initialized { get; internal set; }
    }
}
