using System;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a singleton resource.  Only one instance of the resource will be created
    /// during the lifespan of a particular Framework context, but the resource will be destroyed
    /// and recreated if a new context is introduced.
    /// </summary>
    /// <typeparam name="T">The type of object which is owned by the singleton.</typeparam>
    public sealed class FrameworkSingleton<T> where T : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkSingleton{T}"/> class.
        /// </summary>
        /// <param name="initializer">A function which initializes a new instance of the bound resource.</param>
        public FrameworkSingleton(Func<FrameworkContext, T> initializer)
            : this(FrameworkSingletonFlags.None, initializer)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkSingleton{T}"/> class.
        /// </summary>
        /// <param name="flags">A set of flags which modify the singleton's behavior.</param>
        /// <param name="initializer">A function which initializes a new instance of the bound resource.</param>
        public FrameworkSingleton(FrameworkSingletonFlags flags, Func<FrameworkContext, T> initializer)
        {
            Contract.Require(initializer, nameof(initializer));

            this.Flags = flags;
            this.initializer = initializer;

            var uv = FrameworkContext.RequestCurrent();
            if (uv != null && uv.IsInitialized && (flags & FrameworkSingletonFlags.Lazy) != FrameworkSingletonFlags.Lazy)
                InitializeResource();

            FrameworkContext.ContextInitialized += FrameworkContext_ContextInitialized;
            FrameworkContext.ContextInvalidated += FrameworkContext_ContextInvalidated;
        }

        /// <summary>
        /// Implicitly converts a bound resource to its underlying resource object.
        /// </summary>
        /// <param name="resource">The bound resource to convert.</param>
        /// <returns>The converted resource.</returns>
        public static implicit operator T(FrameworkSingleton<T> resource)
        {
            return resource == null ? null : resource.Value;
        }

        /// <summary>
        /// Initializes the singleton resource, assuming the Framework context is currently in a valid state.
        /// </summary>
        /// <returns><see langword="true"/> if the instance was successfully initialized; otherwise, <see langword="false"/>.</returns>
        public Boolean InitializeResource()
        {
            if (initialized)
                return true;

            var context = FrameworkContext.RequestCurrent();
            if (context == null || context.Disposing || context.Disposed)
                return false;

            if (resource == null || resource.FrameworkContext != context)
            {
                if (ShouldInitializeResource(context))
                    resource = initializer(context);
            }

            initialized = true;
            return true;
        }

        /// <summary>
        /// Gets the singleton's flags.
        /// </summary>
        public FrameworkSingletonFlags Flags { get; }

        /// <summary>
        /// Gets the bound resource.
        /// </summary>
        public T Value
        {
            get 
            {
                if (!initialized && !InitializeResource())
                    throw new InvalidOperationException(FrameworkStrings.FailedToInitializeSingleton);

                return resource; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the singleton should be initialized for the
        /// specified Framework context.
        /// </summary>
        private Boolean ShouldInitializeResource(FrameworkContext context)
        {
            var disabledInServiceMode = (Flags & FrameworkSingletonFlags.DisabledInServiceMode) == FrameworkSingletonFlags.DisabledInServiceMode;
            if (disabledInServiceMode && context.IsRunningInServiceMode)
                return false;

            return true;
        }
        
        /// <summary>
        /// Handles the <see cref="FrameworkContext.ContextInitialized"/> event.
        /// </summary>
        private void FrameworkContext_ContextInitialized(object sender, EventArgs e)
        {
            if ((Flags & FrameworkSingletonFlags.Lazy) != FrameworkSingletonFlags.Lazy)
                InitializeResource();
        }

        /// <summary>
        /// Handles the <see cref="FrameworkContext.ContextInvalidated"/> event.
        /// </summary>
        private void FrameworkContext_ContextInvalidated(object sender, EventArgs e)
        {
            SafeDispose.DisposeRef(ref resource);
            initialized = false;
        }

        // State values.
        private readonly Func<FrameworkContext, T> initializer;
        private T resource;
        private Boolean initialized;
    }
}
