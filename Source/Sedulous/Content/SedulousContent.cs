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
    public sealed class SedulousContent : SedulousResource, ISedulousContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousContent"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public SedulousContent(SedulousContext uv)
            : base(uv)
        {

        }

        /// <inheritdoc/>
        public void RegisterImportersAndProcessors(Assembly asm)
        {
            Contract.Require(asm, nameof(asm));
            Contract.EnsureNotDisposed(this, Disposed);

            importers.RegisterAssembly(asm);
            processors.RegisterAssembly(asm);
        }

        /// <inheritdoc/>
        public void RegisterImportersAndProcessors(IEnumerable<Assembly> additionalAssemblies = null)
        {
            Contract.EnsureNot(registered, SedulousStrings.ContentHandlersAlreadyRegistered);
            Contract.EnsureNotDisposed(this, Disposed);

            var asmCore = typeof(SedulousContext).Assembly;
            var asmImpl = Sedulous.GetType().Assembly;
            var asmEntry = Assembly.GetEntryAssembly();
            var asmShim = Sedulous.PlatformCompatibilityShimAssembly;
            var asmViews = Sedulous.ViewProviderAssembly;

            var assemblies = new[] { asmCore, asmImpl, asmShim, asmViews }
                .Union(additionalAssemblies ?? Enumerable.Empty<Assembly>()).Where(x => x != null).Distinct();

            foreach (var asm in assemblies)
                RegisterImportersAndProcessors(asm);

            if (asmEntry != null)
                RegisterImportersAndProcessors(asmEntry);

            registered = true;
        }

        /// <inheritdoc/>
        public void Update(SedulousTime time)
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
        public event SedulousSubsystemUpdateEventHandler Updating;

        /// <summary>
        /// Raises the <see cref="Updating"/> event.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Update(SedulousTime)"/>.</param>
        private void OnUpdating(SedulousTime time) =>
            Updating?.Invoke(this, time);
        
        // Registered content importers and processors.
        private Boolean registered;
        private readonly ContentManifestRegistry manifests = new ContentManifestRegistry();
        private readonly ContentImporterRegistry importers = new ContentImporterRegistry();
        private readonly ContentProcessorRegistry processors = new ContentProcessorRegistry();
    }
}
