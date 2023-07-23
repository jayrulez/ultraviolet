﻿using Sedulous.Platform;
using UIKit;

namespace Sedulous.Shims.iOS.Platform
{
    /// <summary>
    /// Represents an implementation of the <see cref="ScreenRotationService"/> class for the iOS platform.
    /// </summary>
    public sealed class iOSScreenOrientationService : ScreenRotationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="iOSScreenOrientationService"/> class.
        /// </summary>
        /// <param name="display">The <see cref="IFrameworkDisplay"/> for which to retrieve rotation information.</param>
        public iOSScreenOrientationService(IFrameworkDisplay display)
            : base(display)
        {

        }

        /// <inheritdoc/>
        public override ScreenRotation ScreenRotation
        {
            get
            {
                var native = UIDevice.CurrentDevice.Orientation;
                switch (native)
                {
                    case UIDeviceOrientation.PortraitUpsideDown:
                        return ScreenRotation.Rotation180;

                    case UIDeviceOrientation.LandscapeLeft:
                        return ScreenRotation.Rotation270;

                    case UIDeviceOrientation.LandscapeRight:
                        return ScreenRotation.Rotation90;

                    default:
                        return ScreenRotation.Rotation0;
                }
            }
        }
    }
}
