using System;
using Sedulous.Core.Data;

namespace Sedulous.Content
{
    /// <summary>
    /// Represents an instance of <see cref="DataObject"/> which is designed for use with the Sedulous Framework.
    /// </summary>
    /// <inheritdoc/>
    [CLSCompliant(false)]
    public abstract class SedulousDataObject : DataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousDataObject"/> class.
        /// </summary>
        /// <param name="key">The object's uniquely identifying key.</param>
        /// <param name="id">The object's globally-unique identifier.</param>
        public SedulousDataObject(String key, Guid id)
            : base(key, id)
        {

        }

        /// <summary>
        /// Gets the Sedulous context.
        /// </summary>
        public SedulousContext Sedulous =>
            SedulousContext.DemandCurrent();
    }
}
