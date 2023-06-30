using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sedulous.Core;

namespace Sedulous.Content
{
    /// <summary>
    /// Represents the core implementation of the Sedulous content subsystem.
    /// </summary>
    public sealed class ContentSubsystem : FrameworkResource, IContentSubsystem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentSubsystem"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        public ContentSubsystem(FrameworkContext context)
            : base(context)
        {

        }

        /// <inheritdoc/>
        public void Update(FrameworkTime time)
        {
            Contract.EnsureNotDisposed(this, Disposed);

            OnUpdating(time);
        }

        /// <inheritdoc/>
        public ContentManifestRegistry Manifests => manifests;

        /// <inheritdoc/>
        public ContentImporterRegistry Importers => importers;

        /// <inheritdoc/>
        public ContentProcessorRegistry Processors => processors;

        /// <inheritdoc/>
        public event FrameworkSubsystemUpdateEventHandler Updating;

        /// <summary>
        /// Raises the <see cref="Updating"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="FrameworkContext.Update(FrameworkTime)"/>.</param>
        private void OnUpdating(FrameworkTime time) =>
            Updating?.Invoke(this, time);
        
        // Registered content importers and processors.
        private readonly ContentManifestRegistry manifests = new ContentManifestRegistry();
        private readonly ContentImporterRegistry importers = new ContentImporterRegistry();
        private readonly ContentProcessorRegistry processors = new ContentProcessorRegistry();
    }
}
