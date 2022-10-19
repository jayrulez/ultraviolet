using Sedulous.Core;
using Sedulous.Graphics;
using Sedulous.Input;
using Sedulous.Platform;
using Sedulous.Shims.NETCore.Graphics;
using Sedulous.Shims.NETCore.Input;
using Sedulous.Shims.NETCore.Platform;

namespace Sedulous.Shims.NETCore
{
    /// <summary>
    /// Initializes factory methods for the .NET Core 3.0 platform compatibility shim.
    /// </summary>
    internal sealed class NETCoreFactoryInitializer : ISedulousFactoryInitializer
    {
        /// <summary>
        /// Initializes the specified factory.
        /// </summary>
        /// <param name="owner">The Sedulous context that owns the initializer.</param>
        /// <param name="factory">The <see cref="SedulousFactory"/> to initialize.</param>
        public void Initialize(SedulousContext owner, SedulousFactory factory)
        {
            factory.SetFactoryMethod<SurfaceSourceFactory>((stream) => new NETCoreSurfaceSource(stream));
            factory.SetFactoryMethod<SurfaceSaverFactory>(() => new NETCoreSurfaceSaver());
            factory.SetFactoryMethod<IconLoaderFactory>(() => new NETCoreIconLoader());
            factory.SetFactoryMethod<FileSystemServiceFactory>(() => new FileSystemService());
            factory.SetFactoryMethod<ScreenRotationServiceFactory>((display) => new NETCoreScreenOrientationService(display));

            switch (SedulousPlatformInfo.CurrentPlatform)
            {
                case SedulousPlatform.Windows:
                    factory.SetFactoryMethod<ScreenDensityServiceFactory>((display) => new NETCoreScreenDensityService_Windows(owner, display));
                    break;

                default:
                    factory.SetFactoryMethod<ScreenDensityServiceFactory>((display) => new NETCoreScreenDensityService(owner, display));
                    break;
            }

            var softwareKeyboardService = new NETCoreSoftwareKeyboardService();
            factory.SetFactoryMethod<SoftwareKeyboardServiceFactory>(() => softwareKeyboardService);
        }
    }
}
