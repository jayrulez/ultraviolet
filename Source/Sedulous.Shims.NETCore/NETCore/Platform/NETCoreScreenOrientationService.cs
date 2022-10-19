using Sedulous.Platform;

namespace Sedulous.Shims.NETCore.Platform
{
    /// <summary>
    /// Represents an implementation of the <see cref="ScreenRotationService"/> class for the .NET Core 3.0 platform.
    /// </summary>
    public sealed class NETCoreScreenOrientationService : ScreenRotationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NETCoreScreenOrientationService"/> class.
        /// </summary>
        /// <param name="display">The <see cref="ISedulousDisplay"/> for which to retrieve rotation information.</param>
        public NETCoreScreenOrientationService(ISedulousDisplay display)
            : base(display)
        {

        }

        /// <inheritdoc/>
        public override ScreenRotation ScreenRotation
        {
            get { return ScreenRotation.Rotation0; }
        }
    }
}
