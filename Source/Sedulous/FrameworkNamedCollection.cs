﻿using System;
using System.Collections.Generic;

namespace Sedulous
{
    /// <summary>
    /// Represents an enumerable collection of named objects.
    /// </summary>
    /// <typeparam name="T">The type of item stored in the collection.</typeparam>
    public abstract class FrameworkNamedCollection<T> : FrameworkCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkNamedCollection{T}"/> class.
        /// </summary>
        protected FrameworkNamedCollection()
        {
            this.storageByName = new Dictionary<String, T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkNamedCollection{T}"/> class.
        /// </summary>
        /// <param name="capacity">The collection's initial capacity.</param>
        protected FrameworkNamedCollection(Int32 capacity)
            : base(capacity)
        {
            this.storageByName = new Dictionary<String, T>(capacity);
        }

        /// <summary>
        /// Gets the item with the specified name.
        /// </summary>
        /// <param name="name">The name of the item to retrieve.</param>
        /// <returns>The item with the specified name, or a default value if the item does not exist.</returns>
        public T this[String name]
        {
            get
            {
                T item;
                storageByName.TryGetValue(name, out item);
                return item;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="Sedulous.FrameworkNamedCollection{T}"/>.
        /// </summary>
        public Dictionary<String, T>.KeyCollection Keys
        {
            get { return storageByName.Keys; }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="Sedulous.FrameworkNamedCollection{T}"/>.
        /// </summary>
        public Dictionary<String, T>.ValueCollection Values
        {
            get { return storageByName.Values; }
        }

        /// <summary>
        /// Gets the specified item's name.
        /// </summary>
        /// <param name="item">The item for which to retrieve a name.</param>
        /// <returns>The specified item's name.</returns>
        protected abstract String GetName(T item);

        /// <summary>
        /// Clears the collection.
        /// </summary>
        protected override void ClearInternal()
        {
            storageByName.Clear();
            base.ClearInternal();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        protected override void AddInternal(T item)
        {
            var name = GetName(item);
            if (name == null)
            {
                throw new InvalidOperationException(FrameworkStrings.InvalidNamedCollectionName);
            }
            storageByName.Add(name, item);
            base.AddInternal(item);
        }

        /// <summary>
        /// Removes an item from the collection.
        /// </summary>
        /// <param name="item">The item to remove from the collection.</param>
        /// <returns><see langword="true"/> if the item was removed from the collection; otherwise, <see langword="false"/>.</returns>
        protected override Boolean RemoveInternal(T item)
        {
            var name = GetName(item);
            if (name == null)
            {
                throw new InvalidOperationException(FrameworkStrings.InvalidNamedCollectionName);
            }
            storageByName.Remove(name);
            return base.RemoveInternal(item);
        }

        /// <summary>
        /// Gets a value indicating whether the collection contains the specified item.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns><see langword="true"/> if the collection contains the specified item; otherwise, <see langword="false"/>.</returns>
        protected override Boolean ContainsInternal(T item)
        {
            var name = GetName(item);
            if (name != null)
            {
                return storageByName.ContainsKey(name);
            }
            return base.ContainsInternal(item);
        }

        /// <summary>
        /// Gets the underlying dictionary which associates names with their collection items.
        /// </summary>
        protected Dictionary<String, T> StorageByName
        {
            get { return storageByName; }
        }

        // The collection's backing storage.
        private readonly Dictionary<String, T> storageByName;
    }
}
