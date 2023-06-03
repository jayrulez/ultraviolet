using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a collection of Sedulous plugins.
    /// </summary>
    public sealed class FrameworkPluginCollection : FrameworkCollection<FrameworkPlugin>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkPluginCollection"/> class.
        /// </summary>
        /// <param name="owner">The configuration object which owns this collection.</param>
        public FrameworkPluginCollection(FrameworkConfiguration owner)
        {
            Contract.Require(owner, nameof(owner));

            this.owner = owner;
        }

        /// <summary>
        /// Adds a plugin to the collection.
        /// </summary>
        /// <param name="plugin">The plugin to add to the collection.</param>
        public void Add(FrameworkPlugin plugin)
        {
            Contract.Require(plugin, nameof(plugin));

            plugin.Register(owner);

            AddInternal(plugin);
        }

        // State values.
        private readonly FrameworkConfiguration owner;
    }
}
