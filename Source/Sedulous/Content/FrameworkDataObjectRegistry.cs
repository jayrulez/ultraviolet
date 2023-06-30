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
        /// Gets the Framework context.
        /// </summary>
        public FrameworkContext FrameworkContext =>
            FrameworkContext.DemandCurrent();

        /// <inheritdoc/>
        protected override void OnRegistered()
        {
            FrameworkContext.ContextInvalidated += FrameworkContext_ContextInvalidated;

            base.OnRegistered();
        }

        /// <inheritdoc/>
        protected override void OnUnregistered()
        {
            FrameworkContext.ContextInvalidated -= FrameworkContext_ContextInvalidated;

            base.OnUnregistered();
        }

        /// <inheritdoc/>
        protected override Stream OpenFileStream(String path)
        {
            var fss = FileSystemService.Create();
            return fss.OpenRead(path);
        }

        /// <summary>
        /// Handles Framework context invalidation.
        /// </summary>
        private void FrameworkContext_ContextInvalidated(Object sender, EventArgs e)
        {
            Clear();
        }
    }
}