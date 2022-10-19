using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a collection of Sedulous plugins.
    /// </summary>
    public sealed class SedulousPluginCollection : SedulousCollection<SedulousPlugin>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousPluginCollection"/> class.
        /// </summary>
        /// <param name="owner">The configuration object which owns this collection.</param>
        public SedulousPluginCollection(SedulousConfiguration owner)
        {
            Contract.Require(owner, nameof(owner));

            this.owner = owner;
        }

        /// <summary>
        /// Adds a plugin to the collection.
        /// </summary>
        /// <param name="plugin">The plugin to add to the collection.</param>
        public void Add(SedulousPlugin plugin)
        {
            Contract.Require(plugin, nameof(plugin));

            plugin.Register(owner);

            AddInternal(plugin);
        }

        // State values.
        private readonly SedulousConfiguration owner;
    }
}
