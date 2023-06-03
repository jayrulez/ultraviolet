using Sedulous.Graphics;
using Sedulous.Input;
using Sedulous.Platform;
using Sedulous.Shims.Android.Graphics;
using Sedulous.Shims.Android.Input;
using Sedulous.Shims.Android.Platform;

namespace Sedulous.Shims.Android
{
    /// <summary>
    /// Initializes factory methods for the Android platform compatibility shim.
    /// </summary>
    internal sealed class AndroidFactoryInitializer : IFrameworkFactoryInitializer
    {
        /// <inheritdoc/>
        public void Initialize(FrameworkContext owner, FrameworkFactory factory)
        {
            factory.SetFactoryMethod<SurfaceSourceFactory>((stream) => new AndroidSurfaceSource(stream));
            factory.SetFactoryMethod<SurfaceSaverFactory>(() => new AndroidSurfaceSaver());
            factory.SetFactoryMethod<IconLoaderFactory>(() => new AndroidIconLoader());
            factory.SetFactoryMethod<FileSystemServiceFactory>(() => new FileSystemService());
            factory.SetFactoryMethod<ScreenRotationServiceFactory>((display) => new AndroidScreenRotationService(display));
            factory.SetFactoryMethod<ScreenDensityServiceFactory>((display) => new AndroidScreenDensityService(display));
            factory.SetFactoryMethod<AssemblyLoaderServiceFactory>(() => new AndroidAssemblyLoaderService());

            var softwareKeyboardService = new AndroidSoftwareKeyboardService();
            factory.SetFactoryMethod<SoftwareKeyboardServiceFactory>(() => softwareKeyboardService);
        }
    }
}