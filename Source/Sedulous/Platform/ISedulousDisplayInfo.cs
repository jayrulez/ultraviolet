using System;
using System.Collections.Generic;

namespace Sedulous.Platform
{
    /// <summary>
    /// Provides access to information concerning the system's attached display devices.
    /// </summary>
    public interface ISedulousDisplayInfo : IEnumerable<ISedulousDisplay>
    {
        /// <summary>
        /// Gets the display with the specified index.
        /// </summary>
        /// <param name="index">The index of the display to retrieve.</param>
        /// <returns>The display with the specified index.</returns>
        ISedulousDisplay this[Int32 index]
        {
            get;
        }

        /// <summary>
        /// Gets the application's primary display device.
        /// </summary>
        ISedulousDisplay PrimaryDisplay
        {
            get;
        }

        /// <summary>
        /// Gets the number of connected displays.
        /// </summary>
        Int32 Count
        {
            get;
        }
    }
}
