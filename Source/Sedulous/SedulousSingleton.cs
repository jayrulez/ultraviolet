using System;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a singleton resource.  Only one instance of the resource will be created
    /// during the lifespan of a particular Sedulous context, but the resource will be destroyed
    /// and recreated if a new context is introduced.
    /// </summary>
    /// <typeparam name="T">The type of object which is owned by the singleton.</typeparam>
    public sealed class SedulousSingleton<T> where T : SedulousResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousSingleton{T}"/> class.
        /// </summary>
        /// <param name="initializer">A function which initializes a new instance of the bound resource.</param>
        public SedulousSingleton(Func<SedulousContext, T> initializer)
            : this(SedulousSingletonFlags.None, initializer)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousSingleton{T}"/> class.
        /// </summary>
        /// <param name="flags">A set of flags which modify the singleton's behavior.</param>
        /// <param name="initializer">A function which initializes a new instance of the bound resource.</param>
        public SedulousSingleton(SedulousSingletonFlags flags, Func<SedulousContext, T> initializer)
        {
            Contract.Require(initializer, nameof(initializer));

            this.Flags = flags;
            this.initializer = initializer;

            var uv = SedulousContext.RequestCurrent();
            if (uv != null && uv.IsInitialized && (flags & SedulousSingletonFlags.Lazy) != SedulousSingletonFlags.Lazy)
                InitializeResource();

            SedulousContext.ContextInitialized += SedulousContext_ContextInitialized;
            SedulousContext.ContextInvalidated += SedulousContext_ContextInvalidated;
        }

        /// <summary>
        /// Implicitly converts a bound resource to its underlying resource object.
        /// </summary>
        /// <param name="resource">The bound resource to convert.</param>
        /// <returns>The converted resource.</returns>
        public static implicit operator T(SedulousSingleton<T> resource)
        {
            return resource == null ? null : resource.Value;
        }

        /// <summary>
        /// Initializes the singleton resource, assuming the Sedulous context is currently in a valid state.
        /// </summary>
        /// <returns><see langword="true"/> if the instance was successfully initialized; otherwise, <see langword="false"/>.</returns>
        public Boolean InitializeResource()
        {
            if (initialized)
                return true;

            var uv = SedulousContext.RequestCurrent();
            if (uv == null || uv.Disposing || uv.Disposed)
                return false;

            if (resource == null || resource.Sedulous != uv)
            {
                if (ShouldInitializeResource(uv))
                    resource = initializer(uv);
            }

            initialized = true;
            return true;
        }

        /// <summary>
        /// Gets the singleton's flags.
        /// </summary>
        public SedulousSingletonFlags Flags { get; }

        /// <summary>
        /// Gets the bound resource.
        /// </summary>
        public T Value
        {
            get 
            {
                if (!initialized && !InitializeResource())
                    throw new InvalidOperationException(SedulousStrings.FailedToInitializeSingleton);

                return resource; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the singleton should be initialized for the
        /// specified Sedulous context.
        /// </summary>
        private Boolean ShouldInitializeResource(SedulousContext uv)
        {
            var disabledInServiceMode = (Flags & SedulousSingletonFlags.DisabledInServiceMode) == SedulousSingletonFlags.DisabledInServiceMode;
            if (disabledInServiceMode && uv.IsRunningInServiceMode)
                return false;

            return true;
        }
        
        /// <summary>
        /// Handles the <see cref="SedulousContext.ContextInitialized"/> event.
        /// </summary>
        private void SedulousContext_ContextInitialized(object sender, EventArgs e)
        {
            if ((Flags & SedulousSingletonFlags.Lazy) != SedulousSingletonFlags.Lazy)
                InitializeResource();
        }

        /// <summary>
        /// Handles the <see cref="SedulousContext.ContextInvalidated"/> event.
        /// </summary>
        private void SedulousContext_ContextInvalidated(object sender, EventArgs e)
        {
            SafeDispose.DisposeRef(ref resource);
            initialized = false;
        }

        // State values.
        private readonly Func<SedulousContext, T> initializer;
        private T resource;
        private Boolean initialized;
    }
}
