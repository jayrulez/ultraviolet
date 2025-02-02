﻿using System.Collections;
using System.Collections.Generic;

namespace Sedulous.Presentation.Controls
{
    partial class RowDefinitionCollection : IEnumerable<RowDefinition>
    {
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="List{RowDefinition}.Enumerator"/> object that can be used to iterate through the collection.</returns>
        public List<RowDefinition>.Enumerator GetEnumerator()
        {
            return storage.Count == 0 ? implicitStorage.GetEnumerator() : storage.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<RowDefinition> IEnumerable<RowDefinition>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
