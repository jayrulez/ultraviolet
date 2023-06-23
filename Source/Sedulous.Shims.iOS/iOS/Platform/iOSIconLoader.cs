using System;
using System.Linq;
using System.Reflection;
using Sedulous.Graphics;
using Sedulous.Platform;

namespace Sedulous.Shims.iOS.Platform
{
    /// <summary>
    /// Represents an implementation of the <see cref="IconLoader"/> class for the iOS platform.
    /// </summary>
    public sealed class iOSIconLoader : IconLoader
    {
        /// <inheritdoc/>
        public override Surface2D LoadIcon()
        {
            return null;
        }
    }
}
