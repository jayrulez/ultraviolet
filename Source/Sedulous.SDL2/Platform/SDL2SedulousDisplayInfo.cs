using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sedulous.Core;
using Sedulous.Platform;
using static Sedulous.SDL2.Native.SDLNative;

namespace Sedulous.SDL2.Platform
{
    /// <summary>
    /// Represents the SDL2 implementation of the <see cref="ISedulousDisplayInfo"/> interface.
    /// </summary>
    public sealed class SDL2SedulousDisplayInfo : ISedulousDisplayInfo
    {
        /// <summary>
        /// Initializes a new instance of the OpenGLSedulousDisplayInfo class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        public SDL2SedulousDisplayInfo(SedulousContext uv)
        {
            Contract.Require(uv, nameof(uv));

            this.displays = Enumerable.Range(0, SDL_GetNumVideoDisplays())
                .Select(x => new SDL2SedulousDisplay(uv, x))
                .ToList<ISedulousDisplay>();
        }

        /// <summary>
        /// Updates the state of the application's displays.
        /// </summary>
        /// <param name="time">Time elapsed since the last call to <see cref="SedulousContext.Update(SedulousTime)"/>.</param>
        public void Update(SedulousTime time)
        {
            foreach (var display in displays)
                ((SDL2SedulousDisplay)display).Update(time);
        }

        /// <inheritdoc/>
        public ISedulousDisplay this[Int32 ix]
        {
            get { return displays[ix]; }
        }

        /// <inheritdoc/>
        public ISedulousDisplay PrimaryDisplay
        {
            get { return displays.Count == 0 ? null : displays[0]; }
        }

        /// <inheritdoc/>
        public Int32 Count
        {
            get { return displays.Count; }
        }

        /// <inheritdoc/>
        public List<ISedulousDisplay>.Enumerator GetEnumerator()
        {
            return displays.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<ISedulousDisplay> IEnumerable<ISedulousDisplay>.GetEnumerator()
        {
            return GetEnumerator();
        }

        // The list of display devices.  SDL2 never updates its device info, even if
        // devices are added or removed, so we only need to create this once.
        private readonly List<ISedulousDisplay> displays;
    }
}
