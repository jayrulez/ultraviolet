using System;
using System.Collections;
using System.Collections.Generic;

namespace Sedulous.Platform
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="IFrameworkWindowInfo"/>.
    /// </summary>
    public sealed class DummyFrameworkWindowInfo : IFrameworkWindowInfo
    {
        /// <inheritdoc/>
        public void DesignatePrimary(IFrameworkWindow window)
        {
        }

        /// <inheritdoc/>
        public IFrameworkWindow GetByID(Int32 id)
        {
            return null;
        }

        /// <inheritdoc/>
        public IFrameworkWindow GetPrimary()
        {
            return null;
        }

        /// <inheritdoc/>
        public IFrameworkWindow GetCurrent()
        {
            return null;
        }

        /// <inheritdoc/>
        public IFrameworkWindow Create(String caption, Int32 x, Int32 y, Int32 width, Int32 height, WindowFlags flags = WindowFlags.None)
        {
            return null;
        }

        /// <inheritdoc/>
        public IFrameworkWindow CreateFromNativePointer(IntPtr ptr)
        {
            return null;
        }

        /// <inheritdoc/>
        public Boolean Destroy(IFrameworkWindow window)
        {
            return true;
        }

        /// <inheritdoc/>
        public Boolean DestroyByID(Int32 id)
        {
            return true;
        }

        /// <inheritdoc/>
        IEnumerator<IFrameworkWindow> IEnumerable<IFrameworkWindow>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public List<IFrameworkWindow>.Enumerator GetEnumerator()
        {
            return windows.GetEnumerator();
        }

        /// <inheritdoc/>
        public event FrameworkWindowInfoEventHandler WindowCreated
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event FrameworkWindowInfoEventHandler WindowDestroyed
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event FrameworkWindowInfoEventHandler PrimaryWindowChanging
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event FrameworkWindowInfoEventHandler PrimaryWindowChanged
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event FrameworkWindowInfoEventHandler CurrentWindowChanging
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event FrameworkWindowInfoEventHandler CurrentWindowChanged
        {
            add { }
            remove { }
        }

        // State values.
        private readonly List<IFrameworkWindow> windows = new List<IFrameworkWindow>();
    }
}
