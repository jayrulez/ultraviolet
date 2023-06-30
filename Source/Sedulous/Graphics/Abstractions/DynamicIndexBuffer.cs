using System;
using Sedulous.Core;

namespace Sedulous.Graphics
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="DynamicIndexBuffer"/> class.
    /// </summary>
    /// <param name="context">The Framework context.</param>
    /// <param name="itype">The index element type.</param>
    /// <param name="icount">The index element count.</param>
    /// <returns>The instance of <see cref="DynamicIndexBuffer"/> that was created.</returns>
    public delegate DynamicIndexBuffer DynamicIndexBufferFactory(FrameworkContext context, IndexBufferElementType itype, Int32 icount);

    /// <summary>
    /// Represents a index buffer that is optimized for dynamic updates.
    /// </summary>
    public abstract class DynamicIndexBuffer : IndexBuffer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicIndexBuffer"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        /// <param name="itype">The index element type.</param>
        /// <param name="icount">The index element count.</param>
        public DynamicIndexBuffer(FrameworkContext context, IndexBufferElementType itype, Int32 icount)
            : base(context, itype, icount)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="DynamicIndexBuffer"/> class.
        /// </summary>
        /// <param name="itype">The index element type.</param>
        /// <param name="icount">The index element count.</param>
        /// <returns>The instance of <see cref="DynamicIndexBuffer"/> that was created.</returns>
        public static new DynamicIndexBuffer Create(IndexBufferElementType itype, Int32 icount)
        {
            Contract.EnsureRange(icount > 0, nameof(icount));

            var uv = FrameworkContext.DemandCurrent();
            return uv.GetFactoryMethod<DynamicIndexBufferFactory>()(uv, itype, icount);
        }

        /// <summary>
        /// Gets a value indicating whether the buffer's content has been lost.
        /// </summary>
        public abstract Boolean IsContentLost
        {
            get;
        }

        /// <summary>
        /// Occurs when the buffer's content is lost.
        /// </summary>
        public event EventHandler ContentLost;

        /// <summary>
        /// Raises the <see cref="ContentLost"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnContentLost(EventArgs e) =>
            ContentLost?.Invoke(this, e);
    }
}
