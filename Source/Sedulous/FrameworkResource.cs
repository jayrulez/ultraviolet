using System;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents an object which encapsulates some native or implementation-specific resource.
    /// </summary>
    public abstract class FrameworkResource : IFrameworkComponent, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkResource"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        protected FrameworkResource(FrameworkContext context)
        {
            Contract.Require(context, nameof(context));

            this.context = context;
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        public FrameworkContext FrameworkContext
        {
            get { return context; }
        }

        /// <summary>
        /// Gets a value indicating whether the object has been disposed.
        /// </summary>
        public Boolean Disposed
        {
            get { return disposed; }
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the object is being disposed; <see langword="false"/> if the object is being finalized.</param>
        protected virtual void Dispose(Boolean disposing)
        {
            disposed = true;
        }

        // Property values.
        private readonly FrameworkContext context;

        // State values.
        private Boolean disposed;
    }
}
