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
        public event SedulousWindowInfoEventHandler WindowCreated
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event SedulousWindowInfoEventHandler WindowDestroyed
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event SedulousWindowInfoEventHandler PrimaryWindowChanging
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event SedulousWindowInfoEventHandler PrimaryWindowChanged
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event SedulousWindowInfoEventHandler CurrentWindowChanging
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public event SedulousWindowInfoEventHandler CurrentWindowChanged
        {
            add { }
            remove { }
        }

        // State values.
        private readonly List<IFrameworkWindow> windows = new List<IFrameworkWindow>();
    }
}
