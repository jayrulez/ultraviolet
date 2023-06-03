using Sedulous.Graphics;
using Sedulous.OpenGL;
using Sedulous.Platform;
using Sedulous.SDL2.Graphics;
using Sedulous.SDL2.Platform;

namespace Sedulous.SDL2
{
    /// <summary>
    /// Initializes factory methods for the SDL implementation of the graphics subsystem.
    /// </summary>
    public sealed class SDLFactoryInitializer : IFrameworkFactoryInitializer
    {
        /// <inheritdoc/>
        public void Initialize(FrameworkContext owner, FrameworkFactory factory)
        {
            // Core classes.
            factory.SetFactoryMethod<PlatformNativeSurfaceFactory>((source) => new SDL2PlatformNativeSurface(source));
            factory.SetFactoryMethod<Surface2DFactory>((uv, width, height, options) => new SDL2Surface2D(uv, width, height, options));
            factory.SetFactoryMethod<Surface2DFromSourceFactory>((uv, source, options) => new SDL2Surface2D(uv, source, options));
            factory.SetFactoryMethod<Surface2DFromNativeSurfaceFactory>((uv, surface, options) => new SDL2Surface2D(uv, surface, options));
            factory.SetFactoryMethod<Surface3DFactory>((uv, width, height, depth, bytesPerPixel, options) => new SDL2Surface3D(uv, width, height, depth, bytesPerPixel, options));
            factory.SetFactoryMethod<CursorFactory>((uv, surface, hx, hv) => new SDL2Cursor(uv, surface, hx, hv));

            // Platform services
            var msgboxService = new SDL2MessageBoxService();
            factory.SetFactoryMethod<MessageBoxServiceFactory>(() => msgboxService);
            
            var clipboardService = new SDL2ClipboardService();
            factory.SetFactoryMethod<ClipboardServiceFactory>(() => clipboardService);

            var powerManagementService = new SDL2PowerManagementService();
            factory.SetFactoryMethod<PowerManagementServiceFactory>(() => powerManagementService);

            // Graphics API services
            factory.SetFactoryMethod<OpenGLEnvironmentFactory>((uv) => new SDL2OpenGLEnvironment(uv));
        }
    }
}
