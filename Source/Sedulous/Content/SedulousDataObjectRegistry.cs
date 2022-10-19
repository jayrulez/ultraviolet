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
    public abstract class SedulousDataObjectRegistry<T> : DataObjectRegistry<T> where T : SedulousDataObject
    {
        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        public SedulousContext Sedulous =>
            SedulousContext.DemandCurrent();

        /// <inheritdoc/>
        protected override void OnRegistered()
        {
            SedulousContext.ContextInvalidated += SedulousContext_ContextInvalidated;

            base.OnRegistered();
        }

        /// <inheritdoc/>
        protected override void OnUnregistered()
        {
            SedulousContext.ContextInvalidated -= SedulousContext_ContextInvalidated;

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