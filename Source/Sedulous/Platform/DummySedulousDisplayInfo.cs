using System;
using System.Collections;
using System.Collections.Generic;

namespace Sedulous.Platform
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="ISedulousDisplayInfo"/>.
    /// </summary>
    public sealed class DummySedulousDisplayInfo : ISedulousDisplayInfo
    {
        /// <inheritdoc/>
        public ISedulousDisplay this[Int32 ix]
        {
            get { throw new IndexOutOfRangeException(nameof(ix)); }
        }

        /// <inheritdoc/>
        public ISedulousDisplay PrimaryDisplay
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
        public List<ISedulousDisplay>.Enumerator GetEnumerator()
        {
            return displays.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<ISedulousDisplay> IEnumerable<ISedulousDisplay>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // State values.
        private readonly List<ISedulousDisplay> displays = new List<ISedulousDisplay>();
    }
}
