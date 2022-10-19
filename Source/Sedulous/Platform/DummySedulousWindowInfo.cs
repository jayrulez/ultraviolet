using System;
using System.Collections;
using System.Collections.Generic;

namespace Sedulous.Platform
{
    /// <summary>
    /// Represents a dummy implementation of <see cref="ISedulousWindowInfo"/>.
    /// </summary>
    public sealed class DummySedulousWindowInfo : ISedulousWindowInfo
    {
        /// <inheritdoc/>
        public void DesignatePrimary(ISedulousWindow window)
        {
        }

        /// <inheritdoc/>
        public ISedulousWindow GetByID(Int32 id)
        {
            return null;
        }

        /// <inheritdoc/>
        public ISedulousWindow GetPrimary()
        {
            return null;
        }

        /// <inheritdoc/>
        public ISedulousWindow GetCurrent()
        {
            return null;
        }

        /// <inheritdoc/>
        public ISedulousWindow Create(String caption, Int32 x, Int32 y, Int32 width, Int32 height, WindowFlags flags = WindowFlags.None)
        {
            return null;
        }

        /// <inheritdoc/>
        public ISedulousWindow CreateFromNativePointer(IntPtr ptr)
        {
            return null;
        }

        /// <inheritdoc/>
        public Boolean Destroy(ISedulousWindow window)
        {
            return true;
        }

        /// <inheritdoc/>
        public Boolean DestroyByID(Int32 id)
        {
            return true;
        }

        /// <inheritdoc/>
        IEnumerator<ISedulousWindow> IEnumerable<ISedulousWindow>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public List<ISedulousWindow>.Enumerator GetEnumerator()
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
        private readonly List<ISedulousWindow> windows = new List<ISedulousWindow>();
    }
}
