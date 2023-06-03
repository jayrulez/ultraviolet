using System;
using Sedulous.Core;
using Sedulous.Platform;
using static Sedulous.SDL2.Native.SDLNative;

namespace Sedulous.SDL2.Platform
{
    /// <summary>
    /// Represents an implentation of <see cref="ScreenDensityService"/> using the SDL2 library.
    /// </summary>
    public sealed class SDL2ScreenDensityService : ScreenDensityService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SDL2ScreenDensityService"/> class.
        /// </summary>
        /// <param name="uv">The Sedulous context.</param>
        /// <param name="display">The <see cref="IFrameworkDisplay"/> for which to retrieve density information.</param>
        public SDL2ScreenDensityService(FrameworkContext uv, IFrameworkDisplay display)
            : base(display)
        {
            Contract.Require(uv, nameof(uv));

            this.uv = uv;
            this.display = display;

            Refresh();
        }

        /// <inheritdoc/>
        public override Boolean Refresh()
        {
            var oldDensityX = densityX;
            var oldDensityY = densityY;
            var oldDensityScale = densityScale;
            var oldDensityBucket = densityBucket;

            Single hdpi, vdpi;
            unsafe
            {
                if (SDL_GetDisplayDPI(display.Index, null, &hdpi, &vdpi) < 0)
                    throw new SDL2Exception();                
            }

            this.densityX = hdpi;
            this.densityY = vdpi;
            this.densityScale = hdpi / 96f;
            this.densityBucket = GuessBucketFromDensityScale(densityScale);

            return oldDensityX != densityX || oldDensityY != densityY || oldDensityScale != densityScale || oldDensityBucket != densityBucket;
        }

        /// <inheritdoc/>
        public override Single DeviceScale
        {
            get { return 1f; }
        }

        /// <inheritdoc/>
        public override Single DensityScale
        {
            get { return densityScale; }
        }

        /// <inheritdoc/>
        public override Single DensityX
        {
            get { return densityX; }
        }

        /// <inheritdoc/>
        public override Single DensityY
        {
            get { return densityY; }
        }

        /// <inheritdoc/>
        public override ScreenDensityBucket DensityBucket
        {
            get { return densityBucket; }
        }

        // State values.
        private readonly FrameworkContext uv;
        private readonly IFrameworkDisplay display;
        private Single densityX;
        private Single densityY;
        private Single densityScale;
        private ScreenDensityBucket densityBucket;
    }
}
