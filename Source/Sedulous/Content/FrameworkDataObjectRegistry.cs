using System;
using System.IO;
using Sedulous.Core.Data;
using Sedulous.Platform;

namespace Sedulous.Content
{
    /// <summary>
    /// Represents an instance of <see cref="DataObjectRegistry{T}"/> which is designed for use with the Sedulous Framework.
    /// </summary>
    /// <inheritdoc/>
    [CLSCompliant(false)]
    public abstract class FrameworkDataObjectRegistry<T> : DataObjectRegistry<T> where T : FrameworkDataObject
    {
        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        public FrameworkContext Sedulous =>
            FrameworkContext.DemandCurrent();

        /// <inheritdoc/>
        protected override void OnRegistered()
        {
            FrameworkContext.ContextInvalidated += SedulousContext_ContextInvalidated;

            base.OnRegistered();
        }

        /// <inheritdoc/>
        protected override void OnUnregistered()
        {
            FrameworkContext.ContextInvalidated -= SedulousContext_ContextInvalidated;

            base.OnUnregistered();
        }

        /// <inheritdoc/>
        protected override Stream OpenFileStream(String path)
        {
            var fss = FileSystemService.Create();
            return fss.OpenRead(path);
        }

        /// <summary>
        /// Handles Sedulous context invalidation.
        /// </summary>
        private void SedulousContext_ContextInvalidated(Object sender, EventArgs e)
        {
            Clear();
        }
    }
}