using System;
using System.Collections;
using System.Collections.Generic;

namespace Sedulous.Platform
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="IFrameworkDisplayInfo"/>.
    /// </summary>
    public sealed class DummyFrameworkDisplayInfo : IFrameworkDisplayInfo
    {
        /// <inheritdoc/>
        public IFrameworkDisplay this[Int32 ix]
        {
            get { throw new IndexOutOfRangeException(nameof(ix)); }
        }

        /// <inheritdoc/>
        public IFrameworkDisplay PrimaryDisplay
        {
            get { return null; }
        }

        /// <inheritdoc/>
        public Int32 Count
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets an enumerator for the collection.
        /// </summary>
        /// <returns>A <see cref="List{T}.Enumerator"/> that iterates through the collection.</returns>
        public List<IFrameworkDisplay>.Enumerator GetEnumerator()
        {
            return displays.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<IFrameworkDisplay> IEnumerable<IFrameworkDisplay>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // State values.
        private readonly List<IFrameworkDisplay> displays = new List<IFrameworkDisplay>();
    }
}
