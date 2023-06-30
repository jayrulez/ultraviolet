using System;
using Sedulous.Core.Data;

namespace Sedulous.Content
{
    /// <summary>
    /// Represents an instance of <see cref="DataObject"/> which is designed for use with the Sedulous Framework.
    /// </summary>
    /// <inheritdoc/>
    [CLSCompliant(false)]
    public abstract class FrameworkDataObject : DataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkDataObject"/> class.
        /// </summary>
        /// <param name="key">The object's uniquely identifying key.</param>
        /// <param name="id">The object's globally-unique identifier.</param>
        public FrameworkDataObject(String key, Guid id)
            : base(key, id)
        {

        }

        /// <summary>
        /// Gets the Framework context.
        /// </summary>
        public FrameworkContext FrameworkContext =>
            FrameworkContext.DemandCurrent();
    }
}
